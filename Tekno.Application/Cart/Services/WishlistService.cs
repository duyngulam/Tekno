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
    public class WishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<WishlistService> _logger;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<WishlistService> logger)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<WishlistDto>> GetWishlistAsync(int userId)
        {
            var wishlistItems = await _wishlistRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<WishlistDto>>(wishlistItems);
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

            // Reload wishlist with all navigation properties
            var reloaded = await _wishlistRepository.GetByUserAndVariantAsync(userId, dto.VariantId);
            // Since GetByUserAndVariantAsync doesn't include navigation properties, get from full list
            var allWishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            var wishlistWithNav = allWishlist.FirstOrDefault(w => w.Id == wishlist.Id);
            
            return _mapper.Map<WishlistDto>(wishlistWithNav);
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
