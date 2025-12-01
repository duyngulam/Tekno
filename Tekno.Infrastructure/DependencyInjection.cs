using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Auth.Services;
using Tekno.Application.Blog.Interface;
using Tekno.Application.Blog.Services;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Cart.Services;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Order.Interface;
using Tekno.Application.Promotion.Interface;
using Tekno.Application.Promotion.Services;
using Tekno.Application.Review.Interface;
using Tekno.Application.Review.Services;
using Tekno.Infrastructure.Auth;
using Tekno.Infrastructure.Blog;
using Tekno.Infrastructure.Cart;
using Tekno.Infrastructure.Catalog;
using Tekno.Infrastructure.Logging;
using Tekno.Infrastructure.Order;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Promotion;
using Tekno.Infrastructure.Review;
using Tekno.Infrastructure.Search;
using Tekno.Infrastructure.Services;

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
                options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));

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
            // Auth
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Catalog
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
            
            // Promotion
            services.AddScoped<ICouponRepository, CouponRepository>();
            
            // Cart & Wishlist
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            
            // Review
            services.AddScoped<IReviewRepository, ReviewRepository>();
            
            // Order
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            // Blog
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();

            // ===================================================
            // 6️⃣ APPLICATION SERVICES
            // ===================================================
            // Auth & Profile
            services.AddScoped<AuthService>();
            services.AddScoped<ProfileService>();
            
            // Catalog
            services.AddScoped<CategoryService>();
            services.AddScoped<BrandService>();
            services.AddScoped<ProductService>();
            services.AddScoped<AdvertisementService>();
            
            // Media
            services.AddScoped<MediaService>();
            
            // Promotion
            services.AddScoped<CouponService>();
            
            // Cart & Wishlist
            services.AddScoped<CartService>();
            services.AddScoped<WishlistService>();
            
            // Review
            services.AddScoped<ReviewService>();
            
            // Blog
            services.AddScoped<BlogPostService>();

            // ===================================================
            // 7️⃣ ELASTICSEARCH SERVICES
            // ===================================================
            services.AddScoped<IElasticProductService, ElasticProductService>();
            services.AddScoped<ElasticBulkIndexer>();

            return services;
        }
    }
}
