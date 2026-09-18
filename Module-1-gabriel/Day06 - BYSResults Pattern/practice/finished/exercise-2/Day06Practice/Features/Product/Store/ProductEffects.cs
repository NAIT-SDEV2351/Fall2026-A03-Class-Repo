using Fluxor;
using Day06Practice.Services;

namespace Day06Practice.Features.Product.Store;

// Practice 2 actions
public record LoadProductAction(int ProductId);
public record LoadProductSuccessAction(Services.Product Product);
public record LoadProductFailureAction(string ErrorMessage);

/// <summary>
/// Practice 2 COMPLETE: Effect refactored from try/catch to Result<T> checking.
/// </summary>
public class ProductEffects(IProductService productService)
{
    [EffectMethod]
    public async Task HandleLoadProductAction(
        LoadProductAction action, IDispatcher dispatcher)
    {
        // No try/catch: the service hands back a Result, so the effect just reads it.
        // result.Errors replaces the message we used to pull off a caught exception.
        var result = await productService.GetByIdAsync(action.ProductId);

        if (result.IsSuccess)
        {
            // result.Value! — the IsSuccess guard guarantees non-null at runtime; the ! keeps the build warning-clean since the compiler can't infer that from the guard.
            dispatcher.Dispatch(new LoadProductSuccessAction(result.Value!));
        }
        else
        {
            var errors = result.Errors.Select(e => e.ToString()).ToList();
            dispatcher.Dispatch(new LoadProductFailureAction(string.Join("; ", errors)));
        }
    }
}
