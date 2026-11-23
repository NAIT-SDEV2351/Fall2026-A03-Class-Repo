using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day32Practice.Data;
using Day32Practice.Models;

namespace Day32Practice.Services;

public class ProductService(StoreDbContext context, IDbContextFactory<StoreDbContext> factory)
{
    private readonly StoreDbContext _context = context;
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    /// <summary>
    /// [PRE-BUILT] Get all products.
    /// </summary>
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// Practice 2: Simulate a concurrency conflict.
    /// Two users read the same product. User A saves first. User B gets an exception.
    /// </summary>
    public async Task<Result<string>> SimulateConflictAsync(int productId)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }

    /// <summary>
    /// Optional take-home (Practice 4, Day 34): Update product with full conflict resolution.
    /// Detect conflict, read database values, apply resolution strategy, retry if needed.
    /// </summary>
    public async Task<Result<Product>> UpdateWithResolutionAsync(
        int productId, decimal newPrice, string strategy)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }
}
