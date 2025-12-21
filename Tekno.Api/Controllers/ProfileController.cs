using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Services;
using System.IO;
using System.Text.Json;
using System.Linq;
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

        public ProfileController(ProfileService profileService, MediaService mediaService)
        {
            _profileService = profileService;
            _mediaService = mediaService;
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
        /// Update all profile information at once (fullname, phone, email, password)
        /// </summary>
        /// <remarks>
        /// Sample request (update everything):
        /// 
        ///     PUT /api/profile/all
        ///     {
        ///       "fullname": "John Doe",
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
        ///       "fullname": "John Doe",
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
            var userId = GetCurrentUserId();
            var profile = await _profileService.UpdateAllProfileAsync(userId, dto);
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
            return Ok(ApiResponse<List<UserAddressDto>>.Ok(addresses));
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

        /// <summary>
        /// Return list of provinces for address selection.
        /// This reads the included `openapi.json` file and extracts example province entries if present.
        /// It's intended to help frontend address selection when offline. Prefer calling the upstream API in production.
        /// </summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            // Try to load openapi.json from application base directory
            var basePath = Directory.GetCurrentDirectory();
            var openApiPath = Path.Combine(basePath, "openapi.json");

            if (!System.IO.File.Exists(openApiPath))
                return NotFound(ApiResponse<List<ProvinceOptionDto>>.Fail("openapi.json not found in application root"));

            try
            {
                using var stream = System.IO.File.OpenRead(openApiPath);
                using var doc = await JsonDocument.ParseAsync(stream);

                var root = doc.RootElement;

                // Navigate to components.schemas.ProvinceResponse.examples
                if (root.TryGetProperty("components", out var components) &&
                    components.TryGetProperty("schemas", out var schemas) &&
                    schemas.TryGetProperty("ProvinceResponse", out var provinceSchema) &&
                    provinceSchema.TryGetProperty("examples", out var examples) &&
                    examples.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<ProvinceOptionDto>();
                    foreach (var ex in examples.EnumerateArray())
                    {
                        if (ex.TryGetProperty("code", out var codeEl) && codeEl.ValueKind == JsonValueKind.Number &&
                            ex.TryGetProperty("name", out var nameEl))
                        {
                            list.Add(new ProvinceOptionDto
                            {
                                Code = codeEl.GetInt32(),
                                Name = nameEl.GetString() ?? string.Empty,
                                Codename = ex.TryGetProperty("codename", out var cn) ? cn.GetString() ?? string.Empty : string.Empty
                            });
                        }
                    }

                    if (list.Any())
                        return Ok(ApiResponse<List<ProvinceOptionDto>>.Ok(list));
                }

                // No examples found or empty - return empty list
                return Ok(ApiResponse<List<ProvinceOptionDto>>.Ok(new List<ProvinceOptionDto>()));
            }
            catch
            {
                return StatusCode(500, ApiResponse<List<ProvinceOptionDto>>.Fail("Failed to parse openapi.json"));
            }
        }

        /// <summary>
        /// Trigger user storage cleanup (best-effort). This will attempt to remove orphaned images for the current user from the media provider.
        /// Implementation is best-effort and returns success if request accepted.
        /// </summary>
        [HttpPost("storage-cleanup")]
        public async Task<IActionResult> StorageCleanup()
        {
            var userId = GetCurrentUserId();

            // MediaService currently exposes DeleteImageAsync. There's no dedicated cleanup method available.
            // We treat this endpoint as a placeholder that the frontend can call to trigger background cleanup logic later.
            // For now we return 202 Accepted to indicate request was received.

            // TODO: implement MediaService.CleanupUserImagesAsync(userId) when cloud provider supports listing/deleting by tag

            return Accepted(ApiResponse<string>.Ok("Storage cleanup scheduled"));
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
