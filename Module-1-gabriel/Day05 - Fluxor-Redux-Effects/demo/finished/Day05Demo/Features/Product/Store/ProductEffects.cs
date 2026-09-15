using Day05Demo.Services;
using Fluxor;

namespace Day05Demo.Features.Product.Store;

// The effect is the missing piece from Day 4: reducers are pure and can't do I/O, so the
// async service call has to live somewhere — that somewhere is an effect. The service is
// constructor-injected (primary-constructor syntax); Fluxor resolves it from DI because
// Program.cs registered IProductService. Effects own the side-effect and then dispatch a
// RESULT action back into the store — they never mutate state directly (reducers do that).
public class ProductEffects(IProductService productService)
{
    // [EffectMethod] + a LoadProductsAction parameter is how Fluxor wires this up: dispatch
    // LoadProductsAction anywhere and this method runs. It gets its own IDispatcher so it can
    // fire the follow-up success/failure action once the await completes.
    [EffectMethod]
    public async Task HandleLoadProductsAction(LoadProductsAction action, IDispatcher dispatcher)
    {
        // Day-05 effects use try/catch: the service signature is Task<List<Product>>, so a
        // failure surfaces as a thrown exception we catch and turn into a failure action.
        // (Day 6 refines this to a service that returns Result<T> — a failure becomes a value
        // we inspect instead of an exception we catch. Same triad, sturdier error channel.)
        try
        {
            var products = await productService.GetAllAsync();
            dispatcher.Dispatch(new LoadProductsSuccessAction(products));
        }
        catch (Exception ex)
        {
            // ex.Message preserves the error description but loses the exception type.
            // This is the standard Day-05 pattern — we'll refine catch clauses when
            // specific exception types are introduced in Module 3 (Day 33 concurrency).
            dispatcher.Dispatch(new LoadProductsFailureAction(ex.Message));
        }
    }
}
