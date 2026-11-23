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
    /// Practice 2 [COMPLETED]: Simulate a concurrency conflict.
    /// </summary>
    public async Task<Result<string>> SimulateConflictAsync(int productId)
    {
        var result = new Result<string>();

        // User A reads
        var productA = await _context.Products.FindAsync(productId);
        if (productA is null)
        {
            result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
            return result;
        }

        // User B reads (separate context)
        using var contextB = _factory.CreateDbContext();
        var productB = await contextB.Products.FindAsync(productId);

        // User A saves first
        productA.Price = 1099.99m;
        await _context.SaveChangesAsync();

        // User B tries to save — conflict!
        productB!.Price = 899.99m;
        try
        {
            await contextB.SaveChangesAsync();
            return result.WithValue("No conflict detected (unexpected)");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var dbValues = await entry.GetDatabaseValuesAsync();
            var dbProduct = (Product)dbValues!.ToObject();

            return result.WithValue(
                $"Conflict detected! User A saved: {dbProduct.Price:C}. " +
                $"User B wanted: {productB.Price:C}. " +
                $"Exception: {ex.Message}");
        }
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
