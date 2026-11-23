using Microsoft.EntityFrameworkCore;
using Day32Practice.Models;

namespace Day32Practice.Tests;

/// <summary>
/// Practice 3 [COMPLETED]: the two concurrency tests.
///
/// Both use SqliteDbFactory. Swapping in InMemoryDbFactory would stop Test 2 exercising
/// the AND RowVersion check — the InMemory provider runs no SQL.
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
        // Arrange
        using var context = _factory.CreateDbContext();
        var product = new Product
        {
            Name = "Test Laptop",
            Price = 999.99m,
            StockQuantity = 10
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var originalRowVersion = product.RowVersion; // Capture the Guid

        // Act
        product.Price = 1099.99m;
        await context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(originalRowVersion, product.RowVersion);
    }

    [Fact]
    public async Task UpdatePrice_ConcurrentEdit_ReturnsConflictError()
    {
        // Arrange — seed product
        using var contextSetup = _factory.CreateDbContext();
        var product = new Product
        {
            Name = "Test Laptop",
            Price = 999.99m,
            StockQuantity = 10
        };
        contextSetup.Products.Add(product);
        await contextSetup.SaveChangesAsync();
        var productId = product.Id;

        // Arrange — two users read the same product
        using var contextA = _factory.CreateDbContext();
        using var contextB = _factory.CreateDbContext();
        var productA = await contextA.Products.FindAsync(productId);
        var productB = await contextB.Products.FindAsync(productId);

        // Act — User A saves first (succeeds)
        productA!.Price = 1099.99m;
        await contextA.SaveChangesAsync();

        // Act — User B saves (conflict!)
        productB!.Price = 899.99m;

        // Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => contextB.SaveChangesAsync());
    }

    public void Dispose() => _factory.Dispose();
}
