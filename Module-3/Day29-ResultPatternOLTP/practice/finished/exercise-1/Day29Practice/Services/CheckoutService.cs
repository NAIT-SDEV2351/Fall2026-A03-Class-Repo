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
    /// Practice 1 SOLUTION: Result-wrapped single checkout, structured in three tiers.
    /// Tier 1 = INPUT (arguments only) · Tier 2 = STATE (only the database knows)
    /// · Tier 3 = the WRITES (plus a catch Tier 2 does not make unreachable).
    /// </summary>
    public async Task<Result<Order>> ResultCheckoutAsync(
        int productId, int quantity, string customer)
    {
        var result = new Result<Order>();

        // ── Tier 1: INPUT validation — no database needed ────────────────────────
        if (string.IsNullOrWhiteSpace(customer))
            result.AddError(new Error("Validation", "Customer name is required"));
        else if (customer.Length > 200)
            result.AddError(new Error("Validation", "Customer name cannot exceed 200 characters"));
        if (quantity <= 0)
            result.AddError(new Error("Validation", "Quantity must be positive"));
        if (result.IsFailure)
            return result; // Fast exit — nothing opened, nothing to roll back

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // ── Tier 2: STATE pre-checks — only the database can answer these ────
            var product = await _context.Products.FindAsync(productId);
            if (product is null)
            {
                result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
                await transaction.RollbackAsync();
                return result;
            }
            if (product.StockQuantity < quantity)
            {
                result.AddError(new Error("Stock",
                    $"Product '{product.Name}': need {quantity}, have {product.StockQuantity}"));
                await transaction.RollbackAsync();
                return result;
            }

            // ── Tier 3: the WRITES ───────────────────────────────────────────────
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
            return result.WithValue(order);
        }
        catch (DbUpdateException ex)
        {
            // Tier 2 does NOT make this unreachable: between FindAsync saying the
            // product exists and SaveChangesAsync running, another transaction can
            // delete it. The pre-check buys a clean MESSAGE; the catch buys CORRECTNESS.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database",
                ex.InnerException?.Message ?? ex.Message));
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Internal", $"Checkout failed: {ex.Message}"));
            return result;
        }
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
