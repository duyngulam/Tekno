using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Tekno.Api.Common.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Api.Models.Catalog.Admin.brand;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Media.Services;

namespace Tekno.Api.Controllers.admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/brands")]
    public class BrandAdminController : ControllerBase
    {
        private readonly BrandService _brandService;
        private readonly ILogger<BrandAdminController> _logger;
        private readonly IMapper _mapper;
        private readonly MediaService _Media;
        public BrandAdminController(BrandService brandService, ILogger<BrandAdminController> logger, IMapper mapper, MediaService media)
        {
            _brandService = brandService;
            _logger = logger;
            _mapper = mapper;
            _Media = media;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            var result = _mapper.Map<List<BrandDto>>(brands);
            return Ok(ApiResponse<List<BrandDto>>.Ok(result, "Brands loaded successfully"));
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBrandBySlug(string slug)
        {
            var brand = await _brandService.GetBrandBySlugAsync(slug);
            var result = _mapper.Map<BrandDto>(brand);
            return Ok(ApiResponse<BrandDto>.Ok(result, "Brand loaded successfully"));
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateBrand([FromForm] CreateBrandApiDto createBrandFormDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<BrandDto>.Fail("Invalid brand data"));
            string? logoUrl = null;
            if (createBrandFormDto != null && createBrandFormDto.LogoFile != null) {
                logoUrl = await _Media.UploadBrandLogoAsync(createBrandFormDto.LogoFile);
            }
            var brandDto = new BrandDto
            {
                Name = createBrandFormDto.Name,
                Slug = createBrandFormDto.Slug,
                LogoPath = logoUrl,
            };
            var createdBrand = await _brandService.CreateAsync(brandDto);
            return Ok(ApiResponse<BrandDto>.Ok(brandDto, "Brand Create successfully"));

        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBrand([FromBody]UpdateBrandApiDto apiDto)
        {
            var brandDto = _mapper.Map<BrandDto>(apiDto);
            var updated = await _brandService.UpdateAsync(brandDto);
            if (!updated)
            {
                return BadRequest(ApiResponse<UpdateBrandApiDto>.Fail("Failed to update brand"));
            }
            return Ok(ApiResponse<UpdateBrandApiDto>.Ok(apiDto, "Brand updated successfully"));
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBrand([FromBody] DeleteBrandApiDto apiDto)
        {
            var brandDto = await _brandService.GetBrandByIdAsync(apiDto.Id);
            if (brandDto == null)
            {
                return NotFound(ApiResponse<DeleteBrandApiDto>.Fail("Brand not found"));
            }
            var logoPath = brandDto.LogoPath;
            var deleted = await _brandService.DeleteAsync(brandDto);
            if (!deleted)
            {
                return BadRequest(ApiResponse<DeleteBrandApiDto>.Fail("Failed to delete brand"));
            }
            if (!string.IsNullOrEmpty(logoPath))
            {
                await _Media.DeleteImageAsync(logoPath);
            }
            return Ok(ApiResponse<DeleteBrandApiDto>.Ok(apiDto, "Brand deleted successfully"));
        }
    }
}
