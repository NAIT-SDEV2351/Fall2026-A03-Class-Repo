using Microsoft.EntityFrameworkCore;
using Day32Practice.Data;

namespace Day32Practice.Tests;

/// <summary>
/// [PRE-BUILT] Creates a StoreDbContext backed by the EF Core InMemory provider.
///
/// Use this factory ONLY for tests that never touch a transaction or a concurrency token —
/// input validation, BYSResult error checks, and similar.
///
/// WARNING: the InMemory provider is not a database. It runs no SQL, so a concurrency
/// test here never exercises the AND RowVersion check your app depends on, even though
/// the provider does raise DbUpdateConcurrencyException on a token mismatch.
/// It also refuses BeginTransactionAsync: the call throws InvalidOperationException.
///
/// For concurrency and transaction tests, use <see cref="SqliteDbFactory"/> instead.
/// </summary>
public sealed class InMemoryDbFactory : IDisposable
{
    // A unique name per factory keeps each test class isolated from the others.
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public StoreDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        var context = new StoreDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose() { }
}
