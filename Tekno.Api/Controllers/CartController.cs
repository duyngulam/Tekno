using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Cart.DTOs;
using Tekno.Application.Cart.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Shopping cart management endpoints
    /// </summary>
    [ApiController]
    [Route("api/cart")]
    [Authorize] // All endpoints require authentication
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Get current user's cart
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(ApiResponse<CartDto>.Ok(cart));
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/cart/items
        ///     {
        ///       "variantId": 1,
        ///       "quantity": 2
        ///     }
        /// 
        /// </remarks>
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.AddToCartAsync(userId, dto);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item added to cart successfully"));
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("items/{variantId:int}")]
        public async Task<IActionResult> UpdateCartItem(int variantId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, variantId, dto);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Cart item updated successfully"));
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        [HttpDelete("items/{variantId:int}")]
        public async Task<IActionResult> RemoveFromCart(int variantId)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.RemoveFromCartAsync(userId, variantId);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item removed from cart"));
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            var success = await _cartService.ClearCartAsync(userId);
            
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Cart not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Cart cleared successfully"));
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
