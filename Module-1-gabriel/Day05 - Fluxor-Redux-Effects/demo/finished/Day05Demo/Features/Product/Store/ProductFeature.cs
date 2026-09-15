using Fluxor;

namespace Day05Demo.Features.Product.Store;

public class ProductFeature : Feature<ProductState>
{
    // GetName() identifies this slice in the store (and in Redux DevTools). GetInitialState()
    // is the state before any action runs — an empty list, NOT loading: that's exactly what
    // the page shows on first paint, until OnInitialized dispatches LoadProductsAction.
    public override string GetName() => "Product";

    protected override ProductState GetInitialState()
        => new() { Items = [], IsLoading = false, ErrorMessage = null };
}
