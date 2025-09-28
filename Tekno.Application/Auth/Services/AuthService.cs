using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Domain.Auth;
using Tekno.Application.Common.Interfaces;
namespace Tekno.Application.Auth.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppLogger<AuthService> _logger;

        public AuthService(IUserRepository userRepo, IPasswordHasher passwordHasher, IAppLogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Username} not found", username);
                return null;
            }

            if (!_passwordHasher.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for {Username}", username);
                return null;
            }

            _logger.LogInformation("Login success: {Username} (Role: {Role})", user.Username, user.Role.Name);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Name
            };
        }
    }
}