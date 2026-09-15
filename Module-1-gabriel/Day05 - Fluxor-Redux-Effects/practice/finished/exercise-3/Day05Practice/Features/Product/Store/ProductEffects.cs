using Day05Practice.Services;
using Fluxor;

namespace Day05Practice.Features.Product.Store;

public class ProductEffects(IProductService productService)
{
    [EffectMethod]
    public async Task HandleLoadProductsAction(LoadProductsAction action, IDispatcher dispatcher)
    {
        try
        {
            var products = await productService.GetAllAsync();
            dispatcher.Dispatch(new LoadProductsSuccessAction(products));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadProductsFailureAction(ex.Message));
        }
    }
}
