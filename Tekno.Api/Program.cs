using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tekno.Api.Services;
using Nest;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Tekno.Api.Filters;
using Tekno.Api.Middlewares;
using Tekno.Application.Auth.DTOs;
using Tekno.Infrastructure;
using Tekno.Infrastructure.Persistence;
using Tekno.Infrastructure.Search;
using System.IO;
using System.Linq;

namespace Tekno.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load .env file into environment variables (if present)
            LoadDotEnv();

            var builder = WebApplication.CreateBuilder(args);

            // =======================================================
            // 1. CONFIGURATION
            // =======================================================
            var configuration = builder.Configuration;

            // If DB connection string is provided via environment (e.g. .env or docker), prefer it and override configuration
            var envConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(envConn))
            {
                // Override the configuration value so later services read it from Configuration
                builder.Configuration["ConnectionStrings:DefaultConnection"] = envConn;
            }

            // Override JWT settings from environment variables if present
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? Environment.GetEnvironmentVariable("JWT_SECRET");
            if (!string.IsNullOrWhiteSpace(jwtSecret))
            {
                builder.Configuration["JwtSettings:SecretKey"] = jwtSecret;
            }

            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            if (!string.IsNullOrWhiteSpace(jwtIssuer))
            {
                builder.Configuration["JwtSettings:Issuer"] = jwtIssuer;
            }

            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            if (!string.IsNullOrWhiteSpace(jwtAudience))
            {
                builder.Configuration["JwtSettings:Audience"] = jwtAudience;
            }

            var jwtExpiry = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? Environment.GetEnvironmentVariable("JWT_EXPIRY");
            if (!string.IsNullOrWhiteSpace(jwtExpiry))
            {
                builder.Configuration["JwtSettings:ExpiryMinutes"] = jwtExpiry;
            }

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
            // DO NOT add JsonStringEnumConverter - we want numeric enum values for clarity
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Allow filter to work
            });

            // Add HttpContextAccessor for accessing current request context
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();
            
            // Register RecommendationClient and configure base address from env
            builder.Services.AddHttpClient<RecommendationClient>((sp, client) =>
            {
                var cfg = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                var baseUrl = cfg["TRAINING_API_URL"] ?? "http://trainning_api:8000";
                client.BaseAddress = new Uri(baseUrl);
            });

            // Swagger configuration with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Tekno API", 
                    Version = "v1",
                    Description = @"
## 🚀 Tekno E-Commerce API

Complete REST API for e-commerce platform with:
- 🔐 JWT Authentication & Authorization
- 🛒 Cart & Wishlist Management
- 💳 Payment Processing (Mock, Stripe, VNPay)
- 📦 Product Catalog & Search
- ⭐ Reviews & Ratings
- 📊 Admin Statistics & Reports
- 🎯 Promotions & Coupons

### Quick Start
1. **Register**: POST `/api/auth/register`
2. **Login**: POST `/api/auth/login` to get JWT token
3. **Authorize**: Click 🔓 button and enter: `Bearer YOUR_TOKEN`
4. **Browse Products**: GET `/api/products`
5. **Add to Cart**: POST `/api/cart/items`
6. **Checkout**: POST `/api/payment/process`

### Payment Flow
```
1. Add items to cart
2. POST /api/payment/process (creates order & initiates payment)
3. Redirect to payment gateway
4. Gateway calls POST /api/payment/callback (webhook)
5. GET /api/payment/status/{transactionId} (check status)
```

### Admin Endpoints
Admin endpoints require `Admin` role. Test accounts:
- Admin: `admin@tekno.com` / `Admin123!`
- User: `john.doe@tekno.com` / `User123!`

### Environment
- Base URL: `https://localhost:7145`
- Database: PostgreSQL
- Cache: Redis
- Search: Elasticsearch
",
                    Contact = new OpenApiContact
                    {
                        Name = "Tekno Support",
                        Email = "support@tekno.com",
                        Url = new Uri("https://github.com/duyngulam/Tekno")
                    }
                });

                // Configure enums to serialize as integers in Swagger
                c.UseInlineDefinitionsForEnums(); // This makes Swagger show enum values

                // Add schema filter for examples
                c.SchemaFilter<Tekno.Api.Filters.SwaggerSchemaExampleFilter>();

                // Enable XML comments for all projects
                var xmlFiles = new[]
                {
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml", // Tekno.Api
                    "Tekno.Application.xml",
                    "Tekno.Domain.xml"
                };

                foreach (var xmlFile in xmlFiles)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                    }
                }

                // Group endpoints by tags
                c.TagActionsBy(api =>
                {
                    // If a GroupName is set (via ApiExplorerSettings), use it but map Admin* groups to the common 'Admin' tag
                    if (!string.IsNullOrEmpty(api.GroupName))
                    {
                        if (api.GroupName.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
                            return new[] { "Admin" };

                        return new[] { api.GroupName };
                    }

                    var controllerName = api.ActionDescriptor.RouteValues["controller"];

                    // If controller name starts with 'Admin' (e.g., AdminProductController),
                    // group it under the general 'Admin' tag so admin endpoints are grouped together.
                    if (!string.IsNullOrEmpty(controllerName) && controllerName.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return new[] { "Admin" };
                    }

                    return new[] { controllerName ?? "Unknown" };
                });

                // Sort actions alphabetically within each group
                c.OrderActionsBy(api => api.RelativePath);

                // JWT configuration in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                    
Enter your token in the text input below.

Example: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Don't include 'Bearer' prefix** - it's added automatically.",
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

                // Add operation filters for better documentation
                c.OperationFilter<SwaggerDefaultValues>();
                c.OperationFilter<ProductSearchOperationFilter>();
                c.DocumentFilter<SwaggerTagDescriptions>();
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

            // HTTP client factory for background services
            builder.Services.AddHttpClient();
            
            // Register background service to fetch provinces and store locally
            builder.Services.AddHostedService<Tekno.Api.Services.Hosted.ProvinceFetchBackgroundService>();
            
            // Register background services for auto-expiration
            builder.Services.AddHostedService<Tekno.Infrastructure.BackgroundServices.CouponExpirationBackgroundService>();
            builder.Services.AddHostedService<Tekno.Infrastructure.BackgroundServices.PromotionManagementBackgroundService>();

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
            //using (var scope = app.Services.CreateScope())
            //{
            //    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //    var retries = 5;
            //    for (var i = 0; i < retries; i++)
            //    {
            //        try
            //        {
            //            db.Database.Migrate();
            //            logger.LogInformation("Database migration completed successfully");
            //            break;
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.LogWarning(ex, "Migration attempt {Attempt} failed, retrying...", i + 1);
            //            if (i == retries - 1)
            //            {
            //                logger.LogError(ex, "Database migration failed after {Retries} attempts", retries);
            //                throw;
            //            }
            //            Thread.Sleep(2000);
            //        }
            //    }
            //}

            // =======================================================
            // 7. INITIALIZE ELASTICSEARCH
            // =======================================================
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    // Wait for Elasticsearch to be ready
                    var client = scope.ServiceProvider.GetRequiredService<IElasticClient>();
                    
                    // Retry ping to ensure ES is ready
                    var retries = 10;
                    var esReady = false;
                    
                    for (int i = 0; i < retries; i++)
                    {
                        try
                        {
                            var pingResponse = await client.PingAsync();
                            if (pingResponse.IsValid)
                            {
                                logger.LogInformation("Elasticsearch is ready");
                                esReady = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning("Elasticsearch not ready (attempt {Attempt}/{Max}): {Error}", 
                                i + 1, retries, ex.Message);
                        }
                        
                        await Task.Delay(2000);
                    }
                    
                    if (!esReady)
                    {
                        logger.LogWarning("Elasticsearch failed to become ready after {Retries} attempts - continuing without search", retries);
                        return;
                    }
                    
                    // Create Elasticsearch indices if not exist
                    ElasticMappings.CreateProductIndex(client);
                    ElasticMappings.CreateProductDetailIndex(client);

                    // Run bulk indexing only if products exist
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var hasProducts = await db.Products.AnyAsync();
                    
                    if (hasProducts)
                    {
                        var bulkIndexer = scope.ServiceProvider.GetRequiredService<ElasticBulkIndexer>();
                        await bulkIndexer.RunAsync();
                        logger.LogInformation("Elasticsearch bulk indexing completed successfully");
                    }
                    else
                    {
                        logger.LogInformation("No products to index, skipping bulk indexing");
                    }
                    
                    logger.LogInformation("Elasticsearch initialization completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Elasticsearch initialization failed - app will continue without search functionality");
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
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tekno API v1");
                    c.RoutePrefix = "swagger"; // Access at /swagger
                    
                    // Enhanced UI settings
                    c.DocumentTitle = "Tekno API Documentation";
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Collapse all by default
                    c.DefaultModelsExpandDepth(2); // Show model details
                    c.DisplayRequestDuration(); // Show request duration
                    c.EnableDeepLinking(); // Enable deep linking to operations
                    c.EnableFilter(); // Enable search/filter
                    c.ShowExtensions(); // Show vendor extensions
                    c.EnableValidator(); // Enable request validator
                    c.SupportedSubmitMethods(
                        Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
                        Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
                        Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
                        Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete,
                        Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Patch
                    );
                    
                    // Inject custom CSS for better styling
                    c.InjectStylesheet("/swagger-ui/custom.css");
                });
            }

            // 3️⃣ Exception handler - outermost to catch all errors
            app.UseMiddleware<ExceptionMiddleware>();

            // 4️⃣ Response wrapper - after exception handler, only runs on success
            app.UseMiddleware<ResponseWrapperMiddleware>();

            // 5️⃣ CORS and HTTPS redirect
            app.UseCors("AllowFrontend");
            
            // Enable static files for Swagger custom CSS
            app.UseStaticFiles();
            
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

        private static void LoadDotEnv()
        {
            try
            {
                var basePath = Directory.GetCurrentDirectory();
                var envPath = Path.Combine(basePath, ".env");
                if (!File.Exists(envPath))
                {
                    // If .env not found, walk up parent directories
                    var parentDir = Directory.GetParent(basePath);
                    while (parentDir != null)
                    {
                        envPath = Path.Combine(parentDir.FullName, ".env");
                        if (File.Exists(envPath))
                            break;

                        parentDir = parentDir.Parent;
                    }
                }

                if (!File.Exists(envPath))
                    return;

                foreach (var rawLine in File.ReadAllLines(envPath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var idx = line.IndexOf('=');
                    if (idx <= 0) continue;

                    var key = line.Substring(0, idx).Trim();
                    var val = line.Substring(idx + 1).Trim();

                    // Remove surrounding quotes if present
                    if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
                    {
                        val = val.Substring(1, val.Length - 2);
                    }

                    Environment.SetEnvironmentVariable(key, val);
                }
            }
            catch
            {
                // Ignore - loading .env is best-effort
            }
        }
    }
}
