using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day30Demo.Data;
using Day30Demo.Models;

namespace Day30Demo.Services;

public class OrderService(StoreDbContext context) : IOrderService
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] The READ side of the graph.
    /// Include reaches from the parent to its children; ThenInclude reaches one level
    /// further, from each child to ITS product. Order -> Items -> Product.
    /// AsNoTracking because this copy is only ever displayed, never saved.
    /// </summary>
    public async Task<Result<OrderView>> GetOrderAsync(int orderId)
    {
        var result = new Result<OrderView>();

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            result.AddError(new Error("Not Found", $"Order {orderId} does not exist"));
            return result;
        }

        return result.WithValue(ToView(order));
    }

    /// <summary>
    /// [CODE LIVE] ONE method that both ADDS a new order and EDITS an existing one,
    /// parent and children saved together in a single round trip.
    /// </summary>
    public async Task<Result<OrderView>> AddEditOrderAsync(OrderView view)
    {
        // Collect every error first; nothing has touched the database yet
        var result = new Result<OrderView>();

        // ── Tier 1: INPUT — answerable from the view alone, no database ──────────────
        // A blank customer name is an input error
        if (string.IsNullOrWhiteSpace(view.CustomerName))
            result.AddError(new Error("Validation", "Customer name is required"));
        // Over 200 characters breaks a Tier-1 business rule
        else if (view.CustomerName.Length > 200)
            result.AddError(new Error("Validation", "Customer name cannot exceed 200 characters"));

        // A line the user removed is not a line the user has to fix.
        var activeLines = view.Lines.Where(l => !l.IsRemoved).ToList();

        // No active lines: nothing left to order
        if (activeLines.Count == 0)
            result.AddError(new Error("Validation", "An order needs at least one line"));

        // Report problems by the line number the user can SEE on screen. Naming the
        // product would be useless on the one line that has no product chosen yet.
        for (var i = 0; i < view.Lines.Count; i++)
        {
            // The line at that on-screen position
            var line = view.Lines[i];
            // Removed lines need no checks
            if (line.IsRemoved) continue;

            // No product chosen on this line yet
            if (line.ProductId <= 0)
                result.AddError(new Error("Validation", $"Line {i + 1}: choose a product"));
            // Quantity must be at least one
            if (line.Quantity < 1)
                result.AddError(new Error("Validation",
                    $"Line {i + 1}: quantity must be at least 1"));
            // A price may be zero, never negative
            if (line.UnitPrice < 0)
                result.AddError(new Error("Validation",
                    $"Line {i + 1}: price cannot be negative"));
        }

        // Product ids that sit on more than one active line
        var duplicates = activeLines
            .Where(l => l.ProductId > 0)
            .GroupBy(l => l.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        // One error per duplicated product, named by its id
        foreach (var productId in duplicates)
            result.AddError(new Error("Validation",
                $"Product {productId} appears on more than one line"));

        // Any input error: stop before the database
        if (result.IsFailure)
            return result; // Fast exit — nothing loaded, nothing saved

        // ── Load or create the PARENT, children included ─────────────────────────────
        // Id == 0 means "add". Any other Id means the caller is editing something they
        // believe exists — and if it does not, that is a failure, not a new order.
        var order = view.Id == 0
            ? new Order { OrderDate = DateTime.UtcNow }
            : await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == view.Id);

        // Editing an order that does not exist is a failure, never a new order
        if (order is null)
        {
            result.AddError(new Error("Not Found", $"Order {view.Id} does not exist"));
            return result;
        }

        // ── Parent scalars ───────────────────────────────────────────────────────────
        // Copy the customer name across, trimmed
        order.CustomerName = view.CustomerName.Trim();

        // ── Children: update what matches, add what does not, flag what was removed ──
        // Every line on screen, removed ones included
        foreach (var line in view.Lines)
        {
            // Id == 0 means the database has never seen this line.
            var item = line.Id == 0
                ? null
                : order.Items.FirstOrDefault(i => i.Id == line.Id);

            // No matching tracked row: treat the line as new
            if (item is null)
            {
                // Added and removed within the same edit — nothing to save.
                if (line.IsRemoved) continue;

                // Adding to the tracked parent's collection is ALL EF Core needs.
                // It sets OrderId itself at save time.
                order.Items.Add(new OrderItem
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    IsRemoved = false
                });
            }
            // Matched an existing row: update it in place
            else
            {
                // Copy the product choice onto the tracked row
                item.ProductId = line.ProductId;
                // Copy the quantity
                item.Quantity = line.Quantity;
                // Copy the unit price
                item.UnitPrice = line.UnitPrice;
                item.IsRemoved = line.IsRemoved; // soft delete — the row stays
            }
        }

        // ── Recompute the parent FROM its children ───────────────────────────────────
        // This is the payoff: Total is derived from the lines. Save the two separately
        // and there is a moment where the order says one thing and its lines say another.
        order.Total = order.Items
            .Where(i => !i.IsRemoved)
            .Sum(i => i.Quantity * i.UnitPrice);

        // A NEW parent has to be introduced to the context. An EXISTING one does not
        // need Update() — it has been tracked since the Include load above.
        if (order.Id == 0)
            _context.Orders.Add(order);

        // A failed save becomes an error result, not an exception
        try
        {
            // ONE save. The parent insert/update and every child insert/update travel
            // together in a single implicit transaction — Day 27's lesson. No
            // BeginTransactionAsync is needed here: one save IS one transaction.
            await _context.SaveChangesAsync();
        }
        // The database refused the save
        catch (DbUpdateException ex)
        {
            // Discard the pending tracked changes so none leak into a later save
            _context.ChangeTracker.Clear();
            // Report the database's own message
            result.AddError(new Error("Database", ex.InnerException?.Message ?? ex.Message));
            return result;
        }
        // Anything else that failed while saving
        catch (Exception ex)
        {
            // Discard the pending tracked changes so none leak into a later save
            _context.ChangeTracker.Clear();
            // Report it as a handled failure, never rethrown
            result.AddError(new Error("Internal", $"Saving the order failed: {ex.Message}"));
            return result;
        }

        // Re-read by the order's REAL id so the caller gets database truth back —
        // including the identity values the database just assigned to new rows.
        return await GetOrderAsync(order.Id);
    }

    /// <summary>[PRE-BUILT] Products available to put on a line.</summary>
    public async Task<List<Product>> GetProductsAsync() =>
        await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

    /// <summary>[PRE-BUILT] Flattens a tracked entity graph into the disconnected view.</summary>
    private static OrderView ToView(Order order) => new()
    {
        Id = order.Id,
        CustomerName = order.CustomerName,
        OrderDate = order.OrderDate,
        Total = order.Total,
        Lines = order.Items.Select(i => new OrderLineView
        {
            Id = i.Id,
            ProductId = i.ProductId,
            Description = i.Product?.Name ?? $"Product {i.ProductId}",
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            IsRemoved = i.IsRemoved
        }).ToList()
    };
}
