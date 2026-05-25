using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Services;
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
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        ///       "fullname": "Nguy?n Van A",
        ///       "phoneNumber": "0987654321"
        ///     }
        /// 
        /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        ///       "fullname": "Nguy?n Van A",
        ///       "phoneNumber": "0987654321",
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
        ///       "fullname": "Nguy?n Van A",
        ///       "phoneNumber": "0987654321",
        ///       "newEmail": "newemail@example.com",
        ///       "currentPassword": "CurrentPassword123"
        ///     }
        /// 
        /// Note: Current password is always required. Email and password changes are optional.
        /// </remarks>
        [HttpPut("all")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            await _profileService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponse<bool>.Ok(true, "Password changed successfully"));
        }

        /// <summary>
        /// Get all user addresses with Vietnamese location format
        /// </summary>
        /// <remarks>
        /// Returns addresses with Vietnamese province/district/ward structure:
        /// - address_line: Street address
        /// - province_code/province_name: e.g., 79 / "Thành ph? H? Chí Minh"
        /// - district_code/district_name: e.g., 760 / "Qu?n 1"
        /// - ward_code/ward_name: e.g., 26734 / "Phu?ng B?n Nghé"
        /// 
        /// Use with Location API:
        /// - GET /api/location/provinces - Get all provinces
        /// - GET /api/location/provinces/{code}/districts - Get districts by province
        /// - GET /api/location/districts/{code}/wards - Get wards by district
        /// </remarks>
        [HttpGet("addresses")]
        [ProducesResponseType(typeof(ApiResponse<List<UserAddressDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = GetCurrentUserId();
            var addresses = await _profileService.GetAddressesAsync(userId);
            return Ok(ApiResponse<List<UserAddressDto>>.Ok(addresses));
        }

        /// <summary>
        /// Add new address with Vietnamese location structure
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/profile/addresses
        ///     {
        ///       "recipientName": "Nguy?n Van A",
        ///       "phoneNumber": "0987654321",
        ///       "addressLine": "123 Nguy?n Hu?",
        ///       "provinceCode": 79,
        ///       "provinceName": "Thành ph? H? Chí Minh",
        ///       "districtCode": 760,
        ///       "districtName": "Qu?n 1",
        ///       "wardCode": 26734,
        ///       "wardName": "Phu?ng B?n Nghé",
        ///       "isDefault": true
        ///     }
        /// 
        /// **How to get location data:**
        /// 
        /// 1. Call `GET /api/location/provinces` to get all provinces
        /// 2. User selects province (e.g., code: 79, name: "Thành ph? H? Chí Minh")
        /// 3. Call `GET /api/location/provinces/79/districts` to get districts
        /// 4. User selects district (e.g., code: 760, name: "Qu?n 1")
        /// 5. Call `GET /api/location/districts/760/wards` to get wards
        /// 6. User selects ward (e.g., code: 26734, name: "Phu?ng B?n Nghé")
        /// 7. Submit all codes and names in the address
        /// 
        /// **Why both code and name:**
        /// - Codes are used for validation and lookups
        /// - Names are stored for display (in case location data changes)
        /// - This ensures addresses remain readable even if location database updates
        /// </remarks>
        [HttpPost("addresses")]
        [ProducesResponseType(typeof(ApiResponse<UserAddressDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto dto)
        {
            var userId = GetCurrentUserId();
            var address = await _profileService.AddAddressAsync(userId, dto);
            return CreatedAtAction(
                nameof(GetAddresses),
                ApiResponse<UserAddressDto>.Ok(address, "Address added successfully"));
        }

        /// <summary>
        /// Update existing address with Vietnamese location structure
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/profile/addresses/1
        ///     {
        ///       "recipientName": "Nguy?n Van A",
        ///       "phoneNumber": "0987654321",
        ///       "addressLine": "456 Võ Van T?n",
        ///       "provinceCode": 79,
        ///       "provinceName": "Thành ph? H? Chí Minh",
        ///       "districtCode": 769,
        ///       "districtName": "Qu?n 3",
        ///       "wardCode": 27031,
        ///       "wardName": "Phu?ng 6"
        ///     }
        /// 
        /// Use the same Location API workflow as creating addresses.
        /// All location codes and names must be provided.
        /// </remarks>
        [HttpPut("addresses/{addressId:int}")]
        [ProducesResponseType(typeof(ApiResponse<UserAddressDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressDto dto)
        {
            var userId = GetCurrentUserId();
            var address = await _profileService.UpdateAddressAsync(userId, addressId, dto);
            return Ok(ApiResponse<UserAddressDto>.Ok(address, "Address updated successfully"));
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        /// <remarks>
        /// Sets the specified address as the default shipping address.
        /// All other addresses will be marked as non-default.
        /// 
        ///     PATCH /api/profile/addresses/1/default
        /// 
        /// </remarks>
        [HttpPatch("addresses/{addressId:int}/default")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            await _profileService.SetDefaultAddressAsync(userId, addressId);
            return Ok(ApiResponse<bool>.Ok(true, "Default address updated"));
        }

        /// <summary>
        /// Delete address
        /// </summary>
        /// <remarks>
        /// Deletes the specified address.
        /// Cannot delete the only address if it's marked as default.
        /// If you delete a default address and other addresses exist, another will be automatically set as default.
        /// 
        ///     DELETE /api/profile/addresses/1
        /// 
        /// </remarks>
        [HttpDelete("addresses/{addressId:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            var success = await _profileService.DeleteAddressAsync(userId, addressId);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Address not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Address deleted successfully"));
        }

        /// <summary>
        /// Trigger user storage cleanup (best-effort)
        /// </summary>
        /// <remarks>
        /// This will attempt to remove orphaned images for the current user from the media provider.
        /// Implementation is best-effort and returns success if request accepted.
        /// 
        /// **Note:** This is a placeholder endpoint. Full implementation requires cloud provider 
        /// support for listing/deleting resources by user tag.
        /// 
        ///     POST /api/profile/storage-cleanup
        /// 
        /// </remarks>
        //[HttpPost("storage-cleanup")]
        //public async Task<IActionResult> StorageCleanup()
        //{
        //    var userId = GetCurrentUserId();

        //    // MediaService currently exposes DeleteImageAsync. There's no dedicated cleanup method available.
        //    // We treat this endpoint as a placeholder that the frontend can call to trigger background cleanup logic later.
        //    // For now we return 202 Accepted to indicate request was received.

        //    // TODO: implement MediaService.CleanupUserImagesAsync(userId) when cloud provider supports listing/deleting by tag

        //    return Accepted(ApiResponse<string>.Ok("Storage cleanup scheduled"));
        //}

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
