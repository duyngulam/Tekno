using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Services;
using System;
using Tekno.Application.Common.Media.Services;

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
        private readonly MediaService _mediaService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            ProfileService profileService, 
            MediaService mediaService,
            ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _mediaService = mediaService;
            _logger = logger;
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
        ///       "fullname": "Nguy?n V?n A",
        ///       "phoneNumber": "+84987654321"
        ///     }
        /// 
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _profileService.UpdateProfileAsync(userId, dto);
                return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update profile: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update all profile information at once (fullname, phone, email, password)
        /// </summary>
        /// <remarks>
        /// Sample request (update everything):
        /// 
        ///     PUT /api/profile/all
        ///     {
        ///       "fullname": "Nguy?n V?n A",
        ///       "phoneNumber": "+84987654321",
        ///       "newEmail": "newemail@example.com",
        ///       "newPassword": "NewPassword456",
        ///       "confirmPassword": "NewPassword456",
        ///       "currentPassword": "CurrentPassword123"
        ///     }
        /// 
        /// Sample request (update only profile info and email):
        /// 
        ///     PUT /api/profile/all
        ///     {
        ///       "fullname": "Nguy?n V?n A",
        ///       "phoneNumber": "+84987654321",
        ///       "newEmail": "newemail@example.com",
        ///       "currentPassword": "CurrentPassword123"
        ///     }
        /// 
        /// Note: Current password is always required. Email and password changes are optional.
        /// </remarks>
        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllProfile([FromBody] UpdateAllProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _profileService.UpdateAllProfileAsync(userId, dto);
                return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update profile: {ex.Message}"));
            }
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
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _profileService.UpdateEmailAsync(userId, dto);
                return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Email updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update email");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update email: {ex.Message}"));
            }
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
            try
            {
                var userId = GetCurrentUserId();
                await _profileService.ChangePasswordAsync(userId, dto);
                return Ok(ApiResponse<bool>.Ok(true, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change password");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to change password: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all user addresses
        /// </summary>
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                var addresses = await _profileService.GetAddressesAsync(userId);
                return Ok(ApiResponse<List<UserAddressDto>>.Ok(addresses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get addresses");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get addresses: {ex.Message}"));
            }
        }

        /// <summary>
        /// Add new address
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/profile/addresses
        ///     {
        ///       "recipientName": "Nguy?n V?n A",
        ///       "phoneNumber": "+84987654321",
        ///       "addressLine": "123 Nguy?n Hu?, Ph??ng B?n Nghé",
        ///       "provinceCode": 79,
        ///       "provinceName": "Thành ph? H? Chí Minh",
        ///       "districtCode": 760,
        ///       "districtName": "Qu?n 1",
        ///       "wardCode": 26734,
        ///       "wardName": "Ph??ng B?n Nghé",
        ///       "isDefault": true
        ///     }
        /// 
        /// **Note:** Use `/api/locations/*` endpoints to get province/district/ward codes and names:
        /// - GET /api/locations/provinces - Get all provinces
        /// - GET /api/locations/provinces/{provinceCode}/districts - Get districts by province
        /// - GET /api/locations/districts/{districtCode}/wards - Get wards by district
        /// </remarks>
        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _profileService.AddAddressAsync(userId, dto);
                return CreatedAtAction(
                    nameof(GetAddresses),
                    ApiResponse<UserAddressDto>.Ok(address, "Address added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add address");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to add address: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update existing address
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/profile/addresses/1
        ///     {
        ///       "recipientName": "Nguy?n V?n A",
        ///       "phoneNumber": "+84987654321",
        ///       "addressLine": "456 Lê L?i, Ph??ng B?n Thành",
        ///       "provinceCode": 79,
        ///       "provinceName": "Thành ph? H? Chí Minh",
        ///       "districtCode": 760,
        ///       "districtName": "Qu?n 1",
        ///       "wardCode": 26743,
        ///       "wardName": "Ph??ng B?n Thành"
        ///     }
        /// 
        /// </remarks>
        [HttpPut("addresses/{addressId:int}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _profileService.UpdateAddressAsync(userId, addressId, dto);
                return Ok(ApiResponse<UserAddressDto>.Ok(address, "Address updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update address {AddressId}", addressId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update address: {ex.Message}"));
            }
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        [HttpPatch("addresses/{addressId:int}/default")]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _profileService.SetDefaultAddressAsync(userId, addressId);
                return Ok(ApiResponse<bool>.Ok(true, "Default address updated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set default address {AddressId}", addressId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to set default address: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete address
        /// </summary>
        [HttpDelete("addresses/{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _profileService.DeleteAddressAsync(userId, addressId);

                if (!success)
                    return NotFound(ApiResponse<bool>.Fail("Address not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Address deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete address {AddressId}", addressId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete address: {ex.Message}"));
            }
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
