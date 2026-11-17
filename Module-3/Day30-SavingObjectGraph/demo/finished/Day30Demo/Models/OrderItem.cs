namespace Day30Demo.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Soft delete: a removed line keeps its row and its history. Nothing is deleted —
    // the flag takes the line out of the Total instead.
    public bool IsRemoved { get; set; }

    // Reference navigation properties — the "one" side of each relationship.
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
