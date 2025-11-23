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
    public class WishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAppLogger<WishlistService> _logger;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            IAppLogger<WishlistService> logger)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<List<WishlistDto>> GetWishlistAsync(int userId)
        {
            var wishlistItems = await _wishlistRepository.GetByUserIdAsync(userId);
            var dtos = new List<WishlistDto>();

            foreach (var item in wishlistItems)
            {
                var variant = await _productRepository.GetProductVariantByIdAsync(item.VariantId);
                if (variant == null) continue; // Skip if variant deleted

                var dto = new WishlistDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    VariantId = item.VariantId,
                    AddedAt = item.AddedAt,
                    ProductName = variant.Product.Name,
                    ProductSlug = variant.Product.Slug,
                    Sku = variant.Sku,
                    BrandName = variant.Product.Brand?.Name ?? string.Empty,
                    CategoryName = variant.Product.Category?.Name ?? string.Empty,
                    Price = variant.Price,
                    Stock = variant.Stock,
                    PrimaryImage = variant.Product.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                    Attributes = variant.VariantAttributes.Select(va => new VariantAttributeInfo
                    {
                        AttributeName = va.Attribute?.Name ?? string.Empty,
                        AttributeValue = va.Value?.Value ?? string.Empty
                    }).ToList()
                };

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<WishlistDto> AddToWishlistAsync(int userId, AddToWishlistDto dto)
        {
            // Check if variant exists
            var variant = await _productRepository.GetProductVariantByIdAsync(dto.VariantId);
            if (variant == null)
            {
                throw new NotFoundException("ProductVariant", dto.VariantId);
            }

            // Check if already in wishlist
            var existing = await _wishlistRepository.GetByUserAndVariantAsync(userId, dto.VariantId);
            if (existing != null)
            {
                throw new ConflictException("This item is already in your wishlist", "WISHLIST_DUPLICATE");
            }

            // Add to wishlist
            var wishlist = new Wishlist(userId, dto.VariantId);
            wishlist = await _wishlistRepository.AddAsync(wishlist);

            _logger.LogInformation(
                "User {UserId} added variant {VariantId} to wishlist",
                userId, dto.VariantId);

            return new WishlistDto
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                VariantId = wishlist.VariantId,
                AddedAt = wishlist.AddedAt,
                ProductName = variant.Product.Name,
                ProductSlug = variant.Product.Slug,
                Sku = variant.Sku,
                BrandName = variant.Product.Brand?.Name ?? string.Empty,
                CategoryName = variant.Product.Category?.Name ?? string.Empty,
                Price = variant.Price,
                Stock = variant.Stock,
                PrimaryImage = variant.Product.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                Attributes = variant.VariantAttributes.Select(va => new VariantAttributeInfo
                {
                    AttributeName = va.Attribute?.Name ?? string.Empty,
                    AttributeValue = va.Value?.Value ?? string.Empty
                }).ToList()
            };
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int variantId)
        {
            var success = await _wishlistRepository.RemoveAsync(userId, variantId);
            
            if (success)
            {
                _logger.LogInformation(
                    "User {UserId} removed variant {VariantId} from wishlist",
                    userId, variantId);
            }

            return success;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int variantId)
        {
            return await _wishlistRepository.IsInWishlistAsync(userId, variantId);
        }
    }
}
