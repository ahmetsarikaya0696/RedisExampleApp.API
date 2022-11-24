using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;

namespace RedisExampleApp.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product> AddAsync(Product product)
        {
            return _productRepository.AddAsync(product);
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return _productRepository.GetProductByIdAsync(id);
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return _productRepository.GetProductsAsync();
        }
    }
}
