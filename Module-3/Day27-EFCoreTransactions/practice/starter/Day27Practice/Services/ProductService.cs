using Day27Practice.Data;
using Day27Practice.Models;

namespace Day27Practice.Services;

public class ProductService(StoreDbContext context)
{
    private readonly StoreDbContext _context = context;

    /// <summary>
    /// [PRE-BUILT] Implicit transaction: single SaveChangesAsync wraps both inserts.
    /// </summary>
    public async Task<string> AddProductWithAuditAsync(string name, decimal price, int stock)
    {
        var product = new Product { Name = name, Price = price, StockQuantity = stock };
        _context.Products.Add(product);

        var audit = new StockAuditLog
        {
            Action = "Created",
            ProductName = name,
            Quantity = stock,
            Timestamp = DateTime.UtcNow
        };
        _context.StockAuditLogs.Add(audit);

        await _context.SaveChangesAsync();
        return $"Product '{name}' added with audit log";
    }

    /// <summary>
    /// [PRE-BUILT] Broken: the SAME two inserts artificially split into two SaveChangesAsync
    /// calls, with a failure between them. The first save commits on its own, so the second
    /// never running leaves a product with no audit row. The fix is NOT an explicit transaction
    /// — it is the single-save version above (one SaveChangesAsync is already atomic for both).
    /// Explicit transactions earn their keep only when something non-transactional (an external
    /// call) genuinely forces the writes apart, which is the Day 27 checkout scenario.
    /// </summary>
    public async Task<string> AddProductWithBrokenAuditAsync(string name, decimal price, int stock)
    {
        var product = new Product { Name = name, Price = price, StockQuantity = stock };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        throw new InvalidOperationException("Audit service unavailable!");

        // Never reached:
        // var audit = new StockAuditLog { ... };
        // _context.StockAuditLogs.Add(audit);
        // await _context.SaveChangesAsync();
    }
}
