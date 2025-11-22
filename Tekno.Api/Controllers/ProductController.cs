using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Products;
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
        public async Task<IActionResult> GetPaged([FromQuery] ProductSearchRequestDto request)
        {
            var result = await _productService.GetPagedProductAsync(request);
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

        [HttpGet("variants/{ :int}")]
        public async Task<IActionResult> GetVariantById(int variantId)
        {
            var variant = await _productService.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return NotFound(ApiResponse<ProductVariantDetailDto>.Fail("Product variant not found"));

            return Ok(ApiResponse<ProductVariantDetailDto>.Ok(variant));
        }
    }
}
