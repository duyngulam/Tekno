using Microsoft.EntityFrameworkCore;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Auth.Services;
using Tekno.Application.Common.Interfaces;
using Tekno.Infrastructure.Auth;
using Tekno.Infrastructure.Logging;
using Tekno.Infrastructure.Persistence;
namespace Tekno.Api

{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var hash = BCrypt.Net.BCrypt.HashPassword("admin123");
            //var hash2 = BCrypt.Net.BCrypt.HashPassword("customer123");
            //using var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddConsole();
            //});

            //ILogger logger = loggerFactory.CreateLogger<Program>();

            //logger.LogInformation("Generated hash: {Hash}", hash);      
            //logger.LogInformation("Generated hash: {Hash2}", hash2);

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register Auth module dependencies
            builder.Services.AddScoped<AuthService>(); // Auth business logic
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));


            //configure db context
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            //configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // Apply pending migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var retries = 10;
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
