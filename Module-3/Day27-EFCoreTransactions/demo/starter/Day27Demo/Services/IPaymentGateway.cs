using Day27Demo.Models;
using BYSResults;

namespace Day27Demo.Services;

/// <summary>
/// Abstraction over an external payment provider (Stripe, Square, …). ChargeAsync is a
/// NON-transactional network call: once the provider has taken the money, the database
/// transaction cannot undo it. Its presence BETWEEN the inventory save and the order save
/// is exactly what makes an explicit EF Core transaction necessary on Day 27 — a single
/// batched SaveChangesAsync cannot span an external call that must happen in the middle.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>Charges the customer and returns a provider confirmation reference on success.</summary>
    Task<Result<string>> ChargeAsync(string customer, decimal amount);
}
