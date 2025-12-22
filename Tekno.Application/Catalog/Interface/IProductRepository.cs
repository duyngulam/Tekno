using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPagedProductAsync(string? categorySlug,
            string? brandSlug,
            string? search,
            string? sort,
            string? minPrice,
            string? maxPrice,
            PagingParams paging);

        Task<Product> GetProductBySlugAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> IsProductExistBySlugAsync(string slug);
        Task<bool> IsProductExistByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsWithDetailAsync();
        Task<Product> AddProductAsync(Product newProduct);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        // Admin product listing
        Task<PagedResult<Product>> GetAdminProductsPagedAsync(
            string? search,
            string? categorySlug,
            string? brandSlug,
            string? status,
            PagingParams paging);

        // Image management
        Task<ProductImage?> GetProductImageByIdAsync(int imageId);
        Task<ProductImage> AddProductImageAsync(ProductImage image);
        Task<bool> UpdateProductImageAsync(ProductImage image);
        Task<bool> DeleteProductImageAsync(int imageId);
        Task<List<ProductImage>> GetProductImagesAsync(int productId);

        // Variant management
        Task<ProductVariant> AddProductVariantAsync(ProductVariant variant);
        Task<ProductVariant> UpdateProductVariantAsync(ProductVariant variant);
        Task<bool> DeleteProductVariantAsync(int variantId);
        Task<bool> IsSkuExistsAsync(string sku);

        // Attribute value validation
        Task<bool> IsAttributeValueValidAsync(int attributeId, int valueId);
        
        // Attribute and value management
        Task<AttributeValue?> GetOrCreateAttributeValueAsync(int attributeId, string value, int categoryId);
        Task<ProductAttribute> CreateAttributeAsync(ProductAttribute attribute);
        Task<ProductAttribute?> UpdateAttributeAsync(ProductAttribute attribute);
        Task<bool> DeleteAttributeAsync(int attributeId);
        Task<AttributeValue> AddAttributeValueAsync(AttributeValue value);
        Task<AttributeValue?> UpdateAttributeValueAsync(AttributeValue value);
        Task<bool> DeleteAttributeValueAsync(int valueId);
        Task<AttributeValue?> GetAttributeValueByIdAsync(int valueId);
        Task<List<ProductAttribute>> GetAttributesByCategoryIdAsync(int categoryId);
        Task<ProductAttribute?> GetAttributeByIdAsync(int attributeId);

        // Variant methods
        Task<ProductVariant?> GetProductVariantByIdAsync(int variantId);
        
        // New products by category
        Task<List<Product>> GetTopNewProductsByCategoryAsync(string categorySlug, int count);
        
        // Sold count tracking
        Task IncrementProductSoldCountAsync(int productId, int quantity);
        Task<Dictionary<int, int>> GetProductsSoldCountAsync(List<int> productIds);
        
        // Rating statistics
        Task<Dictionary<int, (double AverageRating, int TotalReviews)>> GetProductsRatingStatsAsync(List<int> productIds);
        
        // Transaction support
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
