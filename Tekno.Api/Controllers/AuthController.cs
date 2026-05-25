using Microsoft.AspNetCore.Mvc;
using Tekno.Application.Auth.Services;
using Tekno.Api.Models.Auth;
using Tekno.Application.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Tekno.Api.Commons.Responses;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [ValidationFilter]
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
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var dto = await _authService.LoginAsync(request.Email, request.Password);

            if (dto == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid email or password"));

            return Ok(ApiResponse<UserDto>.Ok(dto, "Login successful"));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var dto = await _authService.RegisterAsync(request.Email, request.Password, request.Role);

            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail("User already exists"));

            return Ok(ApiResponse<UserDto>.Ok(dto, "Register successful"));
        }
    }
}
