using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day29Demo.Data;
using Day29Demo.Models;

namespace Day29Demo.Services;

public class CheckoutService(StoreDbContext context) : ICheckoutService
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] The bare-string version — the shape Result&lt;T&gt; replaces.
    /// A teaching foil written for today, NOT a copy of Day 27's code:
    /// Day 27's own checkout already returned Result&lt;string&gt;.
    /// The transaction boundary is already correct — the RETURN TYPE is what today fixes.
    /// Kept here for comparison with the Result-based version below.
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
    /// [CODE LIVE] Result-wrapped checkout using BYSResult, structured in three tiers.
    /// Tier 1: INPUT validation — answerable from the arguments alone (accumulate errors).
    /// Tier 2: STATE pre-checks — only the database knows (existence, stock).
    /// Tier 3: the WRITES — plus the catch, which Tier 2 does NOT make unreachable.
    /// </summary>
    public async Task<Result<Order>> ResultCheckoutAsync(
        int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Instructor will implement during demo");
    }

    /// <summary>
    /// [PRE-BUILT] FOIL for Tier 2 — an UPDATE with NO existence pre-check.
    ///
    /// We never load the product; we attach a stub carrying the caller's id and tell EF Core
    /// "a row with this key exists, change its Price." EF emits
    /// UPDATE Products SET Price = @p WHERE Id = @id. If that id does not exist the statement
    /// affects ZERO rows and EF raises DbUpdateConcurrencyException — at SaveChangesAsync,
    /// not at any line above it. Read the message a user would get from a bad id: technically
    /// true, completely useless. ResultUpdatePriceAsync is the fix.
    /// </summary>
    public async Task<Result<Product>> UncheckedUpdatePriceAsync(int productId, decimal newPrice)
    {
        var result = new Result<Product>();

        if (newPrice <= 0)
            result.AddError(new Error("Validation", "Price must be positive"));
        if (result.IsFailure)
            return result;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // NO Tier-2 check. We assert the row exists instead of asking.
            var stub = new Product { Id = productId, Name = "(unchanged)", Price = newPrice };
            _context.Products.Attach(stub);
            _context.Entry(stub).Property(p => p.Price).IsModified = true;

            await _context.SaveChangesAsync(); // <-- the failure surfaces HERE
            await transaction.CommitAsync();
            return result.WithValue(stub);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database", ex.Message));
            return result;
        }
    }

    /// <summary>
    /// [PRE-BUILT] The same update done properly — the three tiers applied to a modification.
    /// Tier 2 turns an opaque "0 rows affected" into a clean Error("Not Found", ...), and
    /// Tier 3 keeps its catch anyway, because the row can vanish between the check and the save.
    /// Compare the two with productId = 9999.
    /// </summary>
    public async Task<Result<Product>> ResultUpdatePriceAsync(int productId, decimal newPrice)
    {
        var result = new Result<Product>();

        // Tier 1: INPUT — answerable without the database
        if (newPrice <= 0)
            result.AddError(new Error("Validation", "Price must be positive"));
        else if (newPrice > 100_000m)
            result.AddError(new Error("Validation", "Price cannot exceed $100,000"));
        if (result.IsFailure)
            return result;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Tier 2: STATE — the check the foil above skipped
            var product = await _context.Products.FindAsync(productId);
            if (product is null)
            {
                result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
                await transaction.RollbackAsync();
                return result;
            }

            // Tier 3: the WRITE
            product.Price = newPrice;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return result.WithValue(product);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Still reachable despite Tier 2 — someone deleted the row in between.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database",
                $"Product {productId} was modified or removed by another user. {ex.Message}"));
            return result;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database",
                ex.InnerException?.Message ?? ex.Message));
            return result;
        }
        catch (Exception ex)
        {
            // Same trailing catch as ResultCheckoutAsync — every outcome comes back
            // as typed data, including the ones we did not anticipate.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Internal", $"Price update failed: {ex.Message}"));
            return result;
        }
    }
}
