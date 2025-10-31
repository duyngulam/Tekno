using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Interfaces;
using Tekno.Infrastructure.Auth;
using Tekno.Infrastructure.Catalog;
using Tekno.Infrastructure.Logging;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Services;
using Tekno.Infrastructure.Search;

namespace Tekno.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // ===================================================
            // 1️⃣ DATABASE (PostgreSQL)
            // ===================================================
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            // ===================================================
            // 2️⃣ REDIS CACHE
            // ===================================================
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:ConnectionString"];
                options.InstanceName = config["Redis:InstanceName"];
            });

            // ===================================================
            // 3️⃣ ELASTICSEARCH
            // ===================================================
            services.AddSingleton<IElasticClient>(sp =>
            {
                var uri = config["ElasticSearch:Uri"] ?? "http://localhost:9200";
                var settings = new ConnectionSettings(new Uri(uri))
                    .DefaultIndex(config["ElasticSearch:IndexName"] ?? "products");

                return new ElasticClient(settings);
            });

            // ===================================================
            // 4️⃣ INFRASTRUCTURE SERVICES
            // ===================================================
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddScoped<ICacheService, RedisCacheService>();


            // ===================================================
            // 5️⃣ REPOSITORIES
            // ===================================================
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            // ===================================================
            // 6️⃣ ELASTIC PRODUCT SERVICE
            // ===================================================
            services.AddScoped<IElasticProductService, ElasticProductService>();
            services.AddScoped<ElasticBulkIndexer>();

            return services;
        }
    }
}
