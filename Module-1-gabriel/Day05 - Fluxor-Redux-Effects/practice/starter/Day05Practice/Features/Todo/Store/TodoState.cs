using Day05Practice.Services;

namespace Day05Practice.Features.Todo.Store;

public record TodoState
{
    public List<TodoItem> Items { get; init; } = [];
    public int NextId { get; init; } = 1;

    // Async-loading state. The loading spinner and error alert in Todo.razor
    // are pre-built to read these two properties; your job is the backend that
    // drives them (the load actions, reducers, and effect).
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
