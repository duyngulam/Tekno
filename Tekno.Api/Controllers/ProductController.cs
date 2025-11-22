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

        [HttpGet("variants/{variantId:int}")]
        public async Task<IActionResult> GetVariantById(int variantId)
        {
            var variant = await _productService.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return NotFound(ApiResponse<ProductVariantDetailDto>.Fail("Product variant not found"));

            return Ok(ApiResponse<ProductVariantDetailDto>.Ok(variant));
        }

        /// <summary>
        /// Get top N newest products by category
        /// </summary>
        /// <param name="categorySlug">Category slug (e.g., "laptops")</param>
        /// <param name="count">Number of products to return (default: 10, max: 100)</param>
        /// <remarks>
        /// Returns the newest products in a category, sorted by creation date (newest first)
        /// 
        /// Examples:
        /// - GET /api/products/new/laptops?count=5 - Get 5 newest laptops
        /// - GET /api/products/new/smartphones?count=10 - Get 10 newest smartphones
        /// </remarks>
        [HttpGet("new/{categorySlug}")]
        public async Task<IActionResult> GetTopNewByCategory(
            string categorySlug,
            [FromQuery] int count = 10)
        {
            var products = await _productService.GetTopNewProductsByCategoryAsync(categorySlug, count);
            return Ok(ApiResponse<System.Collections.Generic.List<ProductSummaryDto>>.Ok(
                products, 
                $"Retrieved {products.Count} newest products"));
        }
    }
}
