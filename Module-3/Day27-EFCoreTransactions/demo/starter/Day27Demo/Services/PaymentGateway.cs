using Day27Demo.Models;
using BYSResults;

namespace Day27Demo.Services;

/// <summary>
/// Stand-in for a real payment provider. Always approves the charge and returns a fake
/// confirmation reference. Registered as the default IPaymentGateway so a normal checkout
/// succeeds. (A real implementation makes an HTTPS call to Stripe/Square here.)
/// </summary>
public class PaymentGateway : IPaymentGateway
{
    public Task<Result<string>> ChargeAsync(string customer, decimal amount)
    {
        var reference = $"PAY-{customer.ToUpperInvariant()}-{amount:0.00}";
        return Task.FromResult(Result<string>.Success(reference));
    }
}
