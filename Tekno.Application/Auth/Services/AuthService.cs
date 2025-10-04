using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Auth;

namespace Tekno.Application.Auth.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IAppLogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            IAppLogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        // Login
        public async Task<AuthResponse?> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for {email}", email);
                return null;
            }

            var (token, expiresAt) = _jwtProvider.GenerateToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.Name,
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        // Register
        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepo.GetByEmailAsync(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Register failed: Username {email} already exists", request.Email);
                return null;
            }

            var hashedPassword = _passwordHasher.Hash(request.Password);

            var role = await _userRepo.GetRoleByNameAsync(request.Role)
                       ?? throw new Exception("Role not found");

            var newUser = new User(request.Username,request.Email, hashedPassword, role.Id);

            await _userRepo.AddAsync(newUser);

            var (token, expiresAt) = _jwtProvider.GenerateToken(newUser);

            _logger.LogInformation("New user registered: {Username}", request.Username);

            return new AuthResponse
            {
                Id = newUser.Id,
                
                Email = newUser.Email,
                Role = newUser.Role.Name,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
