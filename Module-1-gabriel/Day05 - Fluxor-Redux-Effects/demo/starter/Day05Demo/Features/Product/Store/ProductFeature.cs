using Fluxor;

namespace Day05Demo.Features.Product.Store;

public class ProductFeature : Feature<ProductState>
{
    public override string GetName() => "Product";

    protected override ProductState GetInitialState()
    {
        return new ProductState
        {
            Items = [],
            IsLoading = false,
            ErrorMessage = null
        };
    }
}