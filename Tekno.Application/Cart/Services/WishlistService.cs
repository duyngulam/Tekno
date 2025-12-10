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
            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Product", dto.ProductId);
            }

            // Check if already in wishlist
            var existing = await _wishlistRepository.GetByUserAndProductAsync(userId, dto.ProductId);
            if (existing != null)
            {
                throw new ConflictException("This product is already in your wishlist", "WISHLIST_DUPLICATE");
            }

            // Add to wishlist
            var wishlist = new Wishlist(userId, dto.ProductId);
            wishlist = await _wishlistRepository.AddAsync(wishlist);

            _logger.LogInformation(
                "User {UserId} added product {ProductId} to wishlist",
                userId, dto.ProductId);

            // Reload wishlist with all navigation properties
            var allWishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            var wishlistWithNav = allWishlist.FirstOrDefault(w => w.Id == wishlist.Id);
            
            return _mapper.Map<WishlistDto>(wishlistWithNav);
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            var success = await _wishlistRepository.RemoveAsync(userId, productId);
            
            if (success)
            {
                _logger.LogInformation(
                    "User {UserId} removed product {ProductId} from wishlist",
                    userId, productId);
            }

            return success;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            return await _wishlistRepository.IsInWishlistAsync(userId, productId);
        }
    }
}
