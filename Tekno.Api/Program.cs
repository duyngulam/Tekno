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
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Allow filter to work
            });

            builder.Services.AddEndpointsApiExplorer();
            
            // Swagger configuration with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tekno API", Version = "v1" });

                // Add schema filter for examples
                c.SchemaFilter<Tekno.Api.Filters.SwaggerSchemaExampleFilter>();

                // Enable XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

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
