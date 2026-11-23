using Day32Practice.Services;

namespace Day32Practice.Tests;

/// <summary>
/// [PRE-BUILT] Run this test FIRST, before you write anything in ProductServiceTests.
///
/// It has one job: prove the test harness works. If this test is green, then
/// SqliteDbFactory really did create a database, the schema really was built, the seed
/// products really are there, and an explicit transaction really can commit. When your own
/// tests then fail, you know the failure is in your test or in the service — not in the setup.
///
/// This is also the same test shape Lab 3 ships as its reference example
/// (tests/NorthwindTraders.Tests/ProductRestock/RestockServiceTests.cs). The pattern you
/// practice here is the pattern you are graded on there.
/// </summary>
public class RestockServiceTests : IDisposable
{
    private readonly SqliteDbFactory _factory;

    public RestockServiceTests()
    {
        _factory = new SqliteDbFactory();
    }

    [Fact]
    public async Task RestockProductAsync_WithValidQuantity_IncrementsStockAndCommits()
    {
        // Arrange — the seeded Laptop is Id 1 and starts with StockQuantity 10.
        using var context = _factory.CreateDbContext();
        var service = new RestockService(context);

        // Act
        var result = await service.RestockProductAsync(productId: 1, quantity: 25);

        // Assert — the service reported success
        Assert.True(result.IsSuccess);
        Assert.Equal(35, result.Value!.StockQuantity);

        // Assert — and the transaction really committed. Reading through a SECOND context
        // proves the new value is in the database, not just in the first context's change
        // tracker. This is the check that would fail if the commit never happened.
        using var verifyContext = _factory.CreateDbContext();
        var reloaded = await verifyContext.Products.FindAsync(1);
        Assert.Equal(35, reloaded!.StockQuantity);
    }

    public void Dispose() => _factory.Dispose();
}
