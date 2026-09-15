namespace Day04Practice.Features.Cart.Store;

using Fluxor;

public static class CartReducers
{
    [ReducerMethod]
    public static CartState ReduceAddToCartAction(
        CartState state,
        AddToCartAction action)
    {
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

    // TODO: Add ReduceRemoveFromCartAction
    // 1. Filter out the item by ProductId
    // 2. Return state with { Items = updatedItems };

    // TODO: Add ReduceUpdateQuantityAction
    // 1. Map over items, update Quantity for matching ProductId
    // 2. Return state with { Items = updatedItems };
}
