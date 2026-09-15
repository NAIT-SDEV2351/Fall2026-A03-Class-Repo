namespace Day05Demo.Services;

// The service contract — the seam the effect depends on. The effect injects IProductService
// (the interface), not MockProductService (the class), so the real data source can be
// swapped in later without touching the store. Async because real data access is I/O-bound;
// the Async suffix is the course standard for any awaitable method.
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
}
