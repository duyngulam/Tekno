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
