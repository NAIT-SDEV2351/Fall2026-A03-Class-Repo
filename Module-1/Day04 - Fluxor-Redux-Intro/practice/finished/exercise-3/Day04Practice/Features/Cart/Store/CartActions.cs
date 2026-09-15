namespace Day04Practice.Features.Cart.Store;

public record AddToCartAction(int ProductId, string Name, decimal Price);
public record RemoveFromCartAction(int ProductId);
public record UpdateQuantityAction(int ProductId, int NewQuantity);
