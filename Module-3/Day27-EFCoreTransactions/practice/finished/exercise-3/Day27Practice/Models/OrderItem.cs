namespace Day27Practice.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }  // Navigation property — EF Core sets OrderId during SaveChangesAsync (used by Practice 2)
    public int ProductId { get; set; }

    // Navigation property — makes the OrderItem -> Product foreign key real. The
    // ProductName below stays a deliberate denormalized snapshot of the name AT
    // PURCHASE TIME; the FK guarantees the referenced product actually existed.
    public Product? Product { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
