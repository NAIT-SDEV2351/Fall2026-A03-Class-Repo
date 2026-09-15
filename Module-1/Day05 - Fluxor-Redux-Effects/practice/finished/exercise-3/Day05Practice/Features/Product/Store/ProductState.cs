namespace Day05Practice.Features.Product.Store;

public record ProductState
{
    public List<Services.Product> Items { get; init; } = [];
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
