using RedisExampleApp.API.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisExampleApp.API.Repositories
{
    public class ProductRepositoryWithCache : IProductRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly IDatabase _redisDb;
        private const string key = "productsCache";

        public ProductRepositoryWithCache(IProductRepository productRepository, IDatabase redisDb)
        {
            _productRepository = productRepository;
            _redisDb = redisDb;
        }

        public async Task<Product> AddAsync(Product product)
        {
            var newProduct = await _productRepository.AddAsync(product);

            if (!await _redisDb.KeyExistsAsync(key)) await CacheProductsAsync();

            bool isSuccesfull = await _redisDb.HashSetAsync(key, product.Id, JsonSerializer.Serialize(newProduct));
            return isSuccesfull ? newProduct : null;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            if (!await _redisDb.KeyExistsAsync(key)) await CacheProductsAsync();

            var product = await _redisDb.HashGetAsync(key, id);
            return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            if (!await _redisDb.KeyExistsAsync(key)) return await CacheProductsAsync();

            List<Product> cachedProducts = new();
            (await _redisDb.HashGetAllAsync(key)).ToList().ForEach(x =>
            {
                cachedProducts.Add(JsonSerializer.Deserialize<Product>(x.Value));
            });
            return cachedProducts;
        }

        private async Task<List<Product>> CacheProductsAsync()
        {
            var products = await _productRepository.GetProductsAsync();
            products.ForEach(async p =>
            {
                await _redisDb.HashSetAsync(key, p.Id, JsonSerializer.Serialize(p));
            });

            return products;
        }
    }
}
