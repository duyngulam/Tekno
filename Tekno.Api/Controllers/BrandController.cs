using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Application.Catalog.Services;
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
        // GET /api/brands/list
        [HttpGet("list")]
        public async Task<IActionResult> GertBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            var result = _mapper.Map<List<BrandApiDto>>(brands);
            return Ok(ApiResponse<List<BrandApiDto>>.Ok(result, "Brands loaded successfully"));
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBrandBySlug(string slug)
        {
            var brand = await _brandService.GetBrandBySlugAsync(slug);
            if (brand == null)
            {
                return NotFound(ApiResponse<string>.Fail("Brand not found"));
            }
            var result = _mapper.Map<BrandApiDto>(brand);
            return Ok(ApiResponse<BrandApiDto>.Ok(result, "Brand loaded successfully"));
        }
    }
}
