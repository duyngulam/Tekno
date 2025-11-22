using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using System.Reflection;
using System.Text;
using Tekno.Api.Middlewares;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Auth.Services;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Cart.Services;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Promotion.Interface;
using Tekno.Application.Promotion.Services;
using Tekno.Infrastructure;
using Tekno.Infrastructure.Auth;
using Tekno.Infrastructure.Cart;
using Tekno.Infrastructure.Catalog;
using Tekno.Infrastructure.Logging;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Promotion;
using Tekno.Infrastructure.Search;
using Tekno.Infrastructure.Services;

namespace Tekno.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =======================================================
            // 1. CONFIGURATION
            // =======================================================
            var configuration = builder.Configuration;
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT Secret missing");

            // =======================================================
            // 2. REGISTER FRAMEWORK SERVICES
            // =======================================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tekno API", Version = "v1" });

                // 🔑 cấu hình JWT trong Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Nhập token JWT (ví dụ: Bearer eyJhbGciOi...)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            }
                        },
                    Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000") // FE domain
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials(); // nếu dùng cookie-based token
                    });
            });
            builder.Services.AddControllers(options =>
            {
                // register validation filter globally
                options.Filters.Add<ValidationFilterAttribute>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // cho phép filter hoạt động
            });
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly()); // quét profile trong API project
                cfg.AddMaps(typeof(AuthProfile).Assembly); // quét profile trong Application project
            });

            // =======================================================
            // 3. AUTHENTICATION & AUTHORIZATION
            // =======================================================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Dev có thể tắt HTTPS
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Nếu có domain thật -> bật true
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero // Tránh token hết hạn lệch giờ
                };
            });

            builder.Services.AddAuthorization();

            // =======================================================
            // 4. APPLICATION & INFRASTRUCTURE DEPENDENCIES
            // =======================================================
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<BrandService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<MediaService>();
            
            // Coupon/Promotion services
            builder.Services.AddScoped<CouponService>();
            builder.Services.AddScoped<ICouponRepository,CouponRepository>();
            
            // Cart & Wishlist services
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<WishlistService>();
            builder.Services.AddScoped<ICartRepository,CartRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

            // =======================================================
            // 6. LOGGING
            // =======================================================
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // =======================================================
            // 7. APPLY MIGRATIONS ON STARTUP
            // =======================================================
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var retries = 5;
                for (var i = 0; i < retries; i++)
                {
                    try
                    {
                        db.Database.Migrate();
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Migration attempt {Attempt} failed, retrying...", i + 1);
                        Thread.Sleep(2000);
                    }
                }
            }
            using (var scope = app.Services.CreateScope())
            {
                // 1️⃣ Tạo index nếu chưa có
                var client = scope.ServiceProvider.GetRequiredService<IElasticClient>();
                ElasticMappings.CreateProductIndex(client);
                ElasticMappings.CreateProductDetailIndex(client);

                // 2️⃣ Chạy bulk index
                var bulkIndexer = scope.ServiceProvider.GetRequiredService<ElasticBulkIndexer>();
                await bulkIndexer.RunAsync();
            }

            // =======================================================
            // 8. MIDDLEWARE PIPELINE
            // =======================================================

            // 1️⃣ Logging request đầu vào
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 2️⃣ Swagger luôn nằm TRƯỚC các middleware xử lý response
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 3️⃣ Exception handler – nằm NGOÀI CÙNG để bắt tất cả lỗi
            app.UseMiddleware<ExceptionMiddleware>();

            // 4️⃣ Response wrapper – sau Exception, chỉ chạy khi response thành công
            app.UseMiddleware<ResponseWrapperMiddleware>();

            // 5️⃣ CORS và HTTPS redirect
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            // 6️⃣ Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // 7️⃣ Map controllers (đăng ký route)
            app.MapControllers();

            // =======================================================
            // 9. RUN APP
            // =======================================================
            app.Run();

        }
    }
}
