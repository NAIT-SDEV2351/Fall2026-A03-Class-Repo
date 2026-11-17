using Microsoft.EntityFrameworkCore;
using Day30Practice.Models;

namespace Day30Practice.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Order has MANY OrderItems. This is the relationship .Include(o => o.Items)
        // walks. Cascade is correct here: an OrderItem has no meaning without its Order.
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem.ProductId is a real foreign key: a line cannot reference a Product
        // that does not exist. Restrict stops a Product delete from cascading away the
        // order history that references it.
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 10 },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, StockQuantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, StockQuantity = 30 },
            new Product { Id = 4, Name = "Monitor", Price = 449.99m, StockQuantity = 15 },
            new Product { Id = 5, Name = "Headset", Price = 149.99m, StockQuantity = 25 }
        );

        // One seeded order so the EDIT path has something real to load on first run.
        // Total = 999.99 + 79.99 + 29.99 = 1109.97
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                CustomerName = "Dana Whitfield",
                OrderDate = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc),
                Total = 1109.97m
            }
        );

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 999.99m, IsRemoved = false },
            new OrderItem { Id = 2, OrderId = 1, ProductId = 3, Quantity = 1, UnitPrice = 79.99m, IsRemoved = false },
            new OrderItem { Id = 3, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 29.99m, IsRemoved = false }
        );
    }
}
