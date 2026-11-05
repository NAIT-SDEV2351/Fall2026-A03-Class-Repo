namespace Day26Demo.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ProductId { get; set; }

    // Navigation property — this is what makes the foreign key REAL. Without it EF Core
    // creates no FK constraint, and an Order carrying a ProductId that does not exist
    // inserts silently. With it, SaveChangesAsync is the line that finds the problem.
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
