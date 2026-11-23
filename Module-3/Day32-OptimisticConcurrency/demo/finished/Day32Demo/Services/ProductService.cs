using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day32Demo.Data;
using Day32Demo.Models;

namespace Day32Demo.Services;

public class ProductService(StoreDbContext context, IDbContextFactory<StoreDbContext> factory)
{
    private readonly StoreDbContext _context = context;
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// [COMPLETED] Update a product's price, and when a concurrency conflict is detected apply one
    /// of three classic resolution strategies. The happy path is an ordinary save; the teaching is
    /// entirely in the catch block, which only runs when RowVersion no longer matches the DB.
    /// </summary>
    public async Task<Result<Product>> UpdateWithConflictResolutionAsync(
        int productId, decimal newPrice, string strategy)
    {
        // Empty result: filled with a value or errors below
        var result = new Result<Product>();
        // Load the product by its key (null if no such id)
        var product = await _context.Products.FindAsync(productId);
        // No such product: handled failure, nothing written
        if (product is null)
        {
            // Report the missing product as a typed error
            result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
            // Hand the failed result back to the caller
            return result;
        }

        // Change the tracked entity; nothing is sent yet
        product.Price = newPrice;

        // The normal path: an ordinary save
        try
        {
            // UPDATE ... WHERE Id = @id AND RowVersion = @original
            await _context.SaveChangesAsync();
            // Success: return the saved product
            return result.WithValue(product);
        }
        // The WHERE matched zero rows: someone saved first
        catch (DbUpdateConcurrencyException ex)
        {
            // Conflict detected. ex.Entries is the entity that lost the race (just one here).
            // GetDatabaseValuesAsync re-reads the row as it is NOW — the winning user's values —
            // which every strategy below needs in order to decide what to do.
            var entry = ex.Entries.Single();
            // Re-read the row as it is NOW (null if deleted)
            var databaseValues = await entry.GetDatabaseValuesAsync();

            // Null: the other user deleted the row
            if (databaseValues is null)
            {
                // Null means the row is gone entirely — the other user DELETED it, not just edited it.
                _context.ChangeTracker.Clear();
                // Report the deletion as a typed error
                result.AddError(new Error("Deleted",
                    "Product was deleted by another user"));
                // Hand the failed result back to the caller
                return result;
            }

            // Database wins: keep their values, discard ours
            if (strategy == "database-wins")
            {
                // Discard the client's edit. Overwrite both Original and Current with the DB values
                // so our in-memory entity matches the database and there is nothing left to conflict.
                entry.OriginalValues.SetValues(databaseValues);
                // Overwrite our edit with the database values
                entry.CurrentValues.SetValues(databaseValues);
                // Drop the stale entity from the change tracker
                _context.ChangeTracker.Clear();
                // Typed copy of the database row, for the message
                var dbProduct = (Product)databaseValues.ToObject();
                // Report the conflict as a typed error
                result.AddError(new Error("Conflict",
                    $"Kept database values. Current price: {dbProduct.Price:C}"));
                // Hand the failed result back to the caller
                return result;
            }

            // Client wins: force our edit through
            if (strategy == "client-wins")
            {
                // Force the client's edit through. Copy ONLY the OriginalValues to the current DB
                // version — this re-points the WHERE clause at the now-current RowVersion so the
                // second save matches one row. CurrentValues still holds the client's new price.
                entry.OriginalValues.SetValues(databaseValues);
                // NOTE: Single retry only — if a second concurrent update occurs during the retry,
                // this will throw. Production code should use a retry loop with a max attempt limit.
                await _context.SaveChangesAsync();
                // Success: return the saved product
                return result.WithValue(product);
            }

            // Default = "ask the user": surface BOTH values side by side and let a human decide,
            // rather than silently picking a winner. No save happens — the caller re-invokes with
            // an explicit database-wins / client-wins strategy.
            _context.ChangeTracker.Clear();
            // Typed copy of the database row, to show beside ours
            var currentDb = (Product)databaseValues.ToObject();
            // Report the conflict as a typed error
            result.AddError(new Error("Conflict",
                $"DB price: {currentDb.Price:C}, your price: {newPrice:C}. Choose a resolution strategy."));
            // Hand the failed result back to the caller
            return result;
        }
    }
}
