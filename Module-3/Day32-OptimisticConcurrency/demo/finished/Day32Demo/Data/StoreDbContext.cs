using Microsoft.EntityFrameworkCore;
using Day32Demo.Models;

namespace Day32Demo.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IsConcurrencyToken() tells EF to include RowVersion in the UPDATE/DELETE WHERE clause and
        // to compare it on save — that opt-in is what turns the column into a conflict detector.
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

    // The token only works if it CHANGES on every update. On SQL Server you'd use a real
    // `rowversion`/`timestamp` column the database auto-increments. SQLite has no such type, so we
    // simulate it here: just before saving, give every modified Product a fresh Guid. That new value
    // becomes the next reader's "original" — so a writer holding the old Guid is now stale and fails.
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Product>()
            .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.RowVersion = Guid.NewGuid();
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
