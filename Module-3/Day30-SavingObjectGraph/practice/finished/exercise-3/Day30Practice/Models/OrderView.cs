namespace Day30Practice.Models;

/// <summary>
/// DISCONNECTED view of an order — what the UI binds to and hands back to the service.
/// Not an EF entity: the context has never seen it, so nothing on it is tracked.
/// (Precedent: Day 35's CartItem.)
/// </summary>
public class OrderView
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderLineView> Lines { get; set; } = [];
}
