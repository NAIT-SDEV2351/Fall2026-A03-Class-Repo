// =============================================================
// Practice 3: Transaction Classification
// =============================================================
// For each operation below, classify it as:
//   I = Implicit transaction sufficient (one SaveChangesAsync — already atomic —
//       and nothing that can fail runs after it)
//   E = Explicit transaction required (work continues AFTER a save and must be undone
//       if that later work fails. The classic case is a non-transactional EXTERNAL
//       call — e.g. a payment gateway — sitting BETWEEN two database writes, but a
//       SINGLE save also qualifies when a step that can fail comes after it.)
//   N = No transaction needed (read-only)
//
// Write your answer next to each operation.
// =============================================================

namespace Day26Practice.Exercises;

/// <summary>
/// Classify each operation. This file is for analysis only.
/// </summary>
public static class TransactionClassification
{
    //  #  | Operation                                              | Answer
    // ----|--------------------------------------------------------|-------
    //  1  | Read a list of products for display                    | ___
    //  2  | Create a single new product                            | ___
    //  3  | Transfer stock from Warehouse A to Warehouse B         | ___
    //     |   (deduct from A, add to B)                            |
    //  4  | Generate a monthly sales report                        | ___
    //  5  | Save a new order together with its order items         | ___
    //     |   (parent + children) in one call                      |
    //  6  | Update a product's price                               | ___
    //  7  | Batch import 500 products from a CSV file              | ___
    //  8  | Delete an expired shopping cart                        | ___
    //  9  | Checkout: deduct inventory and save, call the external | ___
    //     |   payment gateway, then record the order and save      |
    // 10  | Read a user's profile for display                      | ___

    // REFLECTION:
    // Which operations LOOK like they need an explicit transaction but do not? Why?
    //
    //
}
