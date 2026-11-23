using BYSResults;
using Day32Practice.Models;

namespace Day32Practice.Services;

// Interface extracted for DI best practices — inject IRestockService, not RestockService
public interface IRestockService
{
    Task<Result<Product>> RestockProductAsync(int productId, int quantity);
}
