using Microsoft.EntityFrameworkCore;
using Day26Demo.Models;

namespace Day26Demo.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed a known starting inventory so the demo has real stock to corrupt — the
        // broken checkout deducts against these quantities, then the Inventory page shows
        // the damage. HasData makes the seed part of the model, applied on EnsureCreated.
        // Order.ProductId is a real foreign key: an Order cannot reference a Product
        // that does not exist. Restrict stops a Product delete from cascading away the
        // order history that references it.
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 10 },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, StockQuantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, StockQuantity = 30 },
            new Product { Id = 4, Name = "Monitor", Price = 449.99m, StockQuantity = 15 },
            new Product { Id = 5, Name = "Headset", Price = 149.99m, StockQuantity = 25 }
        );
    }
}
