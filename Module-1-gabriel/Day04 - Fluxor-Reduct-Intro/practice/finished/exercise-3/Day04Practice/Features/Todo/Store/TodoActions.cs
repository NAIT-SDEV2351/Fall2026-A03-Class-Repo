namespace Day04Practice.Features.Todo.Store;

public record AddTodoAction(string Title);
public record RemoveTodoAction(int Id);
public record ToggleTodoAction(int Id);
