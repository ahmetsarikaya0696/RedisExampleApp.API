using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;
using RedisExampleApp.API.Services;
using StackExchange.Redis;

namespace RedisExampleApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(Product product)
        {
            return Created(string.Empty, await _productService.AddAsync(product));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            return Ok(await _productService.GetProductByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(int id)
        {
            return Ok(await _productService.GetProductsAsync());
        }
    }
}
