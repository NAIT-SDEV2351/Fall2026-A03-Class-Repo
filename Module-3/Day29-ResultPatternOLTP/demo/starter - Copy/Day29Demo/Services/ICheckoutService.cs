using BYSResults;
using Day29Demo.Models;

namespace Day29Demo.Services;

// Interface extracted for DI best practices — inject ICheckoutService, not CheckoutService
public interface ICheckoutService
{
    Task<string> StringCheckoutAsync(int productId, int quantity, string customer);
    Task<Result<Order>> ResultCheckoutAsync(int productId, int quantity, string customer);

    // The Tier-2 pair: the same update without and with the existence pre-check.
    Task<Result<Product>> UncheckedUpdatePriceAsync(int productId, decimal newPrice);
    Task<Result<Product>> ResultUpdatePriceAsync(int productId, decimal newPrice);
}
