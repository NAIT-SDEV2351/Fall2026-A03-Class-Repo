namespace Day04Practice.Features.Todo.Store;

using Day04Practice.Features.Todo.Models;

public record TodoState
{
    public IReadOnlyList<TodoItem> Items { get; init; } = [];
}
