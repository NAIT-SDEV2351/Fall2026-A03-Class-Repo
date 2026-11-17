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
    /// Typed during the Exploring demo — see Instructor-Reference/demo-script.md.
    /// </summary>
    public async Task<Result<OrderView>> AddEditOrderAsync(OrderView view)
    {
        // [CODE LIVE] Typed during the demo, in this order:
        //   1. Tier 1 INPUT validation — customer name, at least one line, and per
        //      line a product / quantity >= 1 / price >= 0. (The duplicate-product
        //      check is in demo/finished but is NOT typed live — students add it in
        //      Practice 1.)
        //      Accumulate every error, then return before touching the database.
        //   2. Load or create the parent WITH its children (.Include(o => o.Items)).
        //      Id == 0 -> new Order. A non-zero Id that misses -> Error("Not Found").
        //   3. Copy the parent scalars across from the view.
        //   4. Per line: match by Id -> update it; no match -> order.Items.Add(new
        //      OrderItem). A removed line sets IsRemoved and keeps its row.
        //   5. Recompute order.Total from the non-removed children.
        //   6. Only a NEW parent needs _context.Orders.Add(order) — an existing one has
        //      been tracked since step 2, so it needs no Update() call.
        //   7. ONE SaveChangesAsync in try/catch; ChangeTracker.Clear() on failure.
        //   8. Re-read the saved graph by its real id and return it.

        throw new NotImplementedException("Typed live during the demo");
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
