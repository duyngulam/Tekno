using AutoMapper;
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
        private readonly IMapper _mapper;


        public AuthService(
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            IAppLogger<AuthService> logger,
            IMapper mapper)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _logger = logger;
            _mapper = mapper;
        }

        // Login
        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            {
                return null;
            }

            var (token, expiresAt) = _jwtProvider.GenerateToken(user);

            var dto = _mapper.Map<UserDto>(user);
            dto.Token = token;
            dto.ExpiresAt = expiresAt;

            return dto;
        }

        // Register
        public async Task<UserDto?> RegisterAsync(string email,string password,string rrole)
        {
            var existingUser = await _userRepo.GetByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("Register failed: Email {email} already exists", email);
                return null;
            }

            var hashedPassword = _passwordHasher.Hash(password);

            var role = await _userRepo.GetRoleByNameAsync(rrole)
                       ?? throw new Exception("Role not found");

            var newUser = new User(email, hashedPassword, role.Id);

            await _userRepo.AddAsync(newUser);

            var (token, expiresAt) = _jwtProvider.GenerateToken(newUser);

            _logger.LogInformation("New user registered: {Username}", email);

            var dto = _mapper.Map<UserDto>(newUser);
            dto.Token = token;
            dto.ExpiresAt = expiresAt;

            return dto;
        }
    }
}
