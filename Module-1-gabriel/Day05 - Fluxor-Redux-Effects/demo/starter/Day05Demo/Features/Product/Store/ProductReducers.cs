using Fluxor;

namespace Day05Demo.Features.Product.Store;

public static class ProductReducers
{
    [ReducerMethod]
    public static ProductState ReduceLoadProductsAction(
        ProductState state,
        LoadProductsAction action)
    {
        return state with { IsLoading = true, ErrorMessage = null };
    }

    [ReducerMethod]
    public static ProductState ReduceLoadProductsSuccessAction(ProductState state, LoadProductsSuccessAction action)
    {
        return state with { IsLoading = false, Items = action.Items };
    }
    
    [ReducerMethod]
    public static ProductState ReduceLoadProductsFailureAction(ProductState state, LoadProductsFailureAction action)
    {
        return state with { IsLoading = false, ErrorMessage = action.Error};
    }
}