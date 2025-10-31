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
    public class CategoryAdminController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryAdminController> _logger;
        private readonly IMapper _mapper;
        private readonly MediaService _Media;
        public CategoryAdminController(CategoryService categoryService, ILogger<CategoryAdminController> logger, IMapper mapper, MediaService media)
        {
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
            _Media = media;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryApiDto createCategoryFormDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CategoryTreeDto>.Fail("Invalid category data"));

            string? iconUrl = null;
            if (createCategoryFormDto != null && createCategoryFormDto.IconFile != null)
            {
                iconUrl = await _Media.UploadCategoryIconAsync(createCategoryFormDto.IconFile);
            }

            var categoryDto = new CategoryDto 
            {
                Name = createCategoryFormDto.Name,
                Slug = createCategoryFormDto.Slug,
                ParentId = createCategoryFormDto.ParentId,
                IconPath = iconUrl
            };

            var created = await _categoryService.CreateAsync(categoryDto);
            var mapped = _mapper.Map<CategoryTreeDto>(_mapper.Map<CategoryDto>(created));
            return Ok(ApiResponse<CategoryTreeDto>.Ok(mapped, "Category created successfully"));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryApiDto apiDto)
        {
            var categoryDto = _mapper.Map<CategoryDto>(apiDto);
            var updated = await _categoryService.UpdateAsync(categoryDto);
            if (!updated)
            {
                return BadRequest(ApiResponse<UpdateCategoryApiDto>.Fail("Failed to update category"));
            }
            return Ok(ApiResponse<UpdateCategoryApiDto>.Ok(apiDto, "Category updated successfully"));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryApiDto apiDto)
        {
            var categoryDto = await _categoryService.GetCategoryByIdAsync(apiDto.Id);
            if (categoryDto == null)
            {
                return NotFound(ApiResponse<DeleteCategoryApiDto>.Fail("Category not found"));
            }
            var iconPath = categoryDto.IconPath;
            var deleted = await _categoryService.DeleteAsync(categoryDto);
            if (!deleted)
            {
                return BadRequest(ApiResponse<DeleteCategoryApiDto>.Fail("Failed to delete category"));
            }
            if (!string.IsNullOrEmpty(iconPath))
            {
                await _Media.DeleteImageAsync(iconPath);
            }
            return Ok(ApiResponse<DeleteCategoryApiDto>.Ok(apiDto, "Category deleted successfully"));
        }
    }
}