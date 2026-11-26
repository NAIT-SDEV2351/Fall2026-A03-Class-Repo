using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day34Practice.Data;
using Day34Practice.Models;

namespace Day34Practice.Services;

public class ReservationService(StoreDbContext context)
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
    /// [COMPLETED] Practice 1: Basic pessimistic lock for stock reservation.
    /// Uses SQL Server lock hints (UPDLOCK, ROWLOCK) to lock the row during read-modify-write.
    /// </summary>
    public async Task<Result<Product>> ReserveAsync(int productId, int quantity)
    {
        // Forget rows this context already tracks: the locked read would return their stale values
        _context.ChangeTracker.Clear();

        var result = new Result<Product>();

        // Begin a transaction — the lock's lifetime is the transaction.
        using var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            // Pessimistic lock: SELECT with UPDLOCK, ROWLOCK.
            // UPDLOCK = "I intend to update — block other update locks on this row."
            // ROWLOCK = "Lock just this row, not the page or table."
            var product = await _context.Products
                .FromSqlRaw(
                    "SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}",
                    productId)
                .FirstOrDefaultAsync();

            if (product is null)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("NotFound", $"Product {productId} does not exist."));
                return result;
            }

            if (product.StockQuantity < quantity)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("InsufficientStock",
                    $"Only {product.StockQuantity} units available for \"{product.Name}\", " +
                    $"but {quantity} were requested."));
                return result;
            }

            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return result.WithValue(product);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("ReservationFailed",
                $"Failed to reserve stock: {ex.Message}"));
            return result;
        }
    }

    /// <summary>
    /// [COMPLETED] Practice 2: Pessimistic lock with timeout and BYSResult error handling.
    /// Adds lock timeout configuration and specific error handling for
    /// lock timeout, deadlock, and general failures.
    /// </summary>
    public async Task<Result<Product>> ReserveWithTimeoutAsync(int productId, int quantity)
    {
        var result = new Result<Product>();

        using var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            // Set lock timeout — limits how long we wait for a locked row.
            // 3000 = 3 seconds. Without this, SQL Server sets no limit on the wait.
            // After timeout, SQL Server raises error 1222.
            await _context.Database.ExecuteSqlRawAsync("SET LOCK_TIMEOUT 3000");

            // Pessimistic lock: SELECT with UPDLOCK, ROWLOCK.
            var product = await _context.Products
                .FromSqlRaw(
                    "SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}",
                    productId)
                .FirstOrDefaultAsync();

            if (product is null)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("NotFound", $"Product {productId} does not exist."));
                return result;
            }

            // DEMO ONLY: Simulates network/lock delay for demonstration. Remove in production code.
            // Held for 5s on purpose — LONGER than the 3s LOCK_TIMEOUT above, so a second
            // concurrent request gives up waiting and takes the LockTimeout path below.
            // Drop this under 3s and the other request simply waits, succeeds, and Practice 2
            // demonstrates nothing (see instructor-supplement "Verify: Request A sleeps 5s").
            await Task.Delay(5000);

            if (product.StockQuantity < quantity)
            {
                // Implicit rollback: returning without CommitAsync causes the using block to
                // dispose the transaction uncommitted — equivalent to explicit RollbackAsync().
                result.AddError(new Error("InsufficientStock",
                    $"Only {product.StockQuantity} units available for \"{product.Name}\", " +
                    $"but {quantity} were requested."));
                return result;
            }

            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return result.WithValue(product);
        }
        // Teaching simplification: production keys on the stable SqlException.Number
        // (1222 = lock timeout, 1205 = deadlock), not message text (locale/version-dependent).
        catch (Exception ex) when (ex.Message.Contains("lock request time out", StringComparison.OrdinalIgnoreCase))
        {
            // Lock timeout (SQL Server error 1222) — another transaction holds the lock too long.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("LockTimeout",
                "Could not acquire lock within the timeout period. " +
                "Another transaction is holding the row. Please try again."));
            return result;
        }
        catch (Exception ex) when (ex.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
        {
            // Deadlock (SQL Server error 1205) — two transactions are waiting on each other.
            // The database engine chooses one as the "victim" and kills it.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Deadlock",
                "A deadlock was detected. Your transaction was chosen as the victim. " +
                "Please retry the operation."));
            return result;
        }
        catch (Exception ex)
        {
            // General failure — rollback and report.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("ReservationFailed",
                $"Failed to reserve stock: {ex.Message}"));
            return result;
        }
    }
}
