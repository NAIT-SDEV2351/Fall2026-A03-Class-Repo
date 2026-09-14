namespace Day04Practice.Features.Todo.Store;

using Day04Practice.Features.Todo.Models;
using Fluxor;

public static class TodoReducers
{
    // Collection state is still IMMUTABLE — a reducer never edits the existing list, it builds a
    // NEW one: add = [.. state.Items, newItem] (spread into a fresh list); remove =
    // .Where(...).ToList() (filtered copy); toggle = .Select(... ? item with {…} : item).ToList()
    // (copy that replaces only the changed item). Same `with` rule as Counter, now over a list of records.
    [ReducerMethod]
    public static TodoState ReduceAddTodoAction(
        TodoState state,
        AddTodoAction action)
    {
        var newId = state.Items.Count > 0 ? state.Items.Max(t => t.Id) + 1 : 1;
        var newItem = new TodoItem(newId, action.Title, false);
        return state with { Items = [.. state.Items, newItem] };
    }

    [ReducerMethod]
    public static TodoState ReduceRemoveTodoAction(
        TodoState state,
        RemoveTodoAction action)
    {
        var updatedItems = state.Items
            .Where(t => t.Id != action.Id)
            .ToList();
        return state with { Items = updatedItems };
    }

    [ReducerMethod]
    public static TodoState ReduceToggleTodoAction(
        TodoState state,
        ToggleTodoAction action)
    {
        var updatedItems = state.Items
            .Select(t => t.Id == action.Id
                ? t with { IsComplete = !t.IsComplete }
                : t)
            .ToList();
        return state with { Items = updatedItems };
    }
}
