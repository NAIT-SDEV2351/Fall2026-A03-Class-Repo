namespace Day04Practice.Features.Cart.Store;

public record CartState
{
    public IReadOnlyList<CartItem> Items { get; init; } = [];
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public int TotalItems => Items.Sum(i => i.Quantity);
}
