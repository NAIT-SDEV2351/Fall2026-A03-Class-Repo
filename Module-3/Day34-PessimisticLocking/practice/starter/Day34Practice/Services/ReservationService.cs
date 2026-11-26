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
    /// Practice 1: Basic pessimistic lock for stock reservation.
    /// Use SQL Server lock hints (UPDLOCK, ROWLOCK) to lock the row during read-modify-write.
    /// </summary>
    public async Task<Result<Product>> ReserveAsync(int productId, int quantity)
    {
        // Forget rows this context already tracks: the locked read would return their stale values
        _context.ChangeTracker.Clear();

        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
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
