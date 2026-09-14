namespace Day04Practice.Features.Todo.Store;

using Day04Practice.Features.Todo.Models;
using Fluxor;

public static class TodoReducers
{
    // TODO: Add ReduceAddTodoAction
    // 1. Generate a new Id: state.Items.Count > 0 ? state.Items.Max(t => t.Id) + 1 : 1
    // 2. Create a new TodoItem(id, action.Title, false)
    // 3. Return state with { Items = [..state.Items, newItem] };

    // TODO: Add ReduceRemoveTodoAction
    // 1. Filter out the item: state.Items.Where(t => t.Id != action.Id).ToList()
    // 2. Return state with { Items = updatedItems };

    // TODO: Add ReduceToggleTodoAction
    // 1. Map over items: state.Items.Select(t => t.Id == action.Id ? t with { IsComplete = !t.IsComplete } : t).ToList()
    // 2. Return state with { Items = updatedItems };
}
