using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Cart.DTOs;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Cart;

namespace Tekno.Application.Cart.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                // Create new cart for user
                cart = new UserCart(userId);
                cart = await _cartRepository.CreateAsync(cart);
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // Validate variant exists and has stock
            var variant = await _productRepository.GetProductVariantByIdAsync(dto.VariantId);
            if (variant == null)
            {
                throw new NotFoundException("ProductVariant", dto.VariantId);
            }

            if (variant.Stock < dto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Only {variant.Stock} items available.");
            }

            // Get or create cart
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new UserCart(userId);
                cart = await _cartRepository.CreateAsync(cart);
            }

            // Add item to cart
            cart.AddItem(dto.VariantId, dto.Quantity, variant.Price);
            cart = await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation(
                "User {UserId} added {Quantity}x variant {VariantId} to cart",
                userId, dto.Quantity, dto.VariantId);

            // Reload cart with all navigation properties
            cart = await _cartRepository.GetByUserIdAsync(userId);
            return _mapper.Map<CartDto>(cart!);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int variantId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("Cart", userId);
            }

            // Validate variant stock
            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
            {
                throw new NotFoundException("ProductVariant", variantId);
            }

            if (variant.Stock < dto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Only {variant.Stock} items available.");
            }

            cart.UpdateItemQuantity(variantId, dto.Quantity);
            cart = await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation(
                "User {UserId} updated variant {VariantId} quantity to {Quantity}",
                userId, variantId, dto.Quantity);

            // Reload cart with all navigation properties
            cart = await _cartRepository.GetByUserIdAsync(userId);
            return _mapper.Map<CartDto>(cart!);
        }

        public async Task<CartDto> RemoveFromCartAsync(int userId, int variantId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("Cart", userId);
            }

            cart.RemoveItem(variantId);
            cart = await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation(
                "User {UserId} removed variant {VariantId} from cart",
                userId, variantId);

            // Reload cart with all navigation properties
            cart = await _cartRepository.GetByUserIdAsync(userId);
            return _mapper.Map<CartDto>(cart!);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null) return false;

            cart.Clear();
            await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation("User {UserId} cleared their cart", userId);
            return true;
        }
    }
}
