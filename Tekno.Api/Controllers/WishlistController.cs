using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Cart.DTOs;
using Tekno.Application.Cart.Services;
using Tekno.Application.Catalog.DTOs.Products;

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
        [ProducesResponseType(typeof(ApiResponse<List<ProductSummaryDto>>), 200)]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetWishlistAsync(userId) ?? new List<WishlistDto>();

            List<ProductSummaryDto> productSummary = new List<ProductSummaryDto>();
            foreach (var item in wishlist)
            {
                if (item?.Product != null)
                    productSummary.Add(item.Product);
            }

            return Ok(ApiResponse<List<ProductSummaryDto>>.Ok(productSummary));
        }

        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/wishlist/items
        ///     {
        ///       "productId": 1
        ///     }
        /// 
        /// </remarks>
        [HttpPost("items")]
        [ProducesResponseType(typeof(ApiResponse<WishlistDto>), 200)]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            var userId = GetCurrentUserId();
            var wishlistItem = await _wishlistService.AddToWishlistAsync(userId, dto);
            return Ok(ApiResponse<WishlistDto>.Ok(wishlistItem, "Product added to wishlist successfully"));
        }

        /// <summary>
        /// Remove item from wishlist
        /// </summary>
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = GetCurrentUserId();
            var success = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Product not found in wishlist"));

            return Ok(ApiResponse<bool>.Ok(true, "Product removed from wishlist"));
        }

        /// <summary>
        /// Check if product is in wishlist
        /// </summary>
        [HttpGet("check/{productId:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            var userId = GetCurrentUserId();
            var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
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
