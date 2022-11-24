using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> AddAsync(Product product);
    }
}
