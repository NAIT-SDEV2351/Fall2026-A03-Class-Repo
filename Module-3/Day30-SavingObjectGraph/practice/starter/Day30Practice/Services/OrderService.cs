using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day30Practice.Data;
using Day30Practice.Models;

namespace Day30Practice.Services;

public class OrderService(StoreDbContext context) : IOrderService
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] The READ side of the graph — read it before you write the save.
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
    /// Practice 1 + Practice 2: ONE method that both ADDS a new order and EDITS an
    /// existing one. Practice 1 builds the ADD path. Practice 2 extends this SAME
    /// method to handle the EDIT path — you will not write a second method.
    /// </summary>
    public async Task<Result<OrderView>> AddEditOrderAsync(OrderView view)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
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
