using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Nest;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Catalog
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> GetPagedProductAsync(
            string? categorySlug,
            string? brandSlug,
            string? search,
            string? sort,
            string? minPrice,
            string? maxPrice,
            PagingParams paging)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categorySlug))
                query = query.Where(p => p.Category.Slug == categorySlug);

            if (!string.IsNullOrWhiteSpace(brandSlug))
                query = query.Where(p => p.Brand.Slug == brandSlug);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => EF.Functions.ILike(p.Name, $"%{search}%"));

            if (decimal.TryParse(minPrice, out var min))
                query = query.Where(p => p.BasePrice >= min);

            if (decimal.TryParse(maxPrice, out var max))
                query = query.Where(p => p.BasePrice <= max);

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.BasePrice),
                "price_desc" => query.OrderByDescending(p => p.BasePrice),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<Product>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<bool> IsProductExistBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;
            
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Slug == slug);
        }

        public async Task<bool> IsProductExistByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> AddProductAsync(Product newProduct)
        {
            if (newProduct == null)
                throw new ArgumentNullException(nameof(newProduct));

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null)
                throw new InvalidOperationException($"Product with ID {product.Id} not found");

            _context.Entry(existing).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task IncrementProductSoldCountAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IncrementSoldCount(quantity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Dictionary<int, int>> GetProductsSoldCountAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.TotalSold })
                .ToDictionaryAsync(p => p.Id, p => p.TotalSold);
        }

        public async Task<Dictionary<int, (double AverageRating, int TotalReviews)>> GetProductsRatingStatsAsync(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return new Dictionary<int, (double, int)>();

            // Query approved reviews grouped by product
            var stats = await _context.Set<Tekno.Domain.Review.ProductReview>()
                .AsNoTracking()
                .Where(r => productIds.Contains(r.ProductId) && r.Status == Tekno.Domain.Review.ReviewStatus.Approved)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    TotalReviews = g.Count()
                })
                .ToListAsync();

            return stats.ToDictionary(
                x => x.ProductId,
                x => (AverageRating: x.AverageRating, TotalReviews: x.TotalReviews));
        }

        public async Task<List<Product>> GetProductsWithDiscountAsync(string? categorySlug, int count)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .AsNoTracking()
                .Where(p => p.DiscountPercent.HasValue && p.DiscountPercent.Value > 0);

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                query = query.Where(p => p.Category.Slug == categorySlug);
            }

            // Order by discount percentage (highest first), then by creation date (newest first)
            return await query
                .OrderByDescending(p => p.DiscountPercent)
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<PagedResult<Product>> GetAdminProductsPagedAsync(
    string? search,
    string? categorySlug,
    string? brandSlug,
    string? status,
    PagingParams paging)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.OrderBy(i => i.SortOrder))
                .Include(p => p.Variants)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Slug.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)));
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                query = query.Where(p => p.Category.Slug == categorySlug);
            }

            // Brand filter
            if (!string.IsNullOrWhiteSpace(brandSlug))
            {
                query = query.Where(p => p.Brand.Slug == brandSlug);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            // Order byNewest first
            query = query.OrderByDescending(p => p.CreatedAt);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Product>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<ProductImage?> GetProductImageByIdAsync(int imageId)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == imageId);
        }

        public async Task<ProductImage> AddProductImageAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<bool> UpdateProductImageAsync(ProductImage image)
        {
            _context.ProductImages.Update(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var image = await GetProductImageByIdAsync(imageId);
            if (image == null) return false;

            _context.ProductImages.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.SortOrder)
                .ToListAsync();
        }

        public async Task<ProductVariant> AddProductVariantAsync(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
            return variant;
        }

        public async Task<ProductVariant> UpdateProductVariantAsync(ProductVariant variant)
        {
            var existing = await _context.ProductVariants
                .Include(v => v.VariantAttributes)
                .FirstOrDefaultAsync(v => v.Id == variant.Id);

            if (existing == null)
                throw new InvalidOperationException($"Variant with ID {variant.Id} not found");

            // Update scalar fields
            existing.UpdateSku(variant.Sku);
            existing.UpdatePrice(variant.Price);
            existing.UpdateStock(variant.Stock);
            existing.UpdateStatus(variant.Status);

            // Replace variant attributes
            _context.Set<ProductVariantAttribute>().RemoveRange(existing.VariantAttributes);
            existing.VariantAttributes.Clear();

            foreach (var attr in variant.VariantAttributes)
            {
                existing.VariantAttributes.Add(new ProductVariantAttribute
                {
                    VariantId = existing.Id,
                    AttributeId = attr.AttributeId,
                    ValueId = attr.ValueId
                });
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            var variant = await GetProductVariantByIdAsync(variantId);
            if (variant == null) return false;

            _context.ProductVariants.Remove(variant);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsSkuExistsAsync(string sku)
        {
            return await _context.ProductVariants
                .AnyAsync(v => v.Sku == sku);
        }

        public async Task<bool> IsAttributeValueValidAsync(int attributeId, int valueId)
        {
            return await _context.AttributeValues
                .AnyAsync(v => v.Id == valueId && v.AttributeId == attributeId);
        }

        public async Task<AttributeValue?> GetOrCreateAttributeValueAsync(int attributeId, string value, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty", nameof(value));

            // Trim and normalize the value
            value = value.Trim();

            // First, check if the value already exists for this attribute
            var existingValue = await _context.AttributeValues
                .FirstOrDefaultAsync(v => v.AttributeId == attributeId && v.Value == value);

            if (existingValue != null)
                return existingValue;

            // Validate that the attribute exists and belongs to the category
            var attribute = await _context.Attributes
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == attributeId);

            if (attribute == null)
                throw new InvalidOperationException($"Attribute with ID {attributeId} not found");

            // Check if attribute belongs to category or is global
            if (!attribute.IsGlobal && attribute.CategoryId != categoryId)
                throw new InvalidOperationException($"Attribute {attribute.Name} does not belong to this product's category");

            // Create new attribute value
            var newValue = new AttributeValue(attributeId, value);
            _context.AttributeValues.Add(newValue);
            await _context.SaveChangesAsync();

            return newValue;
        }

        public async Task<List<ProductAttribute>> GetAttributesByCategoryIdAsync(int categoryId)
        {
            return await _context.Attributes
                .Include(a => a.Values)
                .Where(a => a.CategoryId == categoryId || a.IsGlobal)
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductAttribute?> GetAttributeByIdAsync(int attributeId)
        {
            return await _context.Attributes
                .Include(a => a.Values)
                .Include(a => a.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == attributeId);
        }

        public async Task<AttributeValue?> GetAttributeValueAsync(int attributeId, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            return await _context.AttributeValues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.AttributeId == attributeId && v.Value == value);
        }

        public async Task<ProductAttribute> CreateAttributeAsync(ProductAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            _context.Attributes.Add(attribute);
            await _context.SaveChangesAsync();
            return attribute;
        }

        public async Task<ProductAttribute?> UpdateAttributeAsync(ProductAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var existing = await _context.Attributes.FindAsync(attribute.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(attribute);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAttributeAsync(int attributeId)
        {
            var attribute = await _context.Attributes
                .Include(a => a.Values)
                .FirstOrDefaultAsync(a => a.Id == attributeId);

            if (attribute == null)
                return false;

            // Check if any variants are using this attribute
            var isUsed = await _context.Set<ProductVariantAttribute>()
                .AnyAsync(va => va.AttributeId == attributeId);

            if (isUsed)
                throw new InvalidOperationException("Cannot delete attribute that is in use by product variants");

            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AttributeValue> AddAttributeValueAsync(AttributeValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Check for duplicate
            var exists = await _context.AttributeValues
                .AnyAsync(v => v.AttributeId == value.AttributeId && v.Value == value.Value);

            if (exists)
                throw new InvalidOperationException($"Value '{value.Value}' already exists for this attribute");

            _context.AttributeValues.Add(value);
            await _context.SaveChangesAsync();
            return value;
        }

        public async Task<AttributeValue?> UpdateAttributeValueAsync(AttributeValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var existing = await _context.AttributeValues.FindAsync(value.Id);
            if (existing == null)
                return null;

            // Check for duplicate
            var duplicate = await _context.AttributeValues
                .AnyAsync(v => v.Id != value.Id && 
                              v.AttributeId == value.AttributeId && 
                              v.Value == value.Value);

            if (duplicate)
                throw new InvalidOperationException($"Value '{value.Value}' already exists for this attribute");

            _context.Entry(existing).CurrentValues.SetValues(value);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAttributeValueAsync(int valueId)
        {
            var value = await _context.AttributeValues.FindAsync(valueId);
            if (value == null)
                return false;

            // Check if any variants are using this value
            var isUsed = await _context.Set<ProductVariantAttribute>()
                .AnyAsync(va => va.ValueId == valueId);

            if (isUsed)
                throw new InvalidOperationException("Cannot delete attribute value that is in use by product variants");

            _context.AttributeValues.Remove(value);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AttributeValue?> GetAttributeValueByIdAsync(int valueId)
        {
            return await _context.AttributeValues
                .Include(v => v.Attribute)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == valueId);
        }

        public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .AsNoTracking()
                .Include(v => v.Product)
                    .ThenInclude(p => p.Brand)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Category)
                .Include(v => v.VariantAttributes)
                    .ThenInclude(va => va.Attribute)
                .Include(v => v.VariantAttributes)
                    .ThenInclude(va => va.Value)
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        public async Task<List<Product>> GetTopNewProductsByCategoryAsync(string categorySlug, int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .AsNoTracking()
                .Where(p => p.Category.Slug == categorySlug && p.Status == "available")
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task RebuildProductSpecsAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return;

            // Build specs JSON from variants using domain method
            product.BuildSpecsFromVariants();

            await _context.SaveChangesAsync();
        }

        public async Task RebuildAllProductSpecsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .ToListAsync();

            if (products == null || products.Count == 0) return;

            foreach (var product in products)
            {
                product.BuildSpecsFromVariants();
            }

            await _context.SaveChangesAsync();
        }
    }
}

