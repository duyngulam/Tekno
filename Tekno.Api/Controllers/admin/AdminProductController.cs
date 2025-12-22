using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Paging;

namespace Tekno.Api.Controllers.admin
{
    [ApiController]
    [Route("api/admin/products")]
    //[Authorize(Roles = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly ILogger<AdminProductController> _logger;
        private readonly ProductService _productService;

        public AdminProductController(
            ILogger<AdminProductController> logger,
            ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        // Get paginated admin product list with full details (variants, images, stock)
        [HttpGet]
        public async Task<IActionResult> GetPagedAdmin([FromQuery] AdminProductSearchDto request)
        {
            var result = await _productService.GetAdminProductsPagedAsync(request);
            return Ok(ApiResponse<PagedResult<AdminProductListDto>>.Ok(result));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetDetail(string slug)
        {
            var product = await _productService.GetProductDetailAsync(slug);
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found"));

            return Ok(ApiResponse<ProductDetailDto>.Ok(product));
        }

        /// Create product with multiple images
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(dto);
                return Ok(ApiResponse<CreateProductDto>.Ok(product, "Product created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product {Name}", dto.Name);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to create product: {ex.Message}"));
            }
        }

        // Update product replace images
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateProductDto dto)
        {
            try
            {
                var updated = await _productService.UpdateProductAsync(id, dto);
                if (updated == null)
                    return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found"));

                return Ok(ApiResponse<ProductDetailDto>.Ok(updated, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product {Id}", id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update product: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var ok = await _productService.DeleteProductAsync(id);
                if (!ok)
                    return NotFound(ApiResponse<object>.Fail("Product not found"));

                return Ok(ApiResponse<object>.Ok(null, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product {Id}", id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete product: {ex.Message}"));
            }
        }
        
        [HttpPost("images")]
        public async Task<IActionResult> AddProductImage([FromForm] AddProductImageDto dto)
        {
            try
            {
                var image = await _productService.AddProductImageAsync(dto);
                return Ok(ApiResponse<ProductImageDto>.Ok(image, "Image added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add image to product {ProductId}", dto.ProductId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to add image: {ex.Message}"));
            }
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            try
            {
                var ok = await _productService.DeleteProductImageAsync(imageId);
                if (!ok)
                    return NotFound(ApiResponse<bool>.Fail("Image not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Image deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image {ImageId}", imageId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete image: {ex.Message}"));
            }
        }

        // Update product image (set primary, change order)
        [HttpPut("images/{imageId}")]
        public async Task<IActionResult> UpdateProductImage(int imageId, [FromBody] UpdateProductImageDto dto)
        {
            try
            {
                dto.ImageId = imageId;
                var ok = await _productService.UpdateProductImageAsync(dto);
                if (!ok)
                    return NotFound(ApiResponse<bool>.Fail("Image not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Image updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update image {ImageId}", imageId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update image: {ex.Message}"));
            }
        }

        /// Reorder product images
        [HttpPost("images/reorder")]
        public async Task<IActionResult> ReorderImages([FromBody] ReorderImagesDto dto)
        {
            try
            {
                var images = await _productService.ReorderProductImagesAsync(dto);
                return Ok(ApiResponse<List<ProductImageDto>>.Ok(images, "Images reordered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reorder images for product {ProductId}", dto.ProductId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to reorder images: {ex.Message}"));
            }
        }

        // Add variant to product (auto-updates product specs)
        [HttpPost("variants")]
        public async Task<IActionResult> AddProductVariant([FromBody] AddProductVariantDto dto)
        {
            try
            {
                var variant = await _productService.AddProductVariantAsync(dto);
                return Ok(ApiResponse<ProductVariantDetailDto>.Ok(variant, "Variant added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add variant to product {ProductId}", dto.ProductId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to add variant: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a product variant safely inside a transaction
        /// </summary>
        [HttpPut("variants/{variantId:int}")]
        public async Task<IActionResult> UpdateProductVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
        {
            try
            {
                var updated = await _productService.UpdateProductVariantAsync(variantId, dto);
                if (updated == null)
                    return NotFound(ApiResponse<ProductVariantDetailDto>.Fail("Variant not found"));

                return Ok(ApiResponse<ProductVariantDetailDto>.Ok(updated, "Variant updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update variant {VariantId}", variantId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to update variant: {ex.Message}"));
            }
        }
        /// Delete product variant (auto-updates product specs)
        [HttpDelete("variants/{variantId}")]
        public async Task<IActionResult> DeleteProductVariant(int variantId)
        {
            try
            {
                var ok = await _productService.DeleteProductVariantAsync(variantId);
                if (!ok)
                    return NotFound(ApiResponse<bool>.Fail("Variant not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Variant deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete variant {VariantId}", variantId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to delete variant: {ex.Message}"));
            }
        }
    }
}