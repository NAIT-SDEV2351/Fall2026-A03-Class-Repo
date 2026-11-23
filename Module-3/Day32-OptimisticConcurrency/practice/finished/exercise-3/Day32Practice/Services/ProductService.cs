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
    /// Optional take-home (Practice 4, Day 34) [COMPLETED]: Full conflict resolution.
    /// </summary>
    public async Task<Result<Product>> UpdateWithResolutionAsync(
        int productId, decimal newPrice, string strategy)
    {
        var result = new Result<Product>();
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
        {
            result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
            return result;
        }

        product.Price = newPrice;

        try
        {
            await _context.SaveChangesAsync();
            return result.WithValue(product);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Our RowVersion no longer matches the row. Re-read the DB to see the winner's values.
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues is null)
            {
                // Null = the other user DELETED the row, not just edited it.
                _context.ChangeTracker.Clear();
                result.AddError(new Error("Deleted",
                    "Product was deleted by another user"));
                return result;
            }

            if (strategy == "database-wins")
            {
                // Discard our edit: sync both Original and Current to the DB so nothing conflicts.
                entry.OriginalValues.SetValues(databaseValues);
                entry.CurrentValues.SetValues(databaseValues);
                _context.ChangeTracker.Clear();
                var dbProduct = (Product)databaseValues.ToObject();
                result.AddError(new Error("Conflict",
                    $"Kept database values. Current price: {dbProduct.Price:C}"));
                return result;
            }

            if (strategy == "client-wins")
            {
                // Force our edit through: copy ONLY Original to the current RowVersion (so the WHERE
                // clause matches again); CurrentValues keeps our new price. Then save once more.
                entry.OriginalValues.SetValues(databaseValues);
                // NOTE: Single retry only — if a second concurrent update occurs during the retry,
                // this will throw. Production code should use a retry loop with a max attempt limit.
                await _context.SaveChangesAsync();
                return result.WithValue(product);
            }

            _context.ChangeTracker.Clear();
            var currentDb = (Product)databaseValues.ToObject();
            result.AddError(new Error("Conflict",
                $"DB price: {currentDb.Price:C}, your price: {newPrice:C}. " +
                "Choose a resolution strategy."));
            return result;
        }
    }
}
