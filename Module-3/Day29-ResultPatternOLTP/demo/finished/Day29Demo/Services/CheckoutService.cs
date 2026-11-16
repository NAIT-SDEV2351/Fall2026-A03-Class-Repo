using BYSResults;
using Microsoft.EntityFrameworkCore;
using Day29Demo.Data;
using Day29Demo.Models;

namespace Day29Demo.Services;

public class CheckoutService(StoreDbContext context) : ICheckoutService
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] The bare-string version — the FOIL for today's lesson, and the shape
    /// `Result&lt;T&gt;` replaces. Written for today, NOT a copy of Day 27's code:
    /// Day 27's own checkout already returned Result&lt;string&gt;.
    /// The transaction boundary here is already correct (Day 27's Begin -> work -> ONE save ->
    /// Commit); the RETURN TYPE is what today fixes. Every outcome (success, not-found, stock,
    /// DB error) comes back as prose. The UI can show the text but cannot reliably branch on it:
    /// there is no machine-readable category to decide "red banner vs yellow" or "which field to
    /// highlight". A string is a message, not data. ResultCheckoutAsync below fixes exactly that.
    /// </summary>
    public async Task<string> StringCheckoutAsync(int productId, int quantity, string customer)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product is null) return "Product not found";
            if (product.StockQuantity < quantity) return "Insufficient stock";

            product.StockQuantity -= quantity;

            var order = new Order
            {
                CustomerName = customer,
                ProductId = productId,
                Quantity = quantity,
                Total = product.Price * quantity,
                CreatedAt = DateTime.UtcNow
            };
            _context.Orders.Add(order);

            // Single save — inventory update + order creation batched
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return $"Order #{order.Id} created for {customer}";
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            return $"Database error during checkout: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    /// <summary>
    /// [COMPLETED] The same checkout returning a typed Result&lt;Order&gt; (BYSResults package).
    /// Success carries the Order itself; failure carries one or more Error objects, each with a
    /// CATEGORY (.Code) the UI can branch on. Same Result&lt;T&gt; pattern from Module 1, now applied
    /// to an OLTP transaction.
    ///
    /// Structured in THREE TIERS, and the tier a check belongs to is decided by ONE question:
    /// *can I answer this from the arguments alone, or do I have to ask the database?*
    ///
    ///   Tier 1  INPUT   — answerable from the arguments. Free. Accumulate them all.
    ///   Tier 2  STATE   — only the database knows. Costs a round trip, so it runs after Tier 1,
    ///                     but BEFORE any row is modified.
    ///   Tier 3  WRITES  — the actual modification, plus the catch for what still goes wrong.
    ///
    /// Uses the course-default transaction boundary from Day 27 (BeginTransactionAsync ->
    /// work -> ONE SaveChangesAsync -> CommitAsync, with RollbackAsync in the catch). The
    /// boundary is unchanged from Day 27; what this method changes is the return type.
    /// </summary>
    public async Task<Result<Order>> ResultCheckoutAsync(
        int productId, int quantity, string customer)
    {
        // Start empty; each failed check adds an error
        var result = new Result<Order>();

        // ── Tier 1: INPUT validation — no database needed ────────────────────────────
        // These are facts about the request itself: is the customer name present, is the
        // quantity a sensible number. Nothing here needs a connection, so we check every
        // rule and report all the failures at once instead of one per round trip.
        // Blank name: a validation error
        if (string.IsNullOrWhiteSpace(customer))
            result.AddError(new Error("Validation", "Customer name is required"));
        // Name too long: a validation error
        else if (customer.Length > 200)
            result.AddError(new Error("Validation", "Customer name cannot exceed 200 characters"));
        // Zero or negative quantity: a validation error
        if (quantity <= 0)
            result.AddError(new Error("Validation", "Quantity must be positive"));
        // Any Tier-1 error: fast exit with all of them
        if (result.IsFailure)
            return result; // Fast exit — no connection, no transaction, nothing to roll back

        // Open the explicit transaction; it rolls back if never committed
        await using var transaction = await _context.Database.BeginTransactionAsync();
        // Everything in here runs inside the transaction
        try
        {
            // ── Tier 2: STATE pre-checks — only the database can answer these ────────
            // "Does product 9999 exist?" and "is there stock RIGHT NOW?" are unanswerable
            // from the arguments. They run before ANY row is modified, so that a bad
            // request gets a clean categorized message ("Not Found") instead of a raw SQL
            // foreign-key error that means nothing to the person at the keyboard.
            // Ask the database for the product (null if no such id)
            var product = await _context.Products.FindAsync(productId);
            // No such product: add the error, roll back, return
            if (product is null)
            {
                result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
                await transaction.RollbackAsync();
                return result;
            }
            // Not enough stock right now: same three steps, before any write
            if (product.StockQuantity < quantity)
            {
                result.AddError(new Error("Stock",
                    $"Product '{product.Name}': need {quantity}, have {product.StockQuantity}"));
                await transaction.RollbackAsync();
                return result;
            }

            // ── Tier 3: the WRITES ───────────────────────────────────────────────────
            // Write 1: deduct the stock (tracked, not sent yet)
            product.StockQuantity -= quantity;

            // Write 2: build the order from the checked inputs
            var order = new Order
            {
                CustomerName = customer,
                ProductId = productId,
                Quantity = quantity,
                Total = product.Price * quantity,
                CreatedAt = DateTime.UtcNow
            };
            // Track the new order; nothing is sent until the save
            _context.Orders.Add(order);

            // Single save — inventory update + order creation batched into one round trip
            await _context.SaveChangesAsync();

            // Commit: both writes become permanent together
            await transaction.CommitAsync();
            // Success: attach the typed Order payload. The caller gets the real entity back
            // (Id, Total, …) to render — not a "success" string it would have to parse.
            return result.WithValue(order);
        }
        // A failed save lands here (foreign key, constraint, deleted row)
        catch (DbUpdateException ex)
        {
            // The Tier-2 check above does NOT make this catch unreachable. Between the
            // moment FindAsync said "product 7 exists" and the moment SaveChangesAsync
            // runs, another transaction can delete product 7 — and then the foreign key
            // fails here. The pre-check buys a CLEAN MESSAGE; the catch buys CORRECTNESS.
            // You need both. (Day 32 gives this race a name: a concurrency conflict.)
            // Undo everything the transaction did
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Report it as a typed Database error
            result.AddError(new Error("Database",
                ex.InnerException?.Message ?? ex.Message));
            // Handled failure: the page shows the message, nothing rethrown
            return result;
        }
        // Anything else unexpected lands here
        catch (Exception ex)
        {
            // Undo everything the transaction did
            await transaction.RollbackAsync();
            // Drop tracked entities so no stale stock value lingers
            _context.ChangeTracker.Clear();
            // Report it as a typed Internal error
            result.AddError(new Error("Internal", $"Checkout failed: {ex.Message}"));
            return result;
        }
    }

    /// <summary>
    /// [PRE-BUILT] FOIL for Tier 2 — an UPDATE with NO existence pre-check.
    ///
    /// This is the case where "the ID is invalid" is genuinely invisible until the database
    /// is touched. We never load the product; we attach a stub carrying the caller's id and
    /// tell EF Core "a row with this key exists, change its Price." EF trusts us and emits
    /// UPDATE Products SET Price = @p WHERE Id = @id. If that id does not exist the statement
    /// affects ZERO rows, and EF raises DbUpdateConcurrencyException — at SaveChangesAsync,
    /// not at any line above it.
    ///
    /// Run this with a bad id and read the message the user gets: "The database operation was
    /// expected to affect 1 row(s), but actually affected 0 row(s)." That is technically true
    /// and completely useless to a human. ResultUpdatePriceAsync fixes it.
    /// </summary>
    public async Task<Result<Product>> UncheckedUpdatePriceAsync(int productId, decimal newPrice)
    {
        var result = new Result<Product>();

        // Tier 1 only — we validate the input and then go straight to the write.
        if (newPrice <= 0)
            result.AddError(new Error("Validation", "Price must be positive"));
        if (result.IsFailure)
            return result;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // NO Tier-2 check. We assert the row exists instead of asking.
            var stub = new Product { Id = productId, Name = "(unchanged)", Price = newPrice };
            _context.Products.Attach(stub);
            _context.Entry(stub).Property(p => p.Price).IsModified = true;

            await _context.SaveChangesAsync(); // <-- the failure surfaces HERE
            await transaction.CommitAsync();
            return result.WithValue(stub);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            // Note the category: "Database". The UI cannot tell the user which field to fix,
            // because the service never worked out that the PRODUCT ID was the problem.
            result.AddError(new Error("Database", ex.Message));
            return result;
        }
    }

    /// <summary>
    /// [PRE-BUILT] The same update, done properly — the three tiers applied to a modification.
    ///
    /// Tier 1 checks the price (arguments only). Tier 2 asks the database whether the product
    /// id is real and turns "no" into a clean Error("Not Found", …) the UI can act on. Tier 3
    /// does the write and STILL keeps its catch, because the row can disappear between the
    /// check and the save.
    ///
    /// Compare the two methods with productId = 9999. Same failure, same database, same
    /// exception underneath — completely different experience for the user.
    /// </summary>
    public async Task<Result<Product>> ResultUpdatePriceAsync(int productId, decimal newPrice)
    {
        var result = new Result<Product>();

        // ── Tier 1: INPUT — answerable without the database ──────────────────────────
        if (newPrice <= 0)
            result.AddError(new Error("Validation", "Price must be positive"));
        else if (newPrice > 100_000m)
            result.AddError(new Error("Validation", "Price cannot exceed $100,000"));
        if (result.IsFailure)
            return result;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // ── Tier 2: STATE — this is the check the foil above skipped ─────────────
            // One round trip converts an opaque "0 rows affected" into a sentence that
            // names the actual problem.
            var product = await _context.Products.FindAsync(productId);
            if (product is null)
            {
                result.AddError(new Error("Not Found", $"Product {productId} does not exist"));
                await transaction.RollbackAsync();
                return result;
            }

            // ── Tier 3: the WRITE ────────────────────────────────────────────────────
            product.Price = newPrice;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return result.WithValue(product);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Still reachable despite Tier 2 — someone deleted the row in between.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database",
                $"Product {productId} was modified or removed by another user. {ex.Message}"));
            return result;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Database",
                ex.InnerException?.Message ?? ex.Message));
            return result;
        }
        catch (Exception ex)
        {
            // Same trailing catch as ResultCheckoutAsync — every outcome comes back
            // as typed data, including the ones we did not anticipate.
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            result.AddError(new Error("Internal", $"Price update failed: {ex.Message}"));
            return result;
        }
    }
}
