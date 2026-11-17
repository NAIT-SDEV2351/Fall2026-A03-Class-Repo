namespace Day30Demo.Models;

/// <summary>
/// Disconnected view of one order line.
/// Id == 0 means "new line — the database has never seen it".
/// A non-zero Id means it matches an existing OrderItem row, and that match is what
/// tells the service to UPDATE rather than INSERT.
/// </summary>
public class OrderLineView
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsRemoved { get; set; }
}
