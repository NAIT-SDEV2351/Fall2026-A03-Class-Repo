// =============================================================
// Practice 1: ACID Violation Identification
// =============================================================
// For each scenario below, identify which ACID property is violated.
// Write your answer in the YOUR ANSWER line.
//
// Properties: Atomicity, Consistency, Isolation, Durability
// =============================================================

namespace Day26Practice.Exercises;

/// <summary>
/// These scenarios are for analysis — this file does not compile or run.
/// Read each scenario and identify the ACID violation.
/// </summary>
public static class AcidScenarios
{
    // ---------------------------------------------------------
    // Scenario 1: The Double-Purchase
    // ---------------------------------------------------------
    // Two customers (Alice and Bob) both try to buy the last unit
    // of a product at the same time.
    //
    // Timeline:
    //   Alice reads: StockQuantity = 1
    //   Bob reads:   StockQuantity = 1
    //   Alice deducts: StockQuantity = 0 (saved)
    //   Bob deducts:   StockQuantity = -1 (saved)
    //
    // Result: Inventory is -1. Both orders went through.
    //
    // YOUR ANSWER: Which ACID property was violated?
    // ➜ _______________

    // ---------------------------------------------------------
    // Scenario 2: The Phantom Order
    // ---------------------------------------------------------
    // During checkout, the system:
    //   Step 1: Deducts inventory (SaveChangesAsync) ✓
    //   Step 2: Server crashes
    //   Step 3: Creates order (never reached) ✗
    //
    // Result: Inventory was reduced, but no order exists.
    //
    // YOUR ANSWER: Which ACID property was violated?
    // ➜ _______________

    // ---------------------------------------------------------
    // Scenario 3: The Mismatched Total
    // ---------------------------------------------------------
    // An order is created with 3 items:
    //   Item 1: Widget × 2 @ $10 = $20
    //   Item 2: Gadget × 1 @ $25 = $25
    //   Item 3: Gizmo  × 3 @ $5  = $15
    //
    // The order's Total field shows $50, but the line items
    // add up to $60. The price of Gizmo was updated from $5 to
    // $15 by another user between when the line items were created
    // and when the total was calculated.
    //
    // Result: Order total doesn't match the sum of line items.
    //
    // YOUR ANSWER: Which ACID property was violated?
    // ➜ _______________

    // ---------------------------------------------------------
    // Scenario 4: The Vanishing Confirmation
    // ---------------------------------------------------------
    // A customer completes checkout and sees "Order Confirmed!"
    // on their screen. The server crashes immediately after.
    // When the server restarts, the order is not in the database.
    //
    // Result: The customer was told the order succeeded, but
    //         it doesn't exist.
    //
    // YOUR ANSWER: Which ACID property was violated?
    // ➜ _______________

    // ---------------------------------------------------------
    // Scenario 5: The Stale Cart
    // ---------------------------------------------------------
    // A customer browses products and adds a $10 item to their
    // cart. While they're browsing, an admin updates the price
    // to $15. The customer proceeds to checkout. The cart shows
    // $10, but the database now has $15.
    //
    // Result: The customer sees outdated pricing information.
    //
    // YOUR ANSWER: Which ACID property was violated?
    // ➜ _______________
}
