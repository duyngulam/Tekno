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
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Application.Catalog.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly MediaService _mediaService;
        private readonly IAppLogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IMapper mapper, 
            ICacheService cacheService,
            MediaService mediaService,
            IAppLogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cache = cacheService;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _cache.CacheOrGetAsync(
                CachePolicies.CategoryKey,
                async () => _mapper.Map<List<CategoryDto>>(await _categoryRepository.GetAllCategoriesAsync()),
                CachePolicies.CategoryTtl
            );
        }

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(string? search = null, int page = 1, int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _categoryRepository.GetPagedAsync(search, paging);
            
            var dtos = _mapper.Map<List<CategoryDto>>(result.Data);
            return new PagedResult<CategoryDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var dtoList = _mapper.Map<List<CategoryDto>>(categories);

            var lookup = dtoList.ToLookup(c => c.ParentId);

            List<CategoryTreeDto> BuildTree(int? parentId)
            {
                return lookup[parentId]
                    .Select(c => new CategoryTreeDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Slug = c.Slug,
                        IconPath = c.IconPath,
                        ImageUrl = c.ImageUrl, // NEW
                        SubCategories = BuildTree(c.Id)
                    }).ToList();
            }

            return BuildTree(null); // categories cha (ParentId = null)
        }

        public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            var category = await _categoryRepository.GetCategoryBySlugAsync(slug);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        /// <summary>
        /// Create category with transactional image upload
        /// </summary>
        public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto, Microsoft.AspNetCore.Http.IFormFile? iconFile, Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            if (await _categoryRepository.GetCategoryBySlugAsync(categoryDto.Slug) != null)
            {
                throw new ConflictException($"Category '{categoryDto.Slug}' already exists.", "CATEGORY_DUPLICATE");
            }

            string? uploadedIconUrl = null;
            string? uploadedImageUrl = null;

            await using var transaction = await _categoryRepository.BeginTransactionAsync();

            try
            {
                // Upload icon if provided
                if (iconFile != null)
                {
                    uploadedIconUrl = await _mediaService.UploadCategoryIconAsync(iconFile);
                    categoryDto.IconPath = uploadedIconUrl;
                }

                // Upload image if provided
                if (imageFile != null)
                {
                    uploadedImageUrl = await _mediaService.UploadImageAsync(imageFile, $"tekno/category/{categoryDto.Slug}");
                    categoryDto.ImageUrl = uploadedImageUrl;
                }

                var category = _mapper.Map<Category>(categoryDto);
                var created = await _categoryRepository.CreateAsync(category);

                await transaction.CommitAsync();

                _logger.LogInformation("Created category {Name} (ID: {Id}) with icon: {Icon}, image: {Image}",
                    created.Name, created.Id, uploadedIconUrl ?? "none", uploadedImageUrl ?? "none");

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.CategoryKey);

                return _mapper.Map<CategoryDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category {Name}", categoryDto.Name);
                await transaction.RollbackAsync();

                // Clean up uploaded files
                if (uploadedIconUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(uploadedIconUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete icon during rollback"); }
                }

                if (uploadedImageUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(uploadedImageUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete image during rollback"); }
                }

                throw;
            }
        }

        /// <summary>
        /// Update category with transactional image upload/delete
        /// </summary>
        public async Task<CategoryDto?> UpdateAsync(
            int id, 
            CategoryDto categoryDto, 
            Microsoft.AspNetCore.Http.IFormFile? iconFile, 
            Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            var existing = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: Category with ID {Id} not found", id);
                return null;
            }

            string? oldIconUrl = existing.IconPath;
            string? oldImageUrl = existing.ImageUrl;
            string? newIconUrl = null;
            string? newImageUrl = null;

            await using var transaction = await _categoryRepository.BeginTransactionAsync();

            try
            {
                // Upload new icon if provided
                if (iconFile != null)
                {
                    newIconUrl = await _mediaService.UploadCategoryIconAsync(iconFile);
                    categoryDto.IconPath = newIconUrl;
                }
                else
                {
                    categoryDto.IconPath = existing.IconPath; // Keep existing
                }

                // Upload new image if provided
                if (imageFile != null)
                {
                    newImageUrl = await _mediaService.UploadImageAsync(imageFile, $"tekno/category/{categoryDto.Slug}");
                    categoryDto.ImageUrl = newImageUrl;
                }
                else
                {
                    categoryDto.ImageUrl = existing.ImageUrl; // Keep existing
                }

                // Update category
                categoryDto.Id = id;
                var category = _mapper.Map<Category>(categoryDto);
                var updated = await _categoryRepository.UpdateAsync(category);

                if (!updated)
                {
                    throw new InvalidOperationException("Failed to update category in database");
                }

                await transaction.CommitAsync();

                // Delete old files if new ones were uploaded
                if (newIconUrl != null && !string.IsNullOrEmpty(oldIconUrl))
                {
                    try { await _mediaService.DeleteImageAsync(oldIconUrl); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete old icon"); }
                }

                if (newImageUrl != null && !string.IsNullOrEmpty(oldImageUrl))
                {
                    try { await _mediaService.DeleteImageAsync(oldImageUrl); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete old image"); }
                }

                _logger.LogInformation("Updated category ID {Id} ({Name})", id, categoryDto.Name);

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.CategoryKey);

                return categoryDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category ID {Id}", id);
                await transaction.RollbackAsync();

                // Clean up new uploaded files
                if (newIconUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(newIconUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete new icon during rollback"); }
                }

                if (newImageUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(newImageUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete new image during rollback"); }
                }

                throw;
            }
        }

        /// <summary>
        /// Delete category with transactional image cleanup
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Delete failed: Category with ID {Id} not found", id);
                return false;
            }

            var iconPath = category.IconPath;
            var imageUrl = category.ImageUrl;

            await using var transaction = await _categoryRepository.BeginTransactionAsync();

            try
            {
                var deleted = await _categoryRepository.DeleteAsync(id);
                if (!deleted)
                {
                    return false;
                }

                await transaction.CommitAsync();

                // Clean up images (best effort - after commit)
                if (!string.IsNullOrEmpty(iconPath))
                {
                    try { await _mediaService.DeleteImageAsync(iconPath); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete icon for category ID {Id}", id); }
                }

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    try { await _mediaService.DeleteImageAsync(imageUrl); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete image for category ID {Id}", id); }
                }

                _logger.LogInformation("Deleted category ID {Id} ({Name})", id, category.Name);

                // Invalidate cache
                await _cache.RemoveAsync(CachePolicies.CategoryKey);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category ID {Id}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }

        // NEW: expose attributes for a category (includes global attributes)
        public async Task<List<ProductAttributeDto>> GetAttributesByCategoryIdAsync(int categoryId)
        {
            var attrs = await _categoryRepository.GetAttributesForCategoryAsync(categoryId);
            return _mapper.Map<List<ProductAttributeDto>>(attrs);
        }
    }
}
