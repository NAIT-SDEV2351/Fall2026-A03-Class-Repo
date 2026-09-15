namespace Day05Demo.Services;

public class MockProductService : IProductService
{
    public async Task<List<Product>> GetAllAsync()
    {
        // Simulate network latency (1 second delay) so the loading spinner is visible
        await Task.Delay(1000);

        //   throw new InvalidOperationException("Database connection failed");
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
