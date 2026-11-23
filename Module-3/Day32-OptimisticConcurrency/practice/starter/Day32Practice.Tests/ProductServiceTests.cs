using Microsoft.EntityFrameworkCore;
using Day32Practice.Models;
using Day32Practice.Services;

namespace Day32Practice.Tests;

/// <summary>
/// Practice 3: this is your file. Write the two tests below.
///
/// Use _factory (SqliteDbFactory) for both. Do NOT switch to InMemoryDbFactory —
/// it runs no SQL, so Test 2 would never exercise the AND RowVersion check,
/// and it refuses transactions.
///
/// Run RestockServiceTests first to confirm the harness works.
/// </summary>
public class ProductServiceTests : IDisposable
{
    private readonly SqliteDbFactory _factory;

    public ProductServiceTests()
    {
        _factory = new SqliteDbFactory();
    }

    [Fact]
    public async Task UpdatePrice_NoConflict_UpdatesRowVersion()
    {
        throw new NotImplementedException("Complete this test. See the practice walkthrough.");
    }

    [Fact]
    public async Task UpdatePrice_ConcurrentEdit_ReturnsConflictError()
    {
        throw new NotImplementedException("Complete this test. See the practice walkthrough.");
    }

    public void Dispose() => _factory.Dispose();
}
