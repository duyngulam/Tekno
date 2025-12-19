using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Models.Catalog;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Api.Commons.Responses;
using Tekno.Api.Models.Catalog.Admin.Category;

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
        private readonly CategoryAttributeService _attributeService;

        public AdminCategoryController(
            CategoryService categoryService,
            ILogger<AdminCategoryController> logger,
            IMapper mapper,
            CategoryAttributeService attributeService)
        {
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
            _attributeService = attributeService;
        }

        // GET /api/admin/categories - Paginated list
        [HttpGet]
        public async Task<IActionResult> GetCategoriesPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _categoryService.GetPagedAsync(search, page, pageSize);
            return Ok(ApiResponse<PagedResult<CategoryDto>>.Ok(result, "Categories loaded successfully"));
        }

        // GET /api/admin/categories/list - All categories (kept for backward compatibility)
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
        /// Get available attributes for a category (for variant creation)
        /// </summary>
        /// <remarks>
        /// Returns all attributes available for a specific category, including:
        /// - Category-specific attributes
        /// - Global attributes (shared across all categories)
        /// Each attribute includes its possible values.
        /// Used when creating product variants to know which attributes can be selected.
        /// </remarks>
        [HttpGet("{categoryId:int}/attributes")]
        public async Task<IActionResult> GetCategoryAttributes(int categoryId)
        {
            try
            {
                var attributes = await _categoryService.GetAttributesByCategoryIdAsync(categoryId);
                return Ok(ApiResponse<List<ProductAttributeDto>>.Ok(attributes, "Attributes loaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attributes for category {CategoryId}", categoryId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get attributes: {ex.Message}"));
            }
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

        // ===== Attribute management endpoints=====

        /// <summary>
        /// Get a specific attribute by ID
        /// </summary>
        [HttpGet("attributes/{attributeId}")]
        public async Task<IActionResult> GetAttribute(int attributeId)
        {
            try
            {
                var attribute = await _attributeService.GetAttributeByIdAsync(attributeId);
                if (attribute == null)
                    return NotFound(ApiResponse<CategoryAttributeDto>.Fail("Attribute not found"));

                return Ok(ApiResponse<CategoryAttributeDto>.Ok(attribute));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attribute {AttributeId}", attributeId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get attribute: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new attribute
        /// </summary>
        [HttpPost("attributes")]
        public async Task<IActionResult> CreateAttribute([FromBody] CreateAttributeDto dto)
        {
            try
            {
                var attribute = await _attributeService.CreateAttributeAsync(dto);
                return Ok(ApiResponse<CategoryAttributeDto>.Ok(attribute, "Attribute created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create attribute {Name}", dto.Name);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to create attribute: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update an existing attribute
        /// </summary>
        [HttpPut("attributes/{attributeId}")]
        public async Task<IActionResult> UpdateAttribute(int attributeId, [FromBody] UpdateAttributeDto dto)
        {
            try
            {
                var attribute = await _attributeService.UpdateAttributeAsync(attributeId, dto);
                if (attribute == null)
                    return NotFound(ApiResponse<CategoryAttributeDto>.Fail("Attribute not found"));

                return Ok(ApiResponse<CategoryAttributeDto>.Ok(attribute, "Attribute updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update attribute {AttributeId}", attributeId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update attribute: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete an attribute (only if not in use)
        /// </summary>
        [HttpDelete("attributes/{attributeId}")]
        public async Task<IActionResult> DeleteAttribute(int attributeId)
        {
            try
            {
                var deleted = await _attributeService.DeleteAttributeAsync(attributeId);
                if (!deleted)
                    return NotFound(ApiResponse<bool>.Fail("Attribute not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Attribute deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete attribute {AttributeId}", attributeId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete attribute: {ex.Message}"));
            }
        }

        /// <summary>
        /// Add a new value to an attribute
        /// </summary>
        [HttpPost("attributes/values")]
        public async Task<IActionResult> AddAttributeValue([FromBody] AddAttributeValueDto dto)
        {
            try
            {
                var value = await _attributeService.AddAttributeValueAsync(dto);
                return Ok(ApiResponse<AttributeValueDto>.Ok(value, "Value added successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add value to attribute {AttributeId}", dto.AttributeId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to add value: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update an attribute value
        /// </summary>
        [HttpPut("attributes/values/{valueId}")]
        public async Task<IActionResult> UpdateAttributeValue(int valueId, [FromBody] UpdateAttributeValueDto dto)
        {
            try
            {
                dto.ValueId = valueId;
                var value = await _attributeService.UpdateAttributeValueAsync(dto);
                if (value == null)
                    return NotFound(ApiResponse<AttributeValueDto>.Fail("Value not found"));

                return Ok(ApiResponse<AttributeValueDto>.Ok(value, "Value updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update value {ValueId}", valueId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update value: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete an attribute value (only if not in use)
        /// </summary>
        [HttpDelete("attributes/values/{valueId}")]
        public async Task<IActionResult> DeleteAttributeValue(int valueId)
        {
            try
            {
                var deleted = await _attributeService.DeleteAttributeValueAsync(valueId);
                if (!deleted)
                    return NotFound(ApiResponse<bool>.Fail("Value not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Value deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete value {ValueId}", valueId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete value: {ex.Message}"));
            }
        }
    }
}