using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day29Practice.Data;
using Day29Practice.Models;

namespace Day29Practice.Services;

public class CheckoutService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] The bare-string version — the shape Result&lt;T&gt; replaces.
    /// A teaching foil written for today, NOT a copy of Day 27's code:
    /// Day 27's own checkout already returned Result&lt;string&gt;.
    /// The transaction boundary is already correct — the RETURN TYPE is what today fixes.
    /// Compare this with the Result-based version you write in Practice 1.
    /// </summary>
    public async Task<string> StringCheckoutAsync(int productId, int quantity, string customer)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product is null) return "Product not found";
            if (product.StockQuantity < quantity) return "Insufficient stock";

            product.StockQuantity -= quantity;

            var order = new Order
            {
                CustomerName = customer,
                ProductId = productId,
                Quantity = quantity,
                Total = product.Price * quantity,
                CreatedAt = DateTime.UtcNow
            };
            _context.Orders.Add(order);

            // Single save — inventory update + order creation batched
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return $"Order #{order.Id} created for {customer}";
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            return $"Database error during checkout: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    /// <summary>
    /// Practice 1: Wrap a single checkout in BYSResult, structured in three tiers.
    /// Replace string returns with a typed Result&lt;Order&gt;.
    /// </summary>
    public async Task<Result<Order>> ResultCheckoutAsync(
        int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }

    /// <summary>
    /// Practice 2: Multi-step checkout with error accumulation.
    /// Same three tiers, now across N cart items — report every bad item at once.
    /// </summary>
    public async Task<Result<List<Order>>> MultiStepCheckoutAsync(
        List<(int ProductId, int Quantity)> items, string customer)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }
}
