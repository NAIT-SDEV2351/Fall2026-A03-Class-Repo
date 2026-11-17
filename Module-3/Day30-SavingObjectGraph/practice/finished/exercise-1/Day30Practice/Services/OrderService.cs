using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day30Practice.Data;
using Day30Practice.Models;

namespace Day30Practice.Services;

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
    /// Practice 1 [COMPLETED]: the ADD path — a new parent and its new children saved
    /// together in ONE round trip. Practice 2 extends this same method to the EDIT path.
    /// </summary>
    public async Task<Result<OrderView>> AddEditOrderAsync(OrderView view)
    {
        var result = new Result<OrderView>();

        // ── Tier 1: INPUT — answerable from the view alone, no database ──────────────
        if (string.IsNullOrWhiteSpace(view.CustomerName))
            result.AddError(new Error("Validation", "Customer name is required"));
        else if (view.CustomerName.Length > 200)
            result.AddError(new Error("Validation", "Customer name cannot exceed 200 characters"));

        // A line the user removed is not a line the user has to fix.
        var activeLines = view.Lines.Where(l => !l.IsRemoved).ToList();

        if (activeLines.Count == 0)
            result.AddError(new Error("Validation", "An order needs at least one line"));

        // Report problems by the line number the user can SEE on screen. Naming the
        // product would be useless on the one line that has no product chosen yet.
        for (var i = 0; i < view.Lines.Count; i++)
        {
            var line = view.Lines[i];
            if (line.IsRemoved) continue;

            if (line.ProductId <= 0)
                result.AddError(new Error("Validation", $"Line {i + 1}: choose a product"));
            if (line.Quantity < 1)
                result.AddError(new Error("Validation",
                    $"Line {i + 1}: quantity must be at least 1"));
            if (line.UnitPrice < 0)
                result.AddError(new Error("Validation",
                    $"Line {i + 1}: price cannot be negative"));
        }

        var duplicates = activeLines
            .Where(l => l.ProductId > 0)
            .GroupBy(l => l.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        foreach (var productId in duplicates)
            result.AddError(new Error("Validation",
                $"Product {productId} appears on more than one line"));

        if (result.IsFailure)
            return result; // Fast exit — nothing loaded, nothing saved

        // Practice 1 builds the ADD path only. Practice 2 replaces this block with the
        // load-or-create that lets the SAME method handle an existing order.
        if (view.Id != 0)
        {
            result.AddError(new Error("Not Supported",
                "Editing an existing order is Practice 2"));
            return result;
        }

        var order = new Order { OrderDate = DateTime.UtcNow };
        order.CustomerName = view.CustomerName.Trim();

        // Every line is new. Adding to the parent's collection is ALL EF Core needs —
        // it fills in OrderId itself at save time.
        foreach (var line in view.Lines)
        {
            if (line.IsRemoved) continue;

            order.Items.Add(new OrderItem
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                IsRemoved = false
            });
        }

        // ── Recompute the parent FROM its children ───────────────────────────────────
        // This is the payoff: Total is derived from the lines. Save the two separately
        // and there is a moment where the order says one thing and its lines say another.
        order.Total = order.Items
            .Where(i => !i.IsRemoved)
            .Sum(i => i.Quantity * i.UnitPrice);

        _context.Orders.Add(order);

        try
        {
            // ONE save. The parent insert and every child insert travel together in a
            // single implicit transaction — Day 27's lesson. No BeginTransactionAsync
            // is needed here: one save IS one transaction.
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database", ex.InnerException?.Message ?? ex.Message));
            return result;
        }
        catch (Exception ex)
        {
            _context.ChangeTracker.Clear();
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
