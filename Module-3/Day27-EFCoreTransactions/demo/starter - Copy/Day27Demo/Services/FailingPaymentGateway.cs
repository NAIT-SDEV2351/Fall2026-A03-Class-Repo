using Day27Demo.Models;
using BYSResults;

namespace Day27Demo.Services;

/// <summary>
/// A payment provider that always DECLINES. Swap this in (in Program.cs) instead of the real
/// PaymentGateway to exercise the rollback path: the charge fails BETWEEN the two saves, the
/// inventory reservation is rolled back, and no order is created. It is a separate type — not
/// a mutable "fail next" flag — so the failure is a deliberate configuration choice, not
/// hidden state that the calling code has to remember to reset.
/// </summary>
public class FailingPaymentGateway : IPaymentGateway
{
    public Task<Result<string>> ChargeAsync(string customer, decimal amount)
        => Task.FromResult(Result<string>.Failure(new Error("Validation", "Card declined by issuer.")));
}
