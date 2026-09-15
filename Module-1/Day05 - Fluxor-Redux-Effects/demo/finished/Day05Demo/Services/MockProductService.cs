namespace Day05Demo.Services;

// A mock implementation standing in for a real database/API call. Returning canned data
// keeps the demo's focus on the Fluxor async flow, not on data access — and lets us make
// the timing and the failure deterministic, which a real backend can't.
public class MockProductService : IProductService
{
    public async Task<List<Product>> GetAllAsync()
    {
        // Simulate network latency (1 second delay) so the loading spinner is actually
        // visible — instant data would flash past the IsLoading=true state and you'd never
        // see the reducer/effect loading lifecycle the day is teaching.
        await Task.Delay(1000);

        // To demonstrate the error path, replace the return below with:
        //   throw new InvalidOperationException("Database connection failed");
        // The effect's catch turns that into a LoadProductsFailureAction, and the page
        // swaps the spinner for the red error alert — the failure branch, end to end.
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
