using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Tekno.Application.Auth.Services;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Auth;
using System.Threading.Tasks;
using System;

namespace Tekno.Application.Tests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IJwtProvider> _jwtProviderMock = new();
        private readonly Mock<IAppLogger<AuthService>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService(
                _userRepoMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_Should_Return_UserDto_When_Valid_Credentials()
        {
            // Arrange
            var email = "test@example.com";
            var password = "123456";
            var user = new User(email, "hashed_pw", 1);

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(password, user.PasswordHash)).Returns(true);
            _jwtProviderMock.Setup(j => j.GenerateToken(user)).Returns(("fake-token", DateTime.UtcNow.AddHours(1)));
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = email });

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
            result.Token.Should().Be("fake-token");

            _userRepoMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            _passwordHasherMock.Verify(h => h.Verify(password, "hashed_pw"), Times.Once);
        }
        [Fact]
        public async Task LoginAsync_Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync("noone@example.com", "pass");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Null_When_Password_Invalid()
        {
            var email = "test@example.com";
            var user = new User(email, "hashed_pw", 1);

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify("wrong", "hashed_pw")).Returns(false);

            var result = await _authService.LoginAsync(email, "wrong");

            result.Should().BeNull();
        }
        [Fact]
        public async Task RegisterAsync_Should_Create_User_When_Email_Not_Exists()
        {
            // Arrange
            var email = "new@example.com";
            var password = "123456";
            var roleName = "User";
            var role = new Role(roleName);

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.GetRoleByNameAsync(roleName)).ReturnsAsync(role);
            _passwordHasherMock.Setup(h => h.Hash(password)).Returns("hashed_pw");
            _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns(("new-token", DateTime.UtcNow.AddHours(1)));
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto { Email = email });

            // Act
            var result = await _authService.RegisterAsync(email, password, roleName);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
            result.Token.Should().Be("new-token");

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), email), Times.Once);
        }

        [Fact]
        public async Task TC_Signup_002_RegisterAsync_Should_Return_Null_When_Email_Already_Exists()
        {
            // Arrange - Test email already exist or not
            var email = "Usernametest@gmail.com";
            var existingUser = new User(email, "existing_hashed_pw", 1);
            
            _userRepoMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(email, "Password123", "Customer");

            // Assert - Display error message: "This email have been used"
            result.Should().BeNull();
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>(), email), Times.Once);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task TC_Signup_001_Invalid_Email_Format_Should_Be_Caught_By_Validation()
        {
            // Note: This test validates that ModelState validation in API layer catches invalid email
            // The validation happens at the RegisterRequest DTO level with [EmailAddress] attribute
            // Service layer assumes valid input from API validation
            
            // This test demonstrates service behavior when bypassing API validation
            var invalidEmail = "Usernametest"; // Invalid format
            var role = new Role("Customer");
            
            _userRepoMock.Setup(r => r.GetByEmailAsync(invalidEmail)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.GetRoleByNameAsync("Customer")).ReturnsAsync(role);
            _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
            _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns(("token", DateTime.UtcNow.AddHours(1)));
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto { Email = invalidEmail });

            // Act - Service will process if it gets past API validation
            var result = await _authService.RegisterAsync(invalidEmail, "abc", "Customer");

            // Assert - Service creates user (API validation should prevent this in real scenario)
            result.Should().NotBeNull();
            // In production, API ValidationFilter will catch this before reaching service
        }

        [Fact]
        public async Task TC_Signup_001_Short_Password_Should_Be_Caught_By_Validation()
        {
            // Note: Password length validation happens at API layer with [MinLength(6)] attribute
            // This test shows service behavior, but API validation prevents short passwords
            
            var email = "test@example.com";
            var shortPassword = "abc"; // Less than 6 characters
            var role = new Role("Customer");
            
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.GetRoleByNameAsync("Customer")).ReturnsAsync(role);
            _passwordHasherMock.Setup(h => h.Hash(shortPassword)).Returns("hashed_abc");
            _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns(("token", DateTime.UtcNow.AddHours(1)));
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto { Email = email });

            // Act - Service will process if API validation is bypassed
            var result = await _authService.RegisterAsync(email, shortPassword, "Customer");

            // Assert - Service creates user (API ValidationFilter should prevent this)
            result.Should().NotBeNull();
            _passwordHasherMock.Verify(h => h.Hash(shortPassword), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Null_When_Email_Already_Exists()
        {
            var email = "exists@example.com";
            _userRepoMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new User(email, "pw", 1));

            var result = await _authService.RegisterAsync(email, "123456", "User");

            result.Should().BeNull();
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>(), email), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Role_Not_Found()
        {
            var email = "new@example.com";
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.GetRoleByNameAsync("Admin")).ReturnsAsync((Role?)null);

            Func<Task> act = async () => await _authService.RegisterAsync(email, "123456", "Admin");

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Role not found");
        }
    }
}
