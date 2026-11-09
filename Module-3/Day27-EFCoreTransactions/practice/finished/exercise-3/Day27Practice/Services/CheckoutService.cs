using Day27Practice.Data;
using Day27Practice.Models;
using BYSResults;
using Microsoft.EntityFrameworkCore;

namespace Day27Practice.Services;

public class CheckoutService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// Practice 2 SOLUTION: an explicit transaction around a multi-step checkout. The inventory
    /// deduction, the Order, and its OrderItem are all staged in the change tracker and written
    /// by ONE SaveChangesAsync; CommitAsync is what makes them permanent. The OrderItem reaches
    /// the Order through the navigation property, so EF Core fills in the generated OrderId
    /// during that single save — a generated ID is never a reason to save twice.
    /// </summary>
    public async Task<Result<string>> CheckoutAsync(int productId, int quantity, string customer)
    {
        // Open one transaction; await using disposes it (and rolls back) if we never commit
        await using var transaction = await _context.Database.BeginTransactionAsync();
        // Everything from here to CommitAsync is one all-or-nothing unit
        try
        {
            // Load the product inside the transaction
            var product = await _context.Products.FindAsync(productId);
            // Unknown product: nothing written yet, so just return
            if (product is null)
                return Result<string>.Failure(new Error("NotFound", "Product not found."));
            // Not enough stock: also nothing written yet
            if (product.StockQuantity < quantity)
                return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

            // Deduct inventory. Nothing is written yet — this is staged in the change tracker.
            product.StockQuantity -= quantity;

            // Create the order and its line item. The OrderItem reaches the Order through the
            // navigation property, so EF Core sets OrderId for us during the save below.
            var order = new Order
            {
                // Who bought it
                CustomerName = customer,
                // What they bought
                ProductId = productId,
                // How many
                Quantity = quantity,
                // Price at the time of sale
                Total = product.Price * quantity,
                // When the order was placed
                CreatedAt = DateTime.UtcNow
            };
            // Track the new order
            _context.Orders.Add(order);
            // Track its line item
            _context.OrderItems.Add(new OrderItem
            {
                Order = order,            // navigation property — EF Core sets OrderId on save
                // What they bought
                ProductId = productId,
                // Copy the name so the line item reads on its own
                ProductName = product.Name,
                // How many
                Quantity = quantity,
                // Price per unit at the time of sale
                UnitPrice = product.Price
            });

            // ONE save — the inventory change, the Order, and the OrderItem all go together.
            await _context.SaveChangesAsync();

            // Until this line runs, none of the above is permanent.
            await transaction.CommitAsync();
            // Success: the order id exists now that the insert has run
            return Result<string>.Success(
                $"Order #{order.Id} created for {customer}. Total: {order.Total:C}.");
        }
        // Any exception between Begin and Commit lands here
        catch (Exception ex)
        {
            // Undo everything written since BeginTransactionAsync
            await transaction.RollbackAsync();
            // Drop tracked changes so a retry starts clean
            _context.ChangeTracker.Clear();
            // Report the failure to the page instead of crashing it
            return Result<string>.Failure(new Error("Internal", $"Checkout failed: {ex.Message}"));
        }
    }

    /// <summary>
    /// Practice 3 SOLUTION: identical to CheckoutAsync, except an exception is thrown AFTER the
    /// SaveChangesAsync and BEFORE the CommitAsync. This is the whole point of the explicit
    /// boundary: the save really did write the inventory change, the Order, and the OrderItem to
    /// the database — but because the transaction is still open, none of it is permanent. The
    /// catch calls RollbackAsync and every one of those writes disappears. Without the explicit
    /// transaction, that same SaveChangesAsync would have committed on its own and the throw
    /// would have left the data behind.
    /// </summary>
    public async Task<Result<string>> FailingCheckoutAsync(int productId, int quantity, string customer)
    {
        // Open one transaction; await using disposes it (and rolls back) if we never commit
        await using var transaction = await _context.Database.BeginTransactionAsync();
        // Everything from here to CommitAsync is one all-or-nothing unit
        try
        {
            // Load the product inside the transaction
            var product = await _context.Products.FindAsync(productId);
            // Unknown product: nothing written yet, so just return
            if (product is null)
                return Result<string>.Failure(new Error("NotFound", "Product not found."));
            // Not enough stock: also nothing written yet
            if (product.StockQuantity < quantity)
                return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

            // Deduct inventory (staged in the change tracker)
            product.StockQuantity -= quantity;

            // Build the order
            var order = new Order
            {
                // Who bought it
                CustomerName = customer,
                // What they bought
                ProductId = productId,
                // How many
                Quantity = quantity,
                // Price at the time of sale
                Total = product.Price * quantity,
                // When the order was placed
                CreatedAt = DateTime.UtcNow
            };
            // Track the new order
            _context.Orders.Add(order);
            // Track its line item
            _context.OrderItems.Add(new OrderItem
            {
                // Navigation property: EF Core sets OrderId during the save
                Order = order,
                // What they bought
                ProductId = productId,
                // Copy the name so the line item reads on its own
                ProductName = product.Name,
                // How many
                Quantity = quantity,
                // Price per unit at the time of sale
                UnitPrice = product.Price
            });

            // The save writes all three changes to the database — but inside the open
            // transaction, so nothing is permanent yet.
            await _context.SaveChangesAsync();

            // SIMULATED FAILURE between the save and the commit. This stands in for anything
            // that can go wrong late in a real request: a deadlock, a dropped connection, a
            // timeout. CommitAsync below is never reached.
            throw new InvalidOperationException("Simulated failure after save, before commit!");

            // NEVER REACHED — shown for parity with CheckoutAsync.
            // await transaction.CommitAsync();
            // return Result<string>.Success($"Order #{order.Id} created for {customer}.");
        }
        // Any exception between Begin and Commit lands here
        catch (Exception ex)
        {
            // RollbackAsync erases the writes the save had already made. ChangeTracker.Clear()
            // then drops the still-tracked entities so the next request starts clean.
            await transaction.RollbackAsync();
            // Drop tracked changes so a retry starts clean
            _context.ChangeTracker.Clear();
            // Report the failure to the page instead of crashing it
            return Result<string>.Failure(new Error("Internal",
                $"Checkout failed: {ex.Message} Inventory reservation rolled back — stock is unchanged."));
        }
    }
}
