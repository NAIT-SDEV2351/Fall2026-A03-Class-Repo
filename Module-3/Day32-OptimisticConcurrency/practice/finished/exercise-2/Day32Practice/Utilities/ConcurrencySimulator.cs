using Microsoft.EntityFrameworkCore;
using Day32Practice.Data;

namespace Day32Practice.Utilities;

/// <summary>
/// [PRE-BUILT] Creates a second DbContext to simulate a concurrent user.
/// Use _factory.CreateDbContext() in your exercises to get a separate context.
///
/// REFERENCE ONLY: this class is not injected or called anywhere in the practice
/// project. Read it to see the two-context shape, then write that shape yourself
/// inside ProductService.SimulateConflictAsync using its own _factory field.
/// </summary>
public class ConcurrencySimulator(IDbContextFactory<StoreDbContext> factory)
{
    private readonly IDbContextFactory<StoreDbContext> _factory = factory;

    /// <summary>
    /// Creates a new DbContext instance with its own change tracker.
    /// Use this to simulate a second user in your exercises.
    /// </summary>
    public StoreDbContext CreateSecondUserContext()
    {
        return _factory.CreateDbContext();
    }
}
