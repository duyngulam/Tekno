using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.DTOs;
using Tekno.Application.Promotion.Interface;
using Tekno.Domain.Promotion;
using PromotionEntity = Tekno.Domain.Promotion.Promotion;

namespace Tekno.Application.Promotion.Services
{
    public class PromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<PromotionService> _logger;

        public PromotionService(
            IPromotionRepository promotionRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<PromotionService> logger)
        {
            _promotionRepository = promotionRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<PromotionDto>> GetPagedPromotionsAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1,
            int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _promotionRepository.GetPagedAsync(search, status, startDate, endDate, paging);

            var dtos = _mapper.Map<List<PromotionDto>>(result.Data);
            return new PagedResult<PromotionDto>(dtos, result.TotalRecords, page, pageSize);
        }

        public async Task<PromotionDto?> GetPromotionByIdAsync(int id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);
            return promotion == null ? null : _mapper.Map<PromotionDto>(promotion);
        }

        public async Task<List<PromotionDto>> GetActivePromotionsAsync()
        {
            var promotions = await _promotionRepository.GetActivePromotionsAsync();
            return _mapper.Map<List<PromotionDto>>(promotions.ToList());
        }

        public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto)
        {
            // Validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                throw new Application.Common.Exceptions.ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "EndDate", new[] { $"End date must be after start date ({dto.StartDate:yyyy-MM-dd})" } }
                    });
            }

            // Parse promotion type
            if (!Enum.TryParse<PromotionType>(dto.Type, true, out var promotionType))
            {
                throw new Application.Common.Exceptions.ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "Type", new[] { $"Invalid promotion type '{dto.Type}'. Allowed values are: Percentage, FixedAmount" } }
                    });
            }

            // Create promotion entity
            var promotion = new PromotionEntity(
                name: dto.Name,
                description: dto.Description,
                type: promotionType,
                value: dto.Value,
                startDate: dto.StartDate,
                endDate: dto.EndDate,
                priority: dto.Priority,
                stackableWithCoupons: dto.StackableWithCoupons
            );

            // Add applicable categories
            foreach (var categoryId in dto.ApplicableCategoryIds)
            {
                promotion.AddCategory(categoryId);
            }

            // Add applicable products
            foreach (var productId in dto.ApplicableProductIds)
            {
                promotion.AddProduct(productId);
            }

            var created = await _promotionRepository.CreateAsync(promotion);
            _logger.LogInformation("Created promotion {Name} with {Value}% discount", 
                created.Name, created.Value);

            // If promotion should be active immediately, apply to products
            if (created.IsActive)
            {
                await ApplyPromotionToProductsAsync(created);
            }

            return _mapper.Map<PromotionDto>(created);
        }

        public async Task<PromotionDto?> UpdatePromotionAsync(int id, UpdatePromotionDto dto)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                _logger.LogWarning("Update failed: Promotion with ID {Id} not found", id);
                return null;
            }

            // Validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                throw new Application.Common.Exceptions.ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "EndDate", new[] { $"End date must be after start date ({dto.StartDate:yyyy-MM-dd})" } }
                    });
            }

            // Update promotion
            promotion.Update(
                name: dto.Name,
                description: dto.Description,
                value: dto.Value,
                startDate: dto.StartDate,
                endDate: dto.EndDate,
                priority: dto.Priority,
                stackableWithCoupons: dto.StackableWithCoupons
            );

            // Update categories and products
            promotion.ApplicableCategories.Clear();
            foreach (var categoryId in dto.ApplicableCategoryIds)
            {
                promotion.AddCategory(categoryId);
            }

            promotion.ApplicableProducts.Clear();
            foreach (var productId in dto.ApplicableProductIds)
            {
                promotion.AddProduct(productId);
            }

            var updated = await _promotionRepository.UpdateAsync(promotion);
            _logger.LogInformation("Updated promotion ID {Id} ({Name})", id, updated.Name);

            // Re-apply to products if active
            if (updated.IsActive)
            {
                await ApplyPromotionToProductsAsync(updated);
            }

            return _mapper.Map<PromotionDto>(updated);
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var success = await _promotionRepository.DeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Deleted promotion ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Delete failed: Promotion with ID {Id} not found", id);
            }
            return success;
        }

        public async Task<PromotionDto?> ActivatePromotionAsync(int id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                _logger.LogWarning("Activate failed: Promotion with ID {Id} not found", id);
                return null;
            }

            promotion.Activate();
            var updated = await _promotionRepository.UpdateAsync(promotion);
            _logger.LogInformation("Activated promotion ID {Id} ({Name})", id, updated.Name);

            // Apply to products
            await ApplyPromotionToProductsAsync(updated);

            return _mapper.Map<PromotionDto>(updated);
        }

        public async Task<PromotionDto?> PausePromotionAsync(int id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                _logger.LogWarning("Pause failed: Promotion with ID {Id} not found", id);
                return null;
            }

            promotion.Pause();
            var updated = await _promotionRepository.UpdateAsync(promotion);
            _logger.LogInformation("Paused promotion ID {Id} ({Name})", id, updated.Name);

            // Remove from products
            await RemovePromotionFromProductsAsync(updated);

            return _mapper.Map<PromotionDto>(updated);
        }

        /// <summary>
        /// Apply promotion discounts to affected products
        /// </summary>
        public async Task ApplyPromotionToProductsAsync(PromotionEntity promotion)
        {
            _logger.LogInformation("Applying promotion {Name} to products", promotion.Name);

            var affectedProducts = new List<Domain.Catalog.Product>();

            // Get products by specific product IDs
            foreach (var pp in promotion.ApplicableProducts)
            {
                var product = await _productRepository.GetProductByIdAsync(pp.ProductId);
                if (product != null)
                {
                    affectedProducts.Add(product);
                }
            }

            // Get products by category
            foreach (var pc in promotion.ApplicableCategories)
            {
                var categoryProducts = await GetProductsByCategoryIdAsync(pc.CategoryId);
                affectedProducts.AddRange(categoryProducts);
            }

            // Remove duplicates
            affectedProducts = affectedProducts.DistinctBy(p => p.Id).ToList();

            foreach (var product in affectedProducts)
            {
                product.SetDiscount(discountPercent: promotion.Value);
                await _productRepository.UpdateProductAsync(product);
            }

            _logger.LogInformation("Applied promotion {Name} to {Count} products", 
                promotion.Name, affectedProducts.Count);
        }

        /// <summary>
        /// Remove promotion discounts from affected products
        /// </summary>
        public async Task RemovePromotionFromProductsAsync(PromotionEntity promotion)
        {
            _logger.LogInformation("Removing promotion {Name} from products", promotion.Name);

            var affectedProducts = new List<Domain.Catalog.Product>();

            // Get products by specific product IDs
            foreach (var pp in promotion.ApplicableProducts)
            {
                var product = await _productRepository.GetProductByIdAsync(pp.ProductId);
                if (product != null)
                {
                    affectedProducts.Add(product);
                }
            }

            // Get products by category
            foreach (var pc in promotion.ApplicableCategories)
            {
                var categoryProducts = await GetProductsByCategoryIdAsync(pc.CategoryId);
                affectedProducts.AddRange(categoryProducts);
            }

            // Remove duplicates
            affectedProducts = affectedProducts.DistinctBy(p => p.Id).ToList();

            foreach (var product in affectedProducts)
            {
                product.RemoveDiscount();
                await _productRepository.UpdateProductAsync(product);
            }

            _logger.LogInformation("Removed promotion {Name} from {Count} products", 
                promotion.Name, affectedProducts.Count);
        }

        private async Task<List<Domain.Catalog.Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            // Get all products in category
            var allProducts = await _productRepository.GetAllProductsWithDetailAsync();
            return allProducts.Where(p => p.CategoryId == categoryId).ToList();
        }
    }
}
