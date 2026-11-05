// =============================================================
// Practice 3: Transaction Classification — ANSWER KEY
// =============================================================

namespace Day26Practice.Exercises;

public static class TransactionClassificationAnswers
{
    //  #  | Operation                                              | Answer | Reason
    // ----|--------------------------------------------------------|--------|------------------------------------------
    //  1  | Read a list of products for display                    |   N    | Read-only — no writes, no transaction needed
    //  2  | Create a single new product                            |   I    | One entity, one SaveChangesAsync
    //  3  | Transfer stock: deduct A, add B                        |   I    | Two updates, one context → one atomic save, and nothing that can fail runs after it. No later step to undo, so no explicit transaction.
    //  4  | Generate a monthly sales report                        |   N    | Read-only aggregation
    //  5  | Save order + its order items (parent + children)       |   I    | One save; EF Core relationship fixup sets the generated OrderId on the children. Multi-entity is NOT a reason to split.
    //  6  | Update a product's price                               |   I    | One entity, one save
    //  7  | Batch import 500 products from CSV                      |   I    | One SaveChangesAsync inserts all 500 in a single atomic transaction. (Chunking into multiple saves is a performance choice that deliberately trades atomicity — not a correctness need.)
    //  8  | Delete an expired shopping cart                        |   I    | One delete operation, one save
    //  9  | Checkout: deduct + save, external payment, record + save |  E   | A non-transactional EXTERNAL call (the payment gateway) sits between two saves. The gateway cannot enrol in the DB transaction, so the explicit wrapper makes both saves all-or-nothing.
    // 10  | Read a user's profile for display                      |   N    | Read-only

    // Teaching note:
    // #3, #5, and #7 LOOK like they need an explicit transaction (multiple writes,
    // multiple entities, bulk) but do NOT — each collapses to one atomic SaveChangesAsync
    // with nothing after it that can fail.
    // #9 is the only E in this set: the external payment call forces two separate saves that
    // must both commit or both roll back.
    // The rule is NOT "multiple writes -> explicit", and it is NOT "more than one save ->
    // explicit" either. The criterion is the FAILURE POINT: a step that can fail comes AFTER
    // a save, so that save has to be undoable. An external call between writes is the classic
    // instance of that, not the definition of it. This matters because a SINGLE save also
    // needs a boundary when a step that can fail follows it — inside a boundary the save
    // writes but does not commit, so RollbackAsync() can still take it back. Students meet
    // exactly that shape on Day 29, and Lab 3 grades it.
}
