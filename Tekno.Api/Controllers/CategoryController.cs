using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Catalog.DTOs;
using Tekno.Api.Models.Catalog;
using AutoMapper;

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

        // GET /api/categories/landing
        [HttpGet("landing")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var result = _mapper.Map<List<CategoryTreeLandingDto>>(categories);

            return Ok(ApiResponse<List<CategoryTreeLandingDto>>.Ok(result, "Categories loaded successfully"));
        }

        // GET /api/categories/tree
        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoryTree()
        {
            var categoryTree = await _categoryService.GetCategoryTreeAsync();
            var result = _mapper.Map<List<CategoryTreeLandingDto>>(categoryTree);

            return Ok(ApiResponse<List<CategoryTreeLandingDto>>.Ok(result, "Category tree loaded successfully"));
        }
    }
}
