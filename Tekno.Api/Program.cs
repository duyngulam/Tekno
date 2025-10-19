using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Auth.Services;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Cloudinary.Services;
using Tekno.Application.Common.Interfaces;
using Tekno.Infrastructure.Auth;
using Tekno.Infrastructure.Logging;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Services;
using Tekno.Application.Catalog.Interface;
using Tekno.Infrastructure.Catalog;

namespace Tekno.Api
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddSwaggerGen();
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
            builder.Services.AddScoped<AuthService>(); // Business logic Auth
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
            builder.Services.AddScoped<MediaService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<BrandService>();
            // =======================================================
            // 5. DATABASE
            // =======================================================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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

            // =======================================================
            // 8. MIDDLEWARE PIPELINE
            // =======================================================
            // 1. Request logging (incoming)
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 2. Error handling (catch exceptions thrown by lower middleware/controllers)
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            // 3. Authentication & Authorization (use built-in)
            app.UseAuthentication();
            app.UseAuthorization();

            // 4. Response wrapper (should be after controller executed)
            app.UseMiddleware<ResponseWrapperMiddleware>();
            app.MapControllers();

            // =======================================================
            // 9. RUN APP
            // =======================================================
            app.Run();
        }
    }
}
