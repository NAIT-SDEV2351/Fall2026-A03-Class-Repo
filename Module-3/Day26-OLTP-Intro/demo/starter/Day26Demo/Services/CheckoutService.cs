using Microsoft.EntityFrameworkCore;
using Day26Demo.Data;
using Day26Demo.Models;
using BYSResults;

namespace Day26Demo.Services;

public class CheckoutService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    public async Task<Result<string>> BrokenCheckoutAsync(int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Implement during demo");
    }

    public async Task<Result<string>> SafeCheckoutAsync(int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Implement during demo");
    }

    /// <summary>
    /// [PRE-BUILT] Resets the database to its original seeded state.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        _context.Orders.RemoveRange(_context.Orders);

        var products = await _context.Products.ToListAsync();
        var seedData = new Dictionary<int, int>
        {
            { 1, 10 }, { 2, 50 }, { 3, 30 }, { 4, 15 }, { 5, 25 }
        };
        foreach (var product in products)
        {
            if (seedData.TryGetValue(product.Id, out int originalStock))
                product.StockQuantity = originalStock;
        }
        await _context.SaveChangesAsync();
    }
}
