namespace Day05Practice.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
}
