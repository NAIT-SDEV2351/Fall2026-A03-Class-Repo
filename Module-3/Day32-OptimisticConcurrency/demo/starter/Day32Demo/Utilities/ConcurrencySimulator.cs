using Microsoft.EntityFrameworkCore;
using Day32Demo.Data;

namespace Day32Demo.Utilities;

/// <summary>
/// [PRE-BUILT] Simulates two concurrent users editing the same product.
/// Uses IDbContextFactory to create separate DbContext instances,
/// each with its own change tracker — just like two real HTTP requests.
/// </summary>
public class ConcurrencySimulator(IDbContextFactory<StoreDbContext> factory)
{
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    public async Task<List<string>> SimulateConflictAsync(int productId)
    {
        var log = new List<string>();

        // User A reads
        using var contextA = await _factory.CreateDbContextAsync();
        var productA = await contextA.Products.FindAsync(productId);
        if (productA is null)
        {
            log.Add($"Product {productId} not found");
            return log;
        }
        log.Add($"[User A] Read Product #{productA.Id}: \"{productA.Name}\" — " +
                $"Price: {productA.Price:C}, RowVersion: {productA.RowVersion}");

        // User B reads (separate context — separate change tracker)
        using var contextB = await _factory.CreateDbContextAsync();
        var productB = await contextB.Products.FindAsync(productId);
        log.Add($"[User B] Read Product #{productB!.Id}: \"{productB.Name}\" — " +
                $"Price: {productB.Price:C}, RowVersion: {productB.RowVersion}");

        // User A changes price and saves first
        productA.Price = 1099.99m;
        log.Add($"[User A] Changed price to {productA.Price:C}");
        await contextA.SaveChangesAsync();
        log.Add($"[User A] SaveChangesAsync → SUCCESS (RowVersion: {productA.RowVersion})");

        // User B changes price and tries to save — conflict!
        productB.Price = 899.99m;
        log.Add($"[User B] Changed price to {productB.Price:C}");
        try
        {
            await contextB.SaveChangesAsync();
            log.Add("[User B] SaveChangesAsync → SUCCESS (unexpected — no conflict)");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            log.Add($"[User B] SaveChangesAsync → DbUpdateConcurrencyException!");
            log.Add($"  \"{ex.Message}\"");
        }

        return log;
    }
}
