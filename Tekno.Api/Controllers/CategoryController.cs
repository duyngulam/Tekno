using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Catalog.DTOs;
using Tekno.Api.Models.Catalog;
using AutoMapper;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Common.Paging;
using System.Linq;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [ValidationFilter]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(CategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET /api/categories - Paginated list
        [HttpGet]
        public async Task<IActionResult> GetCategoriesPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _categoryService.GetPagedAsync(search, page, pageSize);
            return Ok(ApiResponse<PagedResult<CategoryDto>>.Ok(result, "Categories loaded successfully"));
        }

        // GET /api/categories/list - All categories (kept for backward compatibility)
        [HttpGet("list")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var result = _mapper.Map<List<CategoryDto>>(categories);

            return Ok(ApiResponse<List<CategoryDto>>.Ok(result, "Categories loaded successfully"));
        }

        // GET /api/categories/tree
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
            var categories = await _categoryService.GetCategoryTreeAsync();
            var category = categories.FirstOrDefault(c => c.Slug == slug);
            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail("Category not found"));
            }
            var result = _mapper.Map<CategoryTreeDto>(category);
            return Ok(ApiResponse<CategoryTreeDto>.Ok(result, "Category loaded successfully"));
        }

        // GET /api/categories/{id}/attributes
        [HttpGet("{id:int}/attributes")]
        public async Task<IActionResult> GetAttributesByCategoryId(int id)
        {
            var attributes = await _categoryService.GetAttributesByCategoryIdAsync(id);
            return Ok(ApiResponse<List<ProductAttributeDto>>.Ok(attributes, "Attributes loaded"));
        }
        [HttpGet("{slug}/attributes")]
        public async Task<IActionResult> GetAttributesBySlugAsync(string slug)
        {
            var attributes = await _categoryService.GetAttributesByCategorySlugAsync(slug);
            return Ok(ApiResponse<List<ProductAttributeDto>>.Ok(attributes,"Attributes loaded succesfully"));
        }
    }
}
