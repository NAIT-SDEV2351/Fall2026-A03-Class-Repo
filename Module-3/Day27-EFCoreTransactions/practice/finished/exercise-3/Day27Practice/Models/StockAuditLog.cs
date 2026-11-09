namespace Day27Practice.Models;

public class StockAuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
