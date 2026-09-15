using Day05Practice.Services;
using Fluxor;

namespace Day05Practice.Features.Todo.Store;

public class TodoEffects(ITodoService todoService)
{
    [EffectMethod]
    public async Task HandleLoadTodosAction(
        LoadTodosAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var todos = await todoService.GetAllAsync();
            dispatcher.Dispatch(new LoadTodosSuccessAction(todos));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadTodosFailureAction("Failed to load todos:" + ex.Message));
        }
    }
}