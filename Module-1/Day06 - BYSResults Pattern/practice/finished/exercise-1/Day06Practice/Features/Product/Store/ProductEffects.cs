using Fluxor;
using Day06Practice.Services;

namespace Day06Practice.Features.Product.Store;

// Practice 2 actions (used by ProductEffects)
public record LoadProductAction(int ProductId);
public record LoadProductSuccessAction(Services.Product Product);
public record LoadProductFailureAction(string ErrorMessage);

/// <summary>
/// Practice 2: After refactoring ProductService to return Result<T>,
/// update this effect to use result.IsSuccess instead of try/catch.
///
/// TODO: Replace the try/catch block with:
///   var result = await productService.GetByIdAsync(action.ProductId);
///   if (result.IsSuccess)
///       dispatcher.Dispatch(new LoadProductSuccessAction(result.Value!));   // ! -- the IsSuccess guard guarantees non-null
///   else
///       dispatcher.Dispatch(new LoadProductFailureAction(...));
/// </summary>
public class ProductEffects(IProductService productService)
{
    [EffectMethod]
    public async Task HandleLoadProductAction(
        LoadProductAction action, IDispatcher dispatcher)
    {
        // TODO (Practice 2): Replace this try/catch with Result<T> checking
        try
        {
            var product = await productService.GetByIdAsync(action.ProductId);
            dispatcher.Dispatch(new LoadProductSuccessAction(product));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadProductFailureAction(ex.Message));
        }
    }
}
