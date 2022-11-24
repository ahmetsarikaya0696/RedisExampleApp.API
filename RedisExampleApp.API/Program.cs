
using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;
using RedisExampleApp.API.Services;
using RedisExampleApp.Cache;
using StackExchange.Redis;

namespace RedisExampleApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Decorator Design Pattern
            builder.Services.AddScoped<IProductRepository>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<AppDbContext>();
                var productRepository = new ProductRepository(context);
                var redisDb = serviceProvider.GetRequiredService<IDatabase>();
                return new ProductRepositoryWithCache(productRepository, redisDb);
            });

            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("RedisExampleAppDb"));

            // Redis
            var redisUrl = builder.Configuration["CacheOptions:Url"];
            builder.Services.AddSingleton<RedisService>(serviceProvider =>
            {
                return new RedisService(redisUrl);
            });

            // IDatabase
            builder.Services.AddSingleton<IDatabase>(serviceProvider =>
            {
                var redisService = serviceProvider.GetRequiredService<RedisService>();
                return redisService.GetDatabase();
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}