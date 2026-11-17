namespace Day30Demo.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    // Total is a fact ABOUT the children — recomputed from Items on every save, never
    // set directly by the caller. That dependency is the whole reason parent and
    // children have to be saved together.
    public decimal Total { get; set; }

    // COLLECTION navigation property: one Order has many OrderItems.
    // This is what .Include(o => o.Items) fills in. Without the Include the list comes
    // back EMPTY even for an order that has children in the database — and then every
    // existing line looks brand new.
    public List<OrderItem> Items { get; set; } = [];
}
