using BYSResults;
using Day30Practice.Models;

namespace Day30Practice.Services;

public interface IOrderService
{
    /// <summary>Loads one complete order graph — parent, lines, and each line's product.</summary>
    Task<Result<OrderView>> GetOrderAsync(int orderId);

    /// <summary>Saves an order and its lines together — new or existing — in ONE round trip.</summary>
    Task<Result<OrderView>> AddEditOrderAsync(OrderView view);

    /// <summary>Products available to put on a line (fills the dropdown).</summary>
    Task<List<Product>> GetProductsAsync();
}
