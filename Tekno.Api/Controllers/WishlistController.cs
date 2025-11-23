using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Cart.DTOs;
using Tekno.Application.Cart.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Wishlist management endpoints
    /// </summary>
    [ApiController]
    [Route("api/wishlist")]
    [Authorize] // All endpoints require authentication
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// Get current user's wishlist
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            return Ok(ApiResponse<List<WishlistDto>>.Ok(wishlist));
        }

        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/wishlist/items
        ///     {
        ///       "variantId": 1
        ///     }
        /// 
        /// </remarks>
        [HttpPost("items")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            var userId = GetCurrentUserId();
            var wishlistItem = await _wishlistService.AddToWishlistAsync(userId, dto);
            return Ok(ApiResponse<WishlistDto>.Ok(wishlistItem, "Item added to wishlist successfully"));
        }

        /// <summary>
        /// Remove item from wishlist
        /// </summary>
        [HttpDelete("items/{variantId:int}")]
        public async Task<IActionResult> RemoveFromWishlist(int variantId)
        {
            var userId = GetCurrentUserId();
            var success = await _wishlistService.RemoveFromWishlistAsync(userId, variantId);
            
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Item not found in wishlist"));

            return Ok(ApiResponse<bool>.Ok(true, "Item removed from wishlist"));
        }

        /// <summary>
        /// Check if variant is in wishlist
        /// </summary>
        [HttpGet("check/{variantId:int}")]
        public async Task<IActionResult> IsInWishlist(int variantId)
        {
            var userId = GetCurrentUserId();
            var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, variantId);
            return Ok(ApiResponse<bool>.Ok(isInWishlist));
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
