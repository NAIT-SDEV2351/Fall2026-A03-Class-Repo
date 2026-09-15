using Fluxor;

namespace Day05Practice.Features.Product.Store;

public static class ProductReducers
{
    [ReducerMethod]
    public static ProductState ReduceLoadProductsAction(ProductState state, LoadProductsAction action)
        => state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static ProductState ReduceLoadProductsSuccessAction(ProductState state, LoadProductsSuccessAction action)
        => state with { Items = action.Products, IsLoading = false };

    [ReducerMethod]
    public static ProductState ReduceLoadProductsFailureAction(ProductState state, LoadProductsFailureAction action)
        => state with { ErrorMessage = action.Error, IsLoading = false };
}
