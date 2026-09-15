using Day05Practice.Services;
using Fluxor;

namespace Day05Practice.Features.Todo.Store;

public static class TodoReducers
{
    // --- Day 4 reducers (working) ---

    [ReducerMethod]
    public static TodoState ReduceAddTodoAction(TodoState state, AddTodoAction action)
        => state with
        {
            Items = [.. state.Items, new TodoItem(state.NextId, action.Title, false)],
            NextId = state.NextId + 1
        };

    [ReducerMethod]
    public static TodoState ReduceRemoveTodoAction(TodoState state, RemoveTodoAction action)
        => state with { Items = state.Items.Where(t => t.Id != action.Id).ToList() };

    [ReducerMethod]
    public static TodoState ReduceToggleTodoAction(TodoState state, ToggleTodoAction action)
        => state with
        {
            Items = state.Items.Select(t =>
                t.Id == action.Id ? t with { IsComplete = !t.IsComplete } : t
            ).ToList()
        };

    // --- Practice 1: Add three reducers for async loading ---
    // TODO: ReduceLoadTodosAction -- start the load: raise the loading flag, clear any previous error
    // TODO: ReduceLoadTodosSuccessAction -- store the loaded items, clear the loading flag
    // TODO: ReduceLoadTodosFailureAction -- store the error message, clear the loading flag

    // TODO (Practice 1, Task 3): Create TodoEffects.cs in this folder — see the practice walkthrough.
}
