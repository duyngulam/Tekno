using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using System.Reflection;
using System.Text;
using Tekno.Api.Middlewares;
using Tekno.Application.Auth.DTOs;
using Tekno.Infrastructure;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Search;

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
            builder.Services.AddControllers(options =>
            {
                // Register validation filter globally
                options.Filters.Add<ValidationFilterAttribute>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Allow filter to work
            });

            builder.Services.AddEndpointsApiExplorer();
            
            // Swagger configuration with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tekno API", Version = "v1" });

                // JWT configuration in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT token (e.g., Bearer eyJhbGciOi...)",
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

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000") // Frontend domain
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            // AutoMapper configuration
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly()); // Scan profiles in API project
                cfg.AddMaps(typeof(AuthProfile).Assembly);     // Scan profiles in Application project
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
                options.RequireHttpsMetadata = false; // Can disable HTTPS in dev
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero // Prevent token expiry time drift
                };
            });

            builder.Services.AddAuthorization();

            // =======================================================
            // 4. INFRASTRUCTURE & APPLICATION DEPENDENCIES
            // =======================================================
            builder.Services.AddInfrastructure(builder.Configuration);

            // =======================================================
            // 5. LOGGING
            // =======================================================
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // =======================================================
            // 6. APPLY MIGRATIONS ON STARTUP
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
                        logger.LogInformation("Database migration completed successfully");
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Migration attempt {Attempt} failed, retrying...", i + 1);
                        if (i == retries - 1)
                        {
                            logger.LogError(ex, "Database migration failed after {Retries} attempts", retries);
                            throw;
                        }
                        Thread.Sleep(2000);
                    }
                }
            }

            // =======================================================
            // 7. INITIALIZE ELASTICSEARCH
            // =======================================================
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    // Create Elasticsearch indices if not exist
                    var client = scope.ServiceProvider.GetRequiredService<IElasticClient>();
                    ElasticMappings.CreateProductIndex(client);
                    ElasticMappings.CreateProductDetailIndex(client);

                    // Run bulk indexing
                    var bulkIndexer = scope.ServiceProvider.GetRequiredService<ElasticBulkIndexer>();
                    await bulkIndexer.RunAsync();
                    
                    logger.LogInformation("Elasticsearch initialization completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Elasticsearch initialization failed");
                    // Don't throw - app can still run without Elasticsearch
                }
            }

            // =======================================================
            // 8. MIDDLEWARE PIPELINE
            // =======================================================

            // 1️⃣ Request logging
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 2️⃣ Swagger - must be before response processing middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 3️⃣ Exception handler - outermost to catch all errors
            app.UseMiddleware<ExceptionMiddleware>();

            // 4️⃣ Response wrapper - after exception handler, only runs on success
            app.UseMiddleware<ResponseWrapperMiddleware>();

            // 5️⃣ CORS and HTTPS redirect
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            // 6️⃣ Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // 7️⃣ Map controllers (register routes)
            app.MapControllers();

            // =======================================================
            // 9. RUN APP
            // =======================================================
            app.Run();
        }
    }
}
