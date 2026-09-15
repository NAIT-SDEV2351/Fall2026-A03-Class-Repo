namespace Day05Practice.Services;

public class MockProductService : IProductService
{
    public async Task<List<Product>> GetAllAsync()
    {
        // Simulate network latency (800ms delay)
        await Task.Delay(800);

        // To test the error state (Practice 2 and Practice 3), replace the return below with:
        //   throw new InvalidOperationException("Database connection failed");
        // Your effect's catch turns that into a LoadProductsFailureAction and the pre-built
        // error alert appears. Put the return back when you are done.

        return
        [
            new Product(1, "Wireless Mouse", 29.99m, "Peripherals"),
            new Product(2, "USB-C Hub", 49.99m, "Accessories"),
            new Product(3, "Mechanical Keyboard", 89.99m, "Peripherals"),
            new Product(4, "27\" Monitor", 349.99m, "Displays"),
            new Product(5, "Webcam HD", 79.99m, "Accessories")
        ];
    }
}
