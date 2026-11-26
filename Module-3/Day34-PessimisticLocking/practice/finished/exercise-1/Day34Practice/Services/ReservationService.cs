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
    /// Practice 2: Pessimistic lock with timeout and detailed BYSResult error handling.
    /// Adds lock timeout configuration and three specific catch blocks for
    /// lock timeout, deadlock, and general failures.
    /// </summary>
    public async Task<Result<Product>> ReserveWithTimeoutAsync(int productId, int quantity)
    {
        // NOTE: message-text matching below is a teaching simplification — production keys on
        //       the stable SqlException.Number (1222 = lock timeout, 1205 = deadlock) instead.

        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }
}
