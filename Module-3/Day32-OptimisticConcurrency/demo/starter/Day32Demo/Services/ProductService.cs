using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day32Demo.Data;
using Day32Demo.Models;

namespace Day32Demo.Services;

public class ProductService(StoreDbContext context, IDbContextFactory<StoreDbContext> factory)
{
    private readonly StoreDbContext _context = context;
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    /// <summary>
    /// [PRE-BUILT] Get all products for display.
    /// </summary>
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// [CODE LIVE] Update product with conflict resolution.
    /// Instructor implements during Exploring demo.
    /// </summary>
    public async Task<Result<Product>> UpdateWithConflictResolutionAsync(
        int productId, decimal newPrice, string strategy)
    {
        throw new NotImplementedException("Instructor will implement during demo");
    }
}
