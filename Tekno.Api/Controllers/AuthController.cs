using Microsoft.AspNetCore.Mvc;
using Tekno.Application.Auth.Services;
using Tekno.Api.Auth.Models;
using Tekno.Application.Auth.DTOs;
namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        //---------------Login----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user {Username}", request.Username);

            var userDto = await _authService.LoginAsync(request.Username, request.Password);

            if (userDto == null)
            {
                _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return Ok(userDto);
        }
        //---------------Register----------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Registration attempt for user {Username}", request.Username);
            var userDto = await _authService.RegisterAsync(request);
            if (userDto == null)
            {
                _logger.LogWarning("Failed registration attempt for user {Username}", request.Username);
                return BadRequest(new { message = "Username already exists" });
            }
            _logger.LogInformation("User {Username} registered successfully", request.Username);
            return Ok(userDto);
        }

    }

}
