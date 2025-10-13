using Microsoft.AspNetCore.Mvc;
using Tekno.Application.Auth.Services;
using Tekno.Api.Models.Auth;
using Tekno.Application.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Tekno.Api.Common.Responses;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var dto = await _authService.LoginAsync(request.Email, request.Password);
            if (dto == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid email or password"));
            var result = AuthResponse.FromAppDto(dto);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var dto = await _authService.RegisterAsync(request.Email, request.Password, request.Role);
            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail("User already exists"));
            var result = AuthResponse.FromAppDto(dto);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Register successful"));
        }
    }
}
