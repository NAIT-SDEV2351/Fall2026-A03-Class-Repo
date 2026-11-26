using BYSResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Day34Practice.Data;
using Day34Practice.Models;
using Day34Practice.Services;

namespace Day34Practice.Utilities;

/// <summary>
/// [PRE-BUILT] Simulates two concurrent requests trying to reserve the same product.
///
/// It calls YOUR <see cref="ReservationService.ReserveWithTimeoutAsync"/> — it does not
/// reserve stock itself. Each simulated request gets its OWN DbContext (from
/// IDbContextFactory) and therefore its OWN ReservationService, exactly like two real HTTP
/// requests each getting their own scoped instances. That separation is mandatory: DbContext
/// is not thread-safe, so two concurrent requests must never share one.
///
/// The lock behaviour you see on /concurrent-test is produced entirely by the code you write.
/// If ReserveWithTimeoutAsync is still a stub, this page reports its NotImplementedException.
/// </summary>
public class ConcurrentRequestSimulator(IDbContextFactory<StoreDbContext> factory)
{
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    public async Task<(Result<Product> ResultA, Result<Product> ResultB, List<string> Log)>
        SimulateConcurrentReservationsAsync(int productId, int quantityA, int quantityB)
    {
        var log = new ConcurrentQueue<string>();

        // Check product exists and show starting stock
        using (var checkContext = await _factory.CreateDbContextAsync())
        {
            var product = await checkContext.Products.FindAsync(productId);
            if (product is null)
            {
                var notFound = new Result<Product>();
                notFound.AddError(new Error("NotFound", $"Product {productId} does not exist."));
                log.Enqueue($"Product {productId} not found.");
                return (notFound, notFound, [.. log]);
            }
            log.Enqueue($"Starting stock for \"{product.Name}\": {product.StockQuantity} units");
            log.Enqueue($"Request A wants: {quantityA} units | Request B wants: {quantityB} units");
            log.Enqueue("---");
        }

        // Launch both reservations concurrently — each with its own DbContext + service
        log.Enqueue("[Both] Starting concurrent reservation requests...");

        var taskA = ReserveWithLoggingAsync(productId, quantityA, "A", log);
        // Small stagger so A wins the race for the lock and the ordering is stable in the log
        await Task.Delay(100);
        var taskB = ReserveWithLoggingAsync(productId, quantityB, "B", log);

        var results = await Task.WhenAll(taskA, taskB);

        log.Enqueue("---");

        // Check final stock
        using (var finalContext = await _factory.CreateDbContextAsync())
        {
            var finalProduct = await finalContext.Products.FindAsync(productId);
            if (finalProduct is not null)
            {
                log.Enqueue($"Final stock for \"{finalProduct.Name}\": {finalProduct.StockQuantity} units");
            }
        }

        return (results[0], results[1], [.. log]);
    }

    /// <summary>
    /// One simulated HTTP request: its own DbContext, its own ReservationService, YOUR method.
    /// </summary>
    private async Task<Result<Product>> ReserveWithLoggingAsync(
        int productId, int quantity, string user, ConcurrentQueue<string> log)
    {
        // A real request gets a scoped DbContext + a scoped ReservationService. We build the
        // same pair by hand so the two simulated requests are genuinely independent.
        using var context = await _factory.CreateDbContextAsync();
        var service = new ReservationService(context);

        log.Enqueue($"[Request {user}] Calling ReserveWithTimeoutAsync({productId}, {quantity})...");

        try
        {
            var result = await service.ReserveWithTimeoutAsync(productId, quantity);

            if (result.IsSuccess)
            {
                log.Enqueue(
                    $"[Request {user}] SUCCESS — reserved {quantity}, " +
                    $"remaining: {result.Value!.StockQuantity}");
                return result;
            }

            // Surface the typed error YOUR method returned. The code is what matters here:
            // "LockTimeout" is the whole point of Practice 2.
            foreach (var error in result.Errors)
            {
                var line = error.Code switch
                {
                    "LockTimeout" => "LOCK TIMEOUT — could not acquire lock",
                    "Deadlock" => "DEADLOCK — chosen as the victim",
                    "InsufficientStock" => $"INSUFFICIENT STOCK — {error.Message}",
                    _ => $"ERROR — [{error.Code}] {error.Message}",
                };
                log.Enqueue($"[Request {user}] {line}");
            }
            return result;
        }
        catch (Exception ex)
        {
            // Reaching here means ReserveWithTimeoutAsync threw instead of returning a Result —
            // most commonly the NotImplementedException from an unfinished stub.
            log.Enqueue($"[Request {user}] ERROR — {ex.Message}");
            var failed = new Result<Product>();
            failed.AddError(new Error("Failed", ex.Message));
            return failed;
        }
    }
}
