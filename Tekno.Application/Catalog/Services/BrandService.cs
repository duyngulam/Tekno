using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class BrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly MediaService _mediaService;
        private readonly IAppLogger<BrandService> _logger;

        public BrandService(
            IBrandRepository brandRepository, 
            IMapper mapper, 
            ICacheService cache,
            MediaService mediaService,
            IAppLogger<BrandService> logger)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _cache = cache;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            return await _cache.CacheOrGetAsync(
                CachePolicies.BrandKey,
                async () => _mapper.Map<List<BrandDto>>(await _brandRepository.GetAllBrandsAsync()),
                CachePolicies.BrandTtl
            );
        }

        public async Task<PagedResult<BrandDto>> GetPagedAsync(string? search = null, int page = 1, int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _brandRepository.GetPagedAsync(search, paging);
            
            var dtos = _mapper.Map<List<BrandDto>>(result.Data);
            return new PagedResult<BrandDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        public async Task<BrandDto> GetBrandBySlugAsync(string slug)
        {
            var brand = await _brandRepository.GetBrandBySlugAsync(slug);
            return _mapper.Map<BrandDto>(brand);
        }

        public async Task<BrandDto> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            return _mapper.Map<BrandDto>(brand);
        }

        /// <summary>
        /// Create brand with transactional logo upload
        /// </summary>
        public async Task<BrandDto> CreateAsync(BrandDto brandDto, Microsoft.AspNetCore.Http.IFormFile? logoFile)
        {
            if (await _brandRepository.GetBrandBySlugAsync(brandDto.Slug) != null)
            {
                throw new ConflictException($"Brand '{brandDto.Slug}' already exists.", "BRAND_DUPLICATE");
            }

            string? uploadedLogoUrl = null;

            await using var transaction = await _brandRepository.BeginTransactionAsync();

            try
            {
                // Upload logo if provided
                if (logoFile != null)
                {
                    uploadedLogoUrl = await _mediaService.UploadBrandLogoAsync(logoFile);
                    brandDto.LogoPath = uploadedLogoUrl;
                }

                var brand = _mapper.Map<Brand>(brandDto);
                var created = await _brandRepository.CreateAsync(brand);

                await transaction.CommitAsync();

                _logger.LogInformation("Created brand {Name} (ID: {Id}) with logo: {Logo}",
                    created.Name, created.Id, uploadedLogoUrl ?? "none");

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.BrandKey);

                return _mapper.Map<BrandDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create brand {Name}", brandDto.Name);
                await transaction.RollbackAsync();

                // Clean up uploaded logo on error
                if (uploadedLogoUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(uploadedLogoUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete logo during rollback"); }
                }

                throw;
            }
        }

        /// <summary>
        /// Update brand with transactional logo upload/delete
        /// </summary>
        public async Task<BrandDto?> UpdateAsync(
            int id, 
            BrandDto brandDto, 
            Microsoft.AspNetCore.Http.IFormFile? logoFile)
        {
            var existing = await _brandRepository.GetBrandByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: Brand with ID {Id} not found", id);
                return null;
            }

            string? oldLogoUrl = existing.LogoPath;
            string? newLogoUrl = null;

            await using var transaction = await _brandRepository.BeginTransactionAsync();

            try
            {
                // Upload new logo if provided
                if (logoFile != null)
                {
                    newLogoUrl = await _mediaService.UploadBrandLogoAsync(logoFile);
                    brandDto.LogoPath = newLogoUrl;
                }
                else
                {
                    brandDto.LogoPath = existing.LogoPath; // Keep existing
                }

                // Update brand
                brandDto.Id = id;
                var brand = _mapper.Map<Brand>(brandDto);
                var updated = await _brandRepository.UpdateAsync(brand);

                if (!updated)
                {
                    throw new InvalidOperationException("Failed to update brand in database");
                }

                await transaction.CommitAsync();

                // Delete old logo if new one was uploaded
                if (newLogoUrl != null && !string.IsNullOrEmpty(oldLogoUrl))
                {
                    try { await _mediaService.DeleteImageAsync(oldLogoUrl); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete old logo"); }
                }

                _logger.LogInformation("Updated brand ID {Id} ({Name})", id, brandDto.Name);

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.BrandKey);

                return brandDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update brand ID {Id}", id);
                await transaction.RollbackAsync();

                // Clean up new uploaded logo on error
                if (newLogoUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(newLogoUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete new logo during rollback"); }
                }

                throw;
            }
        }

        /// <summary>
        /// Delete brand with transactional logo cleanup
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            if (brand == null)
            {
                _logger.LogWarning("Delete failed: Brand with ID {Id} not found", id);
                return false;
            }

            var logoPath = brand.LogoPath;

            await using var transaction = await _brandRepository.BeginTransactionAsync();

            try
            {
                var deleted = await _brandRepository.DeleteAsync(id);
                if (!deleted)
                {
                    return false;
                }

                await transaction.CommitAsync();

                // Clean up logo (best effort - after commit)
                if (!string.IsNullOrEmpty(logoPath))
                {
                    try { await _mediaService.DeleteImageAsync(logoPath); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete logo for brand ID {Id}", id); }
                }

                _logger.LogInformation("Deleted brand ID {Id} ({Name})", id, brand.Name);

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.BrandKey);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete brand ID {Id}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Get brands that have products in a specific category
        /// Only returns brands that have at least one product in the category
        /// </summary>
        public async Task<List<BrandDto>> GetBrandsByCategoryAsync(string categorySlug)
        {
            var cacheKey = $"brands:by-category:{categorySlug}";
            
            return await _cache.CacheOrGetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching brands for category {CategorySlug} from database", categorySlug);
                
                var brands = await _brandRepository.GetBrandsByCategoryAsync(categorySlug);
                return _mapper.Map<List<BrandDto>>(brands);
            }, TimeSpan.FromMinutes(30));
        }
    }
}
