namespace Day04Practice.Features.Cart.Store;

using Fluxor;

public class CartFeature : Feature<CartState>
{
    public override string GetName() => "Cart";

    protected override CartState GetInitialState()
    {
        return new CartState();
    }
}
