using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Common.Responses;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Auth;

namespace Tekno.Api.Controllers.admin
{
    [ApiController]
    [Route("api/admin/products")]
    //[Authorize(Roles = "Admin")]
    public class ProductAdminController : ControllerBase
    {
        private readonly ILogger<ProductAdminController> _logger;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly MediaService _mediaService;
        public ProductAdminController(ILogger<ProductAdminController> logger, ProductService productService, IMapper mapper, MediaService mediaService)
        {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
            _mediaService = mediaService;
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
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto) {
            var product = await _productService.CreateProductAsync(createProductDto);
            return Ok(ApiResponse<CreateProductDto>.Ok(product, "Product created successfully"));
        } 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateProductDto dto)
        {
            var updated = await _productService.UpdateProductAsync(id, dto);
            if (updated == null) return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found"));
            return Ok(ApiResponse<ProductDetailDto>.Ok(updated, "Product updated"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var ok = await _productService.DeleteProductAsync(id);
            if (!ok) return NotFound(ApiResponse<object>.Fail("Product not found"));
            return Ok(ApiResponse<object>.Ok(null, "Product deleted"));
        }
    }
}
