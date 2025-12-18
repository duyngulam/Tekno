using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Commons.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Api.Models.Catalog.Admin.brand;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;

namespace Tekno.Api.Controllers.admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/brands")]
    public class AdminBrandController : ControllerBase
    {
        private readonly BrandService _brandService;
        private readonly ILogger<AdminBrandController> _logger;
        private readonly IMapper _mapper;

        public AdminBrandController(
            BrandService brandService, 
            ILogger<AdminBrandController> logger, 
            IMapper mapper)
        {
            _brandService = brandService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET /api/admin/brands - Paginated list
        [HttpGet]
        public async Task<IActionResult> GetBrandsPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _brandService.GetPagedAsync(search, page, pageSize);
            return Ok(ApiResponse<PagedResult<BrandDto>>.Ok(result, "Brands loaded successfully"));
        }

        // GET /api/admin/brands/list - All brands (kept for backward compatibility)
        [HttpGet("list")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(ApiResponse<List<BrandDto>>.Ok(brands, "Brands loaded successfully"));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBrandBySlug(string slug)
        {
            var brand = await _brandService.GetBrandBySlugAsync(slug);
            if (brand == null)
                return NotFound(ApiResponse<BrandDto>.Fail("Brand not found"));

            return Ok(ApiResponse<BrandDto>.Ok(brand, "Brand loaded successfully"));
        }

        /// <summary>
        /// Create brand with optional logo
        /// </summary>
        /// <remarks>
        /// Upload logo file (optional). Transactions ensure no orphan images.
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> CreateBrand([FromForm] CreateBrandApiDto createDto)
        {
            try
            {
                var brandDto = new BrandDto
                {
                    Name = createDto.Name,
                    Slug = createDto.Slug,
                    Country = createDto.Country
                };

                var created = await _brandService.CreateAsync(brandDto, createDto.LogoFile);
                
                return Ok(ApiResponse<BrandDto>.Ok(created, "Brand created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create brand {Name}", createDto.Name);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to create brand: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update brand with optional new logo
        /// </summary>
        /// <remarks>
        /// Only provide LogoFile if you want to change the logo.
        /// Old logo is automatically deleted when new one is uploaded.
        /// Transactions ensure no orphan images.
        /// </remarks>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBrand([FromForm] UpdateBrandApiDto apiDto)
        {
            try
            {
                var brandDto = new BrandDto
                {
                    Name = apiDto.Name,
                    Slug = apiDto.Slug,
                    Country = apiDto.Country
                };

                var updated = await _brandService.UpdateAsync(apiDto.Id, brandDto, apiDto.LogoFile);
                
                if (updated == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Brand not found"));
                }

                return Ok(ApiResponse<BrandDto>.Ok(updated, "Brand updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update brand ID {Id}", apiDto.Id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update brand: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete brand and logo
        /// </summary>
        /// <remarks>
        /// Deletes brand from database and cleans up logo file.
        /// Transactions ensure database consistency.
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBrand([FromBody] DeleteBrandApiDto apiDto)
        {
            try
            {
                var deleted = await _brandService.DeleteAsync(apiDto.Id);
                
                if (!deleted)
                {
                    return NotFound(ApiResponse<string>.Fail("Brand not found"));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Brand deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete brand ID {Id}", apiDto.Id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete brand: {ex.Message}"));
            }
        }
    }
}
