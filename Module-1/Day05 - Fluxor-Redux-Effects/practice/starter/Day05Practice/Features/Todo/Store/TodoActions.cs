using Day05Practice.Services;

namespace Day05Practice.Features.Todo.Store;

// --- Day 4 actions (working) ---

public record AddTodoAction(string Title);
public record RemoveTodoAction(int Id);
public record ToggleTodoAction(int Id);

// --- Practice 1: Add three new actions for async loading ---
// TODO: Add LoadTodosAction (no parameters)
// TODO: Add LoadTodosSuccessAction -- carries the loaded items
// TODO: Add LoadTodosFailureAction -- carries the error message
