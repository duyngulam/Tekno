using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Commons.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Paging;
using Tekno.Infrastructure.Catalog;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/brands")]
    [ValidationFilter]
    public class BrandController : ControllerBase
    {
        private readonly BrandService _brandService;
        private readonly IMapper _mapper;
        public BrandController(BrandService brandService, IMapper mapper) {
            _brandService = brandService;
            _mapper = mapper;
        }

        // GET /api/brands - Paginated list
        [HttpGet]
        public async Task<IActionResult> GetBrandsPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _brandService.GetPagedAsync(search, page, pageSize);
            return Ok(ApiResponse<PagedResult<BrandDto>>.Ok(result, "Brands loaded successfully"));
        }

        // GET /api/brands/list - All brands (kept for backward compatibility)
        [HttpGet("list")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            var result = _mapper.Map<List<BrandDto>>(brands);
            return Ok(ApiResponse<List<BrandDto>>.Ok(result, "Brands loaded successfully"));
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBrandBySlug(string slug)
        {
            var brand = await _brandService.GetBrandBySlugAsync(slug);
            if (brand == null)
            {
                return NotFound(ApiResponse<string>.Fail("Brand not found"));
            }
            var result = _mapper.Map<BrandDto>(brand);
            return Ok(ApiResponse<BrandDto>.Ok(result, "Brand loaded successfully"));
        }

        /// <summary>
        /// Get brands by category - Only brands that have products in the specified category
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Returns a list of brands that have at least one product in the specified category.
        /// This ensures that when filtering products by category, only relevant brands are shown.

        /// ## Validation
        /// - **categorySlug**: Required, must be valid URL slug
        /// - **categorySlug**: Only lowercase, numbers, hyphens allowed
        /// </remarks>
        /// <param name="categorySlug">Category URL slug (e.g., "laptops", "smartphones")</param>
        /// <response code="200">Returns list of brands with products in category</response>
        [HttpGet("by-category/{categorySlug}")]
        [ProducesResponseType(typeof(ApiResponse<List<BrandDto>>), 200)]
        public async Task<IActionResult> GetBrandsByCategory(string categorySlug)
        {
            var brands = await _brandService.GetBrandsByCategoryAsync(categorySlug);
            return Ok(ApiResponse<List<BrandDto>>.Ok(
                brands,
                $"Retrieved {brands.Count} brands in category"));
        }
    }
}
