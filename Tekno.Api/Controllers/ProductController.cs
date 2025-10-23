using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? category,
            [FromQuery] string? brand,
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? minPrice,
            [FromQuery] string? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            var result = await _productService.GetPagedProductAsync(
                category, brand, search, sort, minPrice, maxPrice,
                new PagingParams(page, pageSize));

            return Ok(ApiResponse<PagedResult<ProductSummaryDto>>.Ok(result));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetDetail(string slug)
        {
            var product = await _productService.GetProductDetailAsync(slug);
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found"));

            return Ok(ApiResponse<ProductDetailDto>.Ok(product));
        }
    }
}
