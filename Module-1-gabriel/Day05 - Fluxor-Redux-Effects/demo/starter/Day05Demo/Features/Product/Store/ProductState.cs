namespace Day05Demo.Features.Product.Store;

public record ProductState
{
    public List<Services.Product> Items { get; init; } = [];
    
    public string? ErrorMessage { get; init; }
    
    public bool IsLoading { get; init; }
}