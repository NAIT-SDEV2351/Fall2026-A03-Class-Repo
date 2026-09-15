namespace Day05Demo.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
}
