using Day05Practice.Services;
using Fluxor;

namespace Day05Practice.Features.Todo.Store;

// Effect: it owns the async side-effect (the service call) — the reducers can't, because
// they're pure and synchronous. The service is constructor-injected (primary-constructor
// syntax); Fluxor resolves it from DI. When the await finishes, the effect dispatches a
// success OR failure action back into the store — it never touches state directly.
public class TodoEffects(ITodoService todoService)
{
    [EffectMethod]
    public async Task HandleLoadTodosAction(LoadTodosAction action, IDispatcher dispatcher)
    {
        // try/catch is the Day-05 error channel: a thrown exception becomes a failure
        // action, so the reducer can turn it into an on-screen error instead of a crash.
        try
        {
            var todos = await todoService.GetAllAsync();
            dispatcher.Dispatch(new LoadTodosSuccessAction(todos));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadTodosFailureAction(ex.Message));
        }
    }
}
