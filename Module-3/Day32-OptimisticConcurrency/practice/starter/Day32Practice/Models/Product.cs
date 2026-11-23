namespace Day32Practice.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // Practice 1: Uncomment the line below to enable optimistic concurrency
    // public Guid RowVersion { get; set; } = Guid.NewGuid();
}
