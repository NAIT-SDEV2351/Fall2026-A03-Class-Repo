namespace Day27Demo.Models;

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

    // Confirmation reference returned by the payment gateway. The order cannot be written
    // until the charge succeeds and hands back this value — which is why the charge sits
    // BETWEEN the two saves and the explicit transaction is required.
    public string? PaymentReference { get; set; }
}
