namespace Day32Demo.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public Guid RowVersion { get; set; } = Guid.NewGuid();
}
