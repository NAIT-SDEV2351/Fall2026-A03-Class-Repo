using Microsoft.EntityFrameworkCore;
using Day34Demo.Models;

namespace Day34Demo.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Mouse", Price = 29.99m, StockQuantity = 25 },
            new Product { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m, StockQuantity = 15 },
            new Product { Id = 3, Name = "USB-C Hub", Price = 49.99m, StockQuantity = 40 },
            new Product { Id = 4, Name = "Monitor Stand", Price = 34.99m, StockQuantity = 30 },
            new Product { Id = 5, Name = "Webcam HD", Price = 79.99m, StockQuantity = 20 },
            new Product { Id = 6, Name = "Desk Lamp", Price = 24.99m, StockQuantity = 50 },
            new Product { Id = 7, Name = "Laptop Sleeve", Price = 19.99m, StockQuantity = 35 },
            new Product { Id = 8, Name = "Cable Organizer", Price = 14.99m, StockQuantity = 45 },
            new Product { Id = 9, Name = "Noise-Cancelling Headphones", Price = 149.99m, StockQuantity = 10 },
            new Product { Id = 10, Name = "Portable SSD 1TB", Price = 109.99m, StockQuantity = 5 }
        );
    }
}
