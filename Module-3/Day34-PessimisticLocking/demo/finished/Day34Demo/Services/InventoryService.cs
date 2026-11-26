using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day34Demo.Data;
using Day34Demo.Models;

namespace Day34Demo.Services;

public class InventoryService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] Get all products for display.
    /// </summary>
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// [COMPLETED] Reserve stock using pessimistic locking.
    /// Uses a transaction with SQL Server lock hints (UPDLOCK, ROWLOCK) to prevent
    /// concurrent modifications at the row level.
    /// </summary>
    public async Task<Result<Product>> ReserveStockAsync(int productId, int quantity)
    {
        // Forget rows this context already tracks: the locked read would return their stale values
        _context.ChangeTracker.Clear();

        // Empty result: carries the product on success or a typed error on failure
        var result = new Result<Product>();

        // Begin a transaction — the lock's lifetime is the transaction.
        // The lock starts when we execute the SELECT WITH (UPDLOCK, ROWLOCK)
        // and releases when we commit or rollback.
        using var transaction = await _context.Database
            .BeginTransactionAsync();

        // Everything in here runs inside the transaction
        try
        {
            // SET LOCK_TIMEOUT limits how long we wait for a locked row.
            // Without this, SQL Server sets no limit on the wait.
            // 3000 = 3 seconds. After that, SQL Server raises error 1222.
            await _context.Database.ExecuteSqlRawAsync("SET LOCK_TIMEOUT 3000");

            // Pessimistic lock: SELECT with UPDLOCK, ROWLOCK.
            // UPDLOCK = "I intend to update this row — block other update locks."
            // ROWLOCK = "Lock just this row, not the page or table."
            // The lock is held until CommitAsync or RollbackAsync.
            var product = await _context.Products
                .FromSqlRaw(
                    "SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}",
                    productId)
                .FirstOrDefaultAsync();

            // No such product: handled failure, nothing changed
            if (product is null)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("NotFound", $"Product {productId} does not exist."));
                // Return the failure to the caller; nothing is rethrown
                return result;
            }

            // DEMO ONLY: Simulates network/lock delay for demonstration. Remove in production code.
            // Held for 5s on purpose — LONGER than the 3s SET LOCK_TIMEOUT above. The concurrent
            // simulator relies on it: if this delay drops BELOW the timeout, the second request
            // simply waits its turn and succeeds, and the lock timeout this demo exists to show
            // never fires.
            await Task.Delay(5000);

            // Not enough stock: fail before any data changes
            if (product.StockQuantity < quantity)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("InsufficientStock",
                    $"Only {product.StockQuantity} units available for \"{product.Name}\", " +
                    $"but {quantity} were requested."));
                // Return the failure to the caller; nothing is rethrown
                return result;
            }

            // Deduct under the lock: no other UPDLOCK request can reach this row
            product.StockQuantity -= quantity;
            // Write the new stock level inside the transaction
            await _context.SaveChangesAsync();
            // Commit: the change becomes permanent and the lock is released
            await transaction.CommitAsync();

            // Success: return the updated product
            return result.WithValue(product);
        }
        // Teaching simplification: production keys on the stable SqlException.Number
        // (1222 = lock timeout, 1205 = deadlock), not message text (locale/version-dependent).
        catch (Exception ex) when (ex.Message.Contains("lock request time out", StringComparison.OrdinalIgnoreCase))
        {
            // Lock timeout (SQL Server error 1222) — another transaction holds the lock too long.
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Typed LockTimeout error for the caller (the page or the simulator)
            result.AddError(new Error("LockTimeout",
                "Could not acquire lock within the timeout period. " +
                "Another transaction is holding the row. Please try again."));
            // Return the failure to the caller; nothing is rethrown
            return result;
        }
        // Deadlock (SQL error 1205): two transactions waiting on each other
        catch (Exception ex) when (ex.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
        {
            // Deadlock (SQL Server error 1205) — two transactions are waiting on each other.
            // The database engine chooses one as the "victim" and kills it.
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Typed Deadlock error for the caller (the page or the simulator)
            result.AddError(new Error("Deadlock",
                "A deadlock was detected. Your transaction was chosen as the victim. " +
                "Please retry the operation."));
            // Return the failure to the caller; nothing is rethrown
            return result;
        }
        // Any other failure lands here
        catch (Exception ex)
        {
            // General failure — rollback and report.
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Typed ReservationFailed error for the caller (the page or the simulator)
            result.AddError(new Error("ReservationFailed",
                $"Failed to reserve stock: {ex.Message}"));
            // Return the failure to the caller; nothing is rethrown
            return result;
        }
    }
}
