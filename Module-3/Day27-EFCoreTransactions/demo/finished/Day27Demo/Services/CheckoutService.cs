using Day27Demo.Data;
using Day27Demo.Models;
using BYSResults;
using Microsoft.EntityFrameworkCore;

namespace Day27Demo.Services;

public class CheckoutService(StoreDbContext context, IPaymentGateway paymentGateway)
{
    private readonly StoreDbContext _context = context;
    private readonly IPaymentGateway _paymentGateway = paymentGateway;

    /// <summary>
    /// Counter-example FIRST (run this before the gateway story): a pure-EF checkout that
    /// writes BOTH the inventory change and the order in a SINGLE SaveChangesAsync. One save
    /// is already one implicit transaction — both rows commit together or neither does. There
    /// is no external call in the middle, so NO explicit transaction is needed. This is the
    /// industry-correct shape whenever everything is pure EF Core; we only reach for an
    /// explicit transaction once a non-transactional step forces the writes apart (below).
    /// </summary>
    public async Task<Result<string>> SimpleCheckoutAsync(int productId, int quantity, string customer)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
            return Result<string>.Failure(new Error("NotFound", "Product not found."));
        if (product.StockQuantity < quantity)
            return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

        product.StockQuantity -= quantity;
        _context.Orders.Add(new Order
        {
            CustomerName = customer,
            ProductId = productId,
            Quantity = quantity,
            Total = product.Price * quantity,
            CreatedAt = DateTime.UtcNow
        });

        // ONE save — the inventory update and the order insert are already atomic together.
        await _context.SaveChangesAsync();
        return Result<string>.Success($"Order created for {customer} (no transaction needed).");
    }

    /// <summary>
    /// BROKEN: the same checkout, but now a payment gateway sits between two saves and there
    /// is NO explicit transaction. Step 1 reserves inventory and commits independently; the
    /// card is then charged; a crash before the order write leaves inventory deducted AND the
    /// customer charged AND no order on file — three systems out of sync with nothing to undo.
    /// Contrast with SafeCheckoutAsync, which wraps the whole sequence in one transaction.
    /// </summary>
    public async Task<Result<string>> BrokenCheckoutAsync(int productId, int quantity, string customer)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
            return Result<string>.Failure(new Error("NotFound", "Product not found."));
        if (product.StockQuantity < quantity)
            return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

        // Step 1: Reserve inventory — this SaveChangesAsync commits on its own.
        product.StockQuantity -= quantity;
        await _context.SaveChangesAsync();

        // Step 2: Charge the external gateway. A handled decline returns a Result.
        var charge = await _paymentGateway.ChargeAsync(customer, product.Price * quantity);
        if (!charge.IsSuccess)
            return Result<string>.Failure(new Error("Validation", $"Payment declined: {charge.FirstError?.Message}"));

        // SIMULATED CRASH between the charge and the order write (network timeout, server
        // restart, deadlock). This is NOT a handled error — it is the uncontrolled interruption
        // the demo exists to show, so it stays a throw. Step 1 already committed and the card was
        // already charged; with no transaction, nothing below can undo either one.
        throw new InvalidOperationException(
            "Simulated failure! Inventory was deducted and the card was charged, but the order was " +
            "never created. This is an Atomicity violation — check the Inventory page to see the damage.");

        // Step 3: Record the order — NEVER REACHED (see SafeCheckoutAsync for the success path)
        // var order = new Order { CustomerName = customer, ProductId = productId, Quantity = quantity,
        //     Total = product.Price * quantity, CreatedAt = DateTime.UtcNow, PaymentReference = charge.Value };
        // _context.Orders.Add(order);
        // await _context.SaveChangesAsync();
        // return Result<string>.Success($"Order #{order.Id} created for {customer}.");
    }

    /// <summary>
    /// FIX: the two saves and the external charge are wrapped in one explicit transaction.
    ///
    /// *** THIS METHOD IS THE COURSE'S ONE EXCEPTION — DO NOT TREAT IT AS THE PATTERN. ***
    /// It has TWO SaveChangesAsync calls for exactly one reason: an external payment charge
    /// sits between them and cannot join a database transaction. That is the only case in
    /// SDEV 2351 that forces a second save, students are never asked to write it, and the
    /// payment gateway does not appear again after Day 27. The pattern to carry forward is
    /// the SINGLE-save transaction — see SimpleCheckoutAsync above for the one-save shape,
    /// and Day-29 ResultCheckoutAsync / Day-35 CheckoutAsync, which both reuse
    /// Begin -> work -> ONE SaveChangesAsync -> Commit, with Rollback + ChangeTracker.Clear
    /// in the catch.
    /// The charge sits BETWEEN the inventory save and the order save — that is what makes the
    /// explicit transaction necessary (a single batched SaveChangesAsync cannot span the
    /// external call). If the charge declines or the order save fails, RollbackAsync undoes the
    /// inventory reservation. NOTE: the gateway charge is the one thing the database cannot
    /// revert — in production a charge-then-rollback case issues a compensating refund. We
    /// order the work so the only post-charge step is a local save, keeping that window small.
    /// </summary>
    public async Task<Result<string>> SafeCheckoutAsync(int productId, int quantity, string customer)
    {
        // Open one transaction; await using disposes it (and rolls back) if we never commit
        await using var transaction = await _context.Database.BeginTransactionAsync();
        // Everything from here to CommitAsync is one all-or-nothing unit
        try
        {
            // Load the product inside the transaction
            var product = await _context.Products.FindAsync(productId);
            // Unknown product: nothing written yet, so just return
            if (product is null)
                return Result<string>.Failure(new Error("NotFound", "Product not found."));
            // Not enough stock: also nothing written yet
            if (product.StockQuantity < quantity)
                return Result<string>.Failure(new Error("Validation", "Insufficient stock."));

            // Step 1: Reserve inventory and save.
            product.StockQuantity -= quantity;
            // Written to the database, but not permanent until CommitAsync
            await _context.SaveChangesAsync();

            // Step 2: Charge the external gateway — sits BETWEEN the two saves.
            var charge = await _paymentGateway.ChargeAsync(customer, product.Price * quantity);
            // A declined charge comes back as a failed Result, not an exception
            if (!charge.IsSuccess)
            {
                // Declined: roll back the inventory reservation. No order, no inventory change.
                await transaction.RollbackAsync();
                // Forget the tracked stock change so this context matches the database again
                _context.ChangeTracker.Clear();
                // Report the decline to the page
                return Result<string>.Failure(new Error("Validation", $"Payment declined: {charge.FirstError?.Message}"));
            }

            // Step 3: Record the order with the gateway's confirmation reference, then save.
            var order = new Order
            {
                // Who bought it
                CustomerName = customer,
                // What they bought
                ProductId = productId,
                // How many
                Quantity = quantity,
                // Price at the time of sale
                Total = product.Price * quantity,
                // When the order was placed
                CreatedAt = DateTime.UtcNow,
                // The gateway's receipt id, so the charge and the order line up
                PaymentReference = charge.Value
            };
            // Track the new order
            _context.Orders.Add(order);
            // Second save, still inside the same transaction
            await _context.SaveChangesAsync();

            // Make both saves permanent together
            await transaction.CommitAsync();
            // Success: the order id exists now that the insert has run
            return Result<string>.Success(
                $"Order #{order.Id} created for {customer}. Total: {order.Total:C} (payment {charge.Value}).");
        }
        // Any exception between Begin and Commit lands here
        catch (Exception ex)
        {
            // Any failure rolls back ALL database changes and returns a handled result — no rethrow.
            await transaction.RollbackAsync();
            // Drop tracked changes so a retry starts clean
            _context.ChangeTracker.Clear();
            // Report the failure to the page instead of crashing it
            return Result<string>.Failure(new Error("Internal", $"Checkout failed: {ex.Message}"));
        }
    }

    /// <summary>
    /// Resets the database to its original seeded state. Useful for re-running demos.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        _context.Orders.RemoveRange(_context.Orders);

        var products = await _context.Products.ToListAsync();
        var seedData = new Dictionary<int, int>
        {
            { 1, 10 }, { 2, 50 }, { 3, 30 }, { 4, 15 }, { 5, 25 }
        };
        foreach (var product in products)
        {
            if (seedData.TryGetValue(product.Id, out int originalStock))
                product.StockQuantity = originalStock;
        }
        await _context.SaveChangesAsync();
    }
}
