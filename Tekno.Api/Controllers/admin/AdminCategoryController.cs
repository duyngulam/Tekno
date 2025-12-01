using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Api.Models.Catalog.Admin;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Media.Services;

namespace Tekno.Api.Controllers.admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/categories")]
    [ValidationFilter]
    public class AdminCategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<AdminCategoryController> _logger;
        private readonly IMapper _mapper;

        public AdminCategoryController(
            CategoryService categoryService, 
            ILogger<AdminCategoryController> logger, 
            IMapper mapper)
        {
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var result = _mapper.Map<List<CategoryTreeDto>>(categories);
            return Ok(ApiResponse<List<CategoryTreeDto>>.Ok(result, "Categories loaded successfully"));
        }

        // New endpoint: returns hierarchical category tree for admin
        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoryTree()
        {
            var categoryTree = await _categoryService.GetCategoryTreeAsync();
            var result = _mapper.Map<List<CategoryTreeDto>>(categoryTree);
            return Ok(ApiResponse<List<CategoryTreeDto>>.Ok(result, "Category tree loaded successfully"));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            var category = await _categoryService.GetCategoryBySlugAsync(slug);
            if (category == null)
                return NotFound(ApiResponse<string>.Fail("Category not found"));
            
            var result = _mapper.Map<CategoryTreeDto>(category);
            return Ok(ApiResponse<CategoryTreeDto>.Ok(result, "Category loaded successfully"));
        }

        /// <summary>
        /// Create category with optional icon and image
        /// </summary>
        /// <remarks>
        /// Upload icon (small) and/or main category image (large banner).
        /// Both files are optional. Transactions ensure no orphan images.
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryApiDto createDto)
        {
            try
            {
                var categoryDto = new CategoryDto 
                {
                    Name = createDto.Name,
                    Slug = createDto.Slug,
                    ParentId = createDto.ParentId,
                    Description = createDto.Description
                };

                var created = await _categoryService.CreateAsync(categoryDto, createDto.IconFile, createDto.ImageFile);
                var mapped = _mapper.Map<CategoryDto>(created);
                
                return Ok(ApiResponse<CategoryDto>.Ok(mapped, "Category created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category {Name}", createDto.Name);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to create category: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update category with optional new icon and/or image
        /// </summary>
        /// <remarks>
        /// Only provide IconFile/ImageFile if you want to change them.
        /// Old images are automatically deleted when new ones are uploaded.
        /// Transactions ensure no orphan images.
        /// </remarks>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryApiDto apiDto)
        {
            try
            {
                var categoryDto = new CategoryDto
                {
                    Name = apiDto.Name,
                    Slug = apiDto.Slug,
                    ParentId = apiDto.ParentId,
                    Description = apiDto.Description
                };

                var updated = await _categoryService.UpdateAsync(apiDto.Id, categoryDto, apiDto.IconFile, apiDto.ImageFile);
                
                if (updated == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Category not found"));
                }

                return Ok(ApiResponse<CategoryDto>.Ok(updated, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category ID {Id}", apiDto.Id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update category: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete category and all associated images
        /// </summary>
        /// <remarks>
        /// Deletes category from database and cleans up icon and image files.
        /// Transactions ensure database consistency.
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryApiDto apiDto)
        {
            try
            {
                var deleted = await _categoryService.DeleteAsync(apiDto.Id);
                
                if (!deleted)
                {
                    return NotFound(ApiResponse<string>.Fail("Category not found"));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Category deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category ID {Id}", apiDto.Id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete category: {ex.Message}"));
            }
        }
    }
}