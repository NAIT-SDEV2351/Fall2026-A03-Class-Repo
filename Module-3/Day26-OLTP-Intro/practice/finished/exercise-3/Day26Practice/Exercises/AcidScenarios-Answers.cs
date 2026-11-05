// =============================================================
// Practice 1: ACID Violation Identification — ANSWER KEY
// =============================================================

namespace Day26Practice.Exercises;

public static class AcidScenariosAnswers
{
    // Scenario 1: The Double-Purchase
    // ANSWER: Isolation
    // Both transactions read the same stale value (stock=1) because
    // they weren't isolated from each other. Proper isolation ensures
    // only one transaction sees stock=1; the other sees stock=0.

    // Scenario 2: The Phantom Order
    // ANSWER: Atomicity
    // Step 1 (inventory deduction) committed independently of Step 2
    // (order creation). Atomicity requires all steps to succeed or
    // all to fail. An explicit transaction would roll back Step 1.

    // Scenario 3: The Mismatched Total
    // ANSWER: Consistency
    // The database ended up in an invalid state — the order total
    // doesn't match the line items. A consistency guarantee would
    // prevent this by ensuring business rules are maintained.
    // (Note: Isolation is also a valid answer — the price change
    // from another user interfered with the total calculation.)

    // Scenario 4: The Vanishing Confirmation
    // ANSWER: Durability
    // Committed data (the order) did not survive the server crash.
    // Durability guarantees that once a transaction is committed,
    // it persists regardless of system failures.

    // Scenario 5: The Stale Cart
    // ANSWER: Isolation
    // The customer's transaction read a price ($10) that was changed
    // by another transaction ($15) before checkout completed. This is
    // a non-repeatable read — the value changed between reads.
}
