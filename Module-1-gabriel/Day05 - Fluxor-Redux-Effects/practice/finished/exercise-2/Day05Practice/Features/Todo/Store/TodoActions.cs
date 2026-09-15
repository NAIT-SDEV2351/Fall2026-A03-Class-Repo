using Day05Practice.Services;

namespace Day05Practice.Features.Todo.Store;

// Day 4 actions
public record AddTodoAction(string Title);
public record RemoveTodoAction(int Id);
public record ToggleTodoAction(int Id);

// Practice 1: Async loading actions (triple-action pattern)
public record LoadTodosAction;
public record LoadTodosSuccessAction(List<TodoItem> Items);
public record LoadTodosFailureAction(string Error);
