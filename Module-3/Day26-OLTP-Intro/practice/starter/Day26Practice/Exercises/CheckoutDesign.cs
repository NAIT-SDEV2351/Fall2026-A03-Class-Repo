// =============================================================
// Practice 2: Transaction Boundary Design
// =============================================================
// Below is a 6-step e-commerce checkout workflow.
// With a partner, decide:
//   1. Which steps should be in the SAME transaction?
//   2. Are there any steps that could be OUTSIDE the transaction?
//   3. What happens if you DON'T put them in a transaction?
//
// Mark your boundaries with comments:
//   // --- TRANSACTION START ---
//   // --- TRANSACTION END ---
// =============================================================

namespace Day26Practice.Exercises;

/// <summary>
/// Analyze this checkout workflow and design transaction boundaries.
/// This file is for analysis — it does not compile or run.
/// </summary>
public static class CheckoutDesign
{
    // E-Commerce Checkout Workflow
    // ============================

    // Step 1: Validate cart items
    // - Check that each product in the cart still exists
    // - Check that each product has sufficient stock
    // - If any item is invalid, reject the checkout

    // Step 2: Create Order record
    // - Insert a new Order row (auto-generated OrderId)
    // - Set status to "Pending"

    // Step 3: Create OrderItem records
    // - For each cart item, insert an OrderItem row
    // - Each OrderItem references the OrderId from Step 2
    // - Record the unit price at time of purchase

    // Step 4: Deduct inventory
    // - For each cart item, reduce the product's StockQuantity
    // - The deduction must match the validated quantity from Step 1

    // Step 5: Record payment
    // - Insert a Payment row linked to the OrderId
    // - Record the total amount and payment method

    // Step 6: Clear the shopping cart
    // - Remove all CartItem rows for this cart
    // - Only clear if all previous steps succeeded

    // YOUR DESIGN:
    // Draw your transaction boundaries below.
    // Which steps are inside? Which could be outside? Why?
    //
    //
    //
    //

    // DISCUSSION QUESTIONS:
    // 1. What happens if Step 4 (deduct inventory) fails but Steps 2-3 already committed?
    //
    // 2. Can Step 1 (validation) be outside the transaction? What's the risk?
    //
    // 3. Can Step 6 (clear cart) be outside the transaction? What's the risk?
    //
}
