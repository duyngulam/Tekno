using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tekno.Domain.Auth;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Seeding
{
    /// <summary>
    /// Service for seeding training users only
    /// </summary>
    public class TrainingUserSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrainingUserSeeder> _logger;

        public TrainingUserSeeder(
            AppDbContext context,
            ILogger<TrainingUserSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seed 30 training users
        /// </summary>
        public async Task<SeedResult> SeedAsync()
        {
            var result = new SeedResult();

            try
            {
                _logger.LogInformation("Starting training user seeding...");

                // Seed training users
                var usersCreated = await SeedTrainingUsersAsync();
                result.UsersCreated = usersCreated;
                _logger.LogInformation("Created {Count} training users", usersCreated);

                result.Success = true;
                result.Message = "Seeding completed successfully";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed training data");
                result.Success = false;
                result.Message = $"Failed: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Create 30 training users (training1@tekno.com - training30@tekno.com)
        /// Password: training1 - training30
        /// </summary>
        private async Task<int> SeedTrainingUsersAsync()
        {
            var usersCreated = 0;

            for (int i = 1; i <= 5000; i++)
            {
                var email = $"training{i}@tekno.com";
                var password = $"training{i}";

                // Check if user exists
                var existingUser = await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    if (!existingUser.PasswordHash.StartsWith("$2", StringComparison.Ordinal))
                    {
                        existingUser.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(password));
                        _logger.LogInformation("User {Email} already existed with a legacy hash and was migrated", email);
                    }
                    else
                    {
                        _logger.LogInformation("User {Email} already exists, skipping", email);
                    }

                    continue;
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Create user (roleId = 2 for Customer)
                var user = new User(email, passwordHash, roleId: 2);
                user.UpdateProfile($"Training User {i}", $"0{900000000 + i}");

                _context.Set<User>().Add(user);
                usersCreated++;

                _logger.LogInformation("Created training user: {Email}", email);
            }

            await _context.SaveChangesAsync();
            return usersCreated;
        }

    }

    /// <summary>
    /// Seed result summary
    /// </summary>
    public class SeedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UsersCreated { get; set; }

        public override string ToString()
        {
            return $"Success: {Success}, Message: {Message}, Users: {UsersCreated}";
        }
    }
}
