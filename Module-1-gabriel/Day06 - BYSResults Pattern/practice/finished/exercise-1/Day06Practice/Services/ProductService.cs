namespace Day06Practice.Services;

/// <summary>
/// Practice 2: Refactor this service to use BYSResult instead of exceptions.
///
/// For each method:
/// 1. Change the return type from Task<Product> to Task<Result<Product>>
/// 2. Create a Result<Product> at the start: var result = new Result<Product>();
/// 3. Replace each 'throw' with result.AddError(new Error("Category", "Message"))
/// 4. Return result (on failure) or result.WithValue(product) (on success)
/// 5. For AddAsync, accumulate BOTH errors before returning (don't return after the first)
/// 6. UpdateAsync is take-home -- do GetByIdAsync and AddAsync first.
///
/// Don't forget: using BYSResults; at the top of the file.
/// </summary>
public class ProductService : IProductService
{
    // Simulated in-memory data store
    private readonly List<Product> _products =
    [
        new Product(1, "Wireless Mouse", 29.99m),
        new Product(2, "USB-C Hub", 49.99m),
        new Product(3, "Mechanical Keyboard", 89.99m)
    ];

    public async Task<Product> GetByIdAsync(int id)
    {
        await Task.Delay(100); // Simulate async operation

        // TODO (Practice 2): Replace this validation with Result<T>
        // Change return type to Task<Result<Product>>
        // Replace 'throw' with result.AddError(new Error(...))
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than zero");

        var product = _products.FirstOrDefault(p => p.Id == id);

        // TODO (Practice 2): Replace this with result.AddError + return result
        if (product is null)
            throw new InvalidOperationException($"Product with ID {id} not found");

        // TODO (Practice 2): Replace with return result.WithValue(product)
        return product;
    }

    public async Task<Product> AddAsync(Product product)
    {
        await Task.Delay(100);

        // TODO (Practice 2): Replace these throws with result.AddError()
        // IMPORTANT: Accumulate BOTH errors before returning.
        // Do NOT return after the first error -- collect them all.
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required");

        if (product.Price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        var newProduct = product with { Id = _products.Max(p => p.Id) + 1 };
        _products.Add(newProduct);

        // TODO (Practice 2): Replace with return result.WithValue(newProduct)
        return newProduct;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        await Task.Delay(100);

        // TODO (Practice 2 -- OPTIONAL, take-home): Replace these throws with result.AddError()
        if (product.Id <= 0)
            throw new ArgumentException("Product ID must be greater than zero");

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required");

        var existing = _products.FirstOrDefault(p => p.Id == product.Id);

        if (existing is null)
            throw new InvalidOperationException($"Product with ID {product.Id} not found");

        _products.Remove(existing);
        _products.Add(product);

        // TODO (Practice 2 -- OPTIONAL, take-home): Replace with return result.WithValue(product)
        return product;
    }
}
