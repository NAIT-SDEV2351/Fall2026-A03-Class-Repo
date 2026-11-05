using Microsoft.EntityFrameworkCore;
using Day26Demo.Data;
using Day26Demo.Models;
using BYSResults;

namespace Day26Demo.Services;

public class CheckoutService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// Demonstrates what goes wrong WITHOUT an explicit transaction.
    /// Step 1 commits (inventory deducted), then an uncontrolled failure occurs,
    /// and Step 2 (order creation) never runs. The data is left inconsistent.
    /// The success path lives in SafeCheckoutAsync — here Step 2 is never reached.
    /// </summary>
    public async Task<Result<string>> BrokenCheckoutAsync(int productId, int quantity, string customer)
    {
        // Load the product by its key (null if no such id)
        var product = await _context.Products.FindAsync(productId);
        // No such product: handled failure, nothing written
        if (product is null)
            return Result<string>.Failure(new Error("NotFound", "Product not found."));
        // Not enough stock: fail before any data changes
        if (product.StockQuantity < quantity)
            return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

        // Step 1: Deduct inventory — this SaveChangesAsync commits independently.
        product.StockQuantity -= quantity;
        // Saves Step 1 now: its own implicit transaction commits it
        await _context.SaveChangesAsync();

        // SIMULATED CRASH between steps (network timeout, server restart, deadlock).
        // This is NOT a handled error — it is the uncontrolled interruption the demo
        // exists to show, so it stays a throw. A validation failure returns a Result;
        // a crash cannot. Step 1 already committed; Step 2 below never runs.
        throw new InvalidOperationException(
            "Simulated failure! Inventory was deducted but the order was never created. " +
            "This is an Atomicity violation — check the Inventory page to see the damage.");

        // Step 2: Create order — NEVER REACHED (see SafeCheckoutAsync for the success path)
        // var order = new Order { CustomerName = customer, ProductId = productId,
        //     Quantity = quantity, Total = product.Price * quantity, CreatedAt = DateTime.UtcNow };
        // _context.Orders.Add(order);
        // await _context.SaveChangesAsync();
        // return Result<string>.Success($"Order #{order.Id} created for {customer}.");
    }

    /// <summary>
    /// PREVIEW of the explicit-transaction pattern. NOTE: this checkout batches the
    /// inventory update and the order insert into a SINGLE SaveChangesAsync, which is
    /// already atomic on its own — the explicit transaction wrapper is not strictly
    /// required here. We use it to rehearse the shape needed on Day-27, where a real
    /// payment-gateway call sits BETWEEN two saves and the wrapper becomes necessary.
    /// </summary>
    public async Task<Result<string>> SafeCheckoutAsync(int productId, int quantity, string customer)
    {
        // Open an explicit transaction; it rolls back if never committed
        await using var transaction = await _context.Database.BeginTransactionAsync();
        // Everything in here runs inside the transaction
        try
        {
            // Load the product by its key (null if no such id)
            var product = await _context.Products.FindAsync(productId);
            // No such product: handled failure, nothing written
            if (product is null)
                return Result<string>.Failure(new Error("NotFound", "Product not found."));
            // Not enough stock: fail before any data changes
            if (product.StockQuantity < quantity)
                return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

            // Step 1: Deduct inventory
            product.StockQuantity -= quantity;

            // Step 2: Create order (flushed in the same single save as Step 1)
            var order = new Order
            {
                CustomerName = customer,
                ProductId = productId,
                Quantity = quantity,
                Total = product.Price * quantity,
                CreatedAt = DateTime.UtcNow
            };
            // Track the new order; nothing is sent until the save
            _context.Orders.Add(order);

            // ONE save — both writes are sent together and are already atomic.
            await _context.SaveChangesAsync();

            // Commit: both writes become permanent together
            await transaction.CommitAsync();
            // Success: report the new order
            return Result<string>.Success($"Order #{order.Id} created for {customer}. Total: {order.Total:C}");
        }
        // Any failure inside try lands here
        catch (Exception ex)
        {
            // Any failure rolls back ALL changes and returns a handled result — no rethrow.
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Handled failure: the page shows the message, nothing rethrown
            return Result<string>.Failure(new Error("Internal", $"Checkout failed: {ex.Message}"));
        }
    }

    /// <summary>
    /// Resets the database to its original seeded state. Useful for re-running demos.
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
