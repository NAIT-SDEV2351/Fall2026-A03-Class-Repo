namespace Day04Practice.Features.Cart.Store;

using Fluxor;

public static class CartReducers
{
    [ReducerMethod]
    public static CartState ReduceAddToCartAction(
        CartState state,
        AddToCartAction action)
    {
        // Add-or-increment: if the product is already in the cart, replace that one line with a
        // Quantity+1 copy; otherwise append a new line. Either branch returns a NEW list, never a mutation.
        var existing = state.Items.FirstOrDefault(i => i.ProductId == action.ProductId);

        if (existing is not null)
        {
            var updatedItems = state.Items
                .Select(i => i.ProductId == action.ProductId
                    ? i with { Quantity = i.Quantity + 1 }
                    : i)
                .ToList();
            return state with { Items = updatedItems };
        }

        var newItem = new CartItem(action.ProductId, action.Name, action.Price, 1);
        return state with { Items = [.. state.Items, newItem] };
    }

    [ReducerMethod]
    public static CartState ReduceRemoveFromCartAction(
        CartState state,
        RemoveFromCartAction action)
    {
        var updatedItems = state.Items
            .Where(i => i.ProductId != action.ProductId)
            .ToList();
        return state with { Items = updatedItems };
    }

    [ReducerMethod]
    public static CartState ReduceUpdateQuantityAction(
        CartState state,
        UpdateQuantityAction action)
    {
        var updatedItems = state.Items
            .Select(i => i.ProductId == action.ProductId
                ? i with { Quantity = action.NewQuantity }
                : i)
            .ToList();
        return state with { Items = updatedItems };
    }
}
