namespace Day06Practice.Services;

public record Product(int Id, string Name, decimal Price);

/// <summary>
/// Practice 2: Refactor this interface and its implementation to use Result<T>.
/// Currently, the service throws exceptions on validation failure.
/// After refactoring, methods should return Result<Product> instead of Task<Product>.
/// </summary>
public interface IProductService
{
    // TODO (Practice 2): Change return type to Task<Result<Product>>
    Task<Product> GetByIdAsync(int id);

    // TODO (Practice 2): Change return type to Task<Result<Product>>
    Task<Product> AddAsync(Product product);

    // TODO (Practice 2 -- OPTIONAL, take-home): Change return type to Task<Result<Product>>
    Task<Product> UpdateAsync(Product product);
}
