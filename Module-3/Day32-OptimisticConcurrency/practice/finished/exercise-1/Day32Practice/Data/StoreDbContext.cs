using Microsoft.EntityFrameworkCore;
using Day32Practice.Models;

namespace Day32Practice.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Practice 1: This configures RowVersion as a concurrency token.
        // EF Core will include it in the WHERE clause on updates.
        modelBuilder.Entity<Product>()
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 10 },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, StockQuantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, StockQuantity = 30 },
            new Product { Id = 4, Name = "Monitor", Price = 449.99m, StockQuantity = 15 },
            new Product { Id = 5, Name = "Headset", Price = 149.99m, StockQuantity = 25 }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Rotate the Guid on modified entities so EF Core detects version changes
        foreach (var entry in ChangeTracker.Entries<Product>()
            .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.RowVersion = Guid.NewGuid();
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
