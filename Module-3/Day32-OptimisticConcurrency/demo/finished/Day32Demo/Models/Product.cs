namespace Day32Demo.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // The optimistic-concurrency TOKEN. EF Core stamps this value into the WHERE clause of every
    // UPDATE ("...WHERE Id = @id AND RowVersion = @originalVersion"). If another user saved first,
    // the stored version no longer matches, the UPDATE hits 0 rows, and EF throws
    // DbUpdateConcurrencyException — that thrown exception is the "someone edited this first" signal.
    public Guid RowVersion { get; set; } = Guid.NewGuid();
}
