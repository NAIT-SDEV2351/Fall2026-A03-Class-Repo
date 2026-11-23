using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Day32Practice.Data;

namespace Day32Practice.Tests;

/// <summary>
/// [PRE-BUILT] Creates a StoreDbContext backed by a real SQLite database held in memory.
///
/// Use this factory for anything that needs real database behaviour:
/// transactions (BeginTransactionAsync) and concurrency tokens (RowVersion).
///
/// Why a field, not a fresh connection each time: an in-memory SQLite database lives only
/// as long as its connection is open. The factory opens ONE connection and keeps it open,
/// so every context it hands out sees the same database. Close the connection and the
/// database disappears — that is what Dispose does at the end of each test class.
/// </summary>
public sealed class SqliteDbFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteDbFactory()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // Create the schema once. This also applies the HasData seed products (Ids 1-5).
        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    /// <summary>
    /// Returns a new StoreDbContext over the same in-memory database.
    /// Call this twice to get two independent contexts — that is how you simulate two users.
    /// </summary>
    public StoreDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new StoreDbContext(options);
    }

    public void Dispose() => _connection.Dispose();
}
