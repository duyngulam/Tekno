using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// User profile management endpoints
    /// </summary>
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound(ApiResponse<UserProfileDto>.Fail("Profile not found"));

            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }

        /// <summary>
        /// Update profile (fullname, phone number)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/profile
        ///     {
        ///       "fullname": "John Doe",
        ///       "phoneNumber": "+84987654321"
        ///     }
        /// 
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Profile updated successfully"));
        }

        /// <summary>
        /// Update email (requires password verification)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/profile/email
        ///     {
        ///       "newEmail": "newemail@example.com",
        ///       "currentPassword": "MyPassword123"
        ///     }
        /// 
        /// </remarks>
        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.UpdateEmailAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Email updated successfully"));
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/profile/password
        ///     {
        ///       "currentPassword": "OldPassword123",
        ///       "newPassword": "NewPassword456",
        ///       "confirmPassword": "NewPassword456"
        ///     }
        /// 
        /// </remarks>
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            await _profileService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponse<bool>.Ok(true, "Password changed successfully"));
        }

        /// <summary>
        /// Get all user addresses
        /// </summary>
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = GetCurrentUserId();
            var addresses = await _profileService.GetAddressesAsync(userId);
            return Ok(ApiResponse<System.Collections.Generic.List<UserAddressDto>>.Ok(addresses));
        }

        /// <summary>
        /// Add new address
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/profile/addresses
        ///     {
        ///       "recipientName": "John Doe",
        ///       "phoneNumber": "+84987654321",
        ///       "addressLine1": "123 Nguyen Hue Street",
        ///       "addressLine2": "Apartment 5B",
        ///       "city": "Ho Chi Minh City",
        ///       "state": "Ho Chi Minh",
        ///       "postalCode": "700000",
        ///       "country": "Vietnam",
        ///       "isDefault": true
        ///     }
        /// 
        /// </remarks>
        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto dto)
        {
            var userId = GetCurrentUserId();
            var address = await _profileService.AddAddressAsync(userId, dto);
            return CreatedAtAction(
                nameof(GetAddresses),
                ApiResponse<UserAddressDto>.Ok(address, "Address added successfully"));
        }

        /// <summary>
        /// Update existing address
        /// </summary>
        [HttpPut("addresses/{addressId:int}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressDto dto)
        {
            var userId = GetCurrentUserId();
            var address = await _profileService.UpdateAddressAsync(userId, addressId, dto);
            return Ok(ApiResponse<UserAddressDto>.Ok(address, "Address updated successfully"));
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        [HttpPatch("addresses/{addressId:int}/default")]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            await _profileService.SetDefaultAddressAsync(userId, addressId);
            return Ok(ApiResponse<bool>.Ok(true, "Default address updated"));
        }

        /// <summary>
        /// Delete address
        /// </summary>
        [HttpDelete("addresses/{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            var success = await _profileService.DeleteAddressAsync(userId, addressId);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Address not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Address deleted successfully"));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }
    }
}
