using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> AddAsync(Product product);
    }
}
