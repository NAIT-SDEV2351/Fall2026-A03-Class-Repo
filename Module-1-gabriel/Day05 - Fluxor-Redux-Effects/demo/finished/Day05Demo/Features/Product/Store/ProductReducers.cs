using Fluxor;

namespace Day05Demo.Features.Product.Store;

public static class ProductReducers
{
    // Reducers are pure: (currentState, action) → NEW state. No I/O, no awaits — that's the
    // effect's job. `state with { ... }` copies the record and overrides only the named
    // properties; every other field carries forward unchanged. One [ReducerMethod] per
    // action; Fluxor routes each dispatched action to the reducer whose parameter type matches.
    // Note the THREE reducers for ONE load: request flips the spinner on, then success OR
    // failure flips it off — which is what makes the loading state self-clearing.
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
