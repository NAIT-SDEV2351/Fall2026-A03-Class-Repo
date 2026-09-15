using Fluxor;

namespace Day05Practice.Features.Product.Store;

public class ProductFeature : Feature<ProductState>
{
    public override string GetName() => "Product";

    protected override ProductState GetInitialState()
        => new() { Items = [], IsLoading = false, ErrorMessage = null };
}
