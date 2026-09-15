using Day05Practice.Services;

namespace Day05Practice.Features.Todo.Store;

public record TodoState
{
    public List<TodoItem> Items { get; init; } = [];
    public int NextId { get; init; } = 1;
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
