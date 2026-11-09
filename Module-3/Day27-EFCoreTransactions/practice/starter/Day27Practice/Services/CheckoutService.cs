using Day27Practice.Data;
using Day27Practice.Models;
using BYSResults;
using Microsoft.EntityFrameworkCore;

namespace Day27Practice.Services;

public class CheckoutService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// Practice 2: Wrap a multi-step checkout in an explicit transaction. Every change —
    /// the inventory deduction, the Order, and its OrderItem — goes to the database in ONE
    /// SaveChangesAsync, and CommitAsync is what makes it permanent. The new mechanic today
    /// is the navigation property: set orderItem.Order = order and EF Core fills in the
    /// generated OrderId during that single save. A generated ID is never a reason to save twice.
    /// </summary>
    public async Task<Result<string>> CheckoutAsync(int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }

    /// <summary>
    /// Practice 3: Copy your CheckoutAsync, then throw an exception AFTER the SaveChangesAsync
    /// but BEFORE the CommitAsync. The save has written rows to the database, but the open
    /// transaction means they are not permanent yet — so RollbackAsync undoes them. Verify the
    /// inventory is unchanged and no orphaned Order or OrderItem exists.
    /// </summary>
    public async Task<Result<string>> FailingCheckoutAsync(int productId, int quantity, string customer)
    {
        throw new NotImplementedException("Complete this method. See the practice walkthrough.");
    }
}
