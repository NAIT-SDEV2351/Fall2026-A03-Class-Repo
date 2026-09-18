using BYSResults;

namespace Day06Practice.Services;

public record Product(int Id, string Name, decimal Price);

/// <summary>
/// Practice 2 COMPLETE: Interface refactored to return Result<T>.
/// </summary>
public interface IProductService
{
    Task<Result<Product>> GetByIdAsync(int id);
    Task<Result<Product>> AddAsync(Product product);
    Task<Result<Product>> UpdateAsync(Product product);
}
