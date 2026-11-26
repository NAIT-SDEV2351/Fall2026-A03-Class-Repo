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
    /// [CODE LIVE] Reserve stock using pessimistic locking.
    /// Instructor implements during Exploring demo.
    /// </summary>
    /// <remarks>
    /// This demo uses SQL Server LocalDB, which supports real row-level locking
    /// with UPDLOCK, ROWLOCK hints. Students will see actual blocking when two
    /// transactions compete for the same row.
    /// </remarks>
    public async Task<Result<Product>> ReserveStockAsync(int productId, int quantity)
    {
        // Forget rows this context already tracks: the locked read would return their stale values
        _context.ChangeTracker.Clear();

        throw new NotImplementedException("Instructor will implement during demo");
    }
}
