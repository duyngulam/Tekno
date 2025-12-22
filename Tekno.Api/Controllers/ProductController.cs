using Microsoft.AspNetCore.Mvc;
using Tekno.Api.Commons.Responses;
using Tekno.Api.Models.Catalog;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Product catalog endpoints - Browse, search, and view products
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get paginated list of products with optional filtering and sorting
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Retrieves a paginated list of products with support for:
        /// - Full-text search by keyword
        /// - Category and brand filtering
        /// - Price range filtering
        /// - Attribute/specification filtering (e.g., Color, Size, RAM)
        /// - Multiple sorting options
        /// - Smart search suggestions
        /// ## Sorting Options
        /// - `price` - Price ascending (low to high)
        /// - `-price` or `price_desc` - Price descending (high to low)
        /// - `name` - Name alphabetically (A-Z)
        /// - `-name` - Name reverse (Z-A)
        /// - `created` - Oldest first
        /// - `-created` or `newest` - Newest first
        /// - `popular` or `sold` - Most sold first
        /// - `rating` - Highest rated first
        /// 
        /// ## Filters Parameter
        /// Use query parameter format: `filters[AttributeName]=Value`
        /// 
        /// Examples:
        /// - `filters[Color]=Black` - Products with Black color
        /// - `filters[Size]=XL` - Products with XL size
        /// - `filters[RAM]=16GB` - Products with 16GB RAM
        /// - `filters[Storage]=512GB` - Products with 512GB storage
        ///Enhanced nested spec filters with multi-value support (UNION/OR logic)
        /// Example: filters[ram]=8gb,16gb will match products with RAM 8GB OR 16GB
        /// 
        /// ## Price Format
        /// Prices are in VND (Vietnamese Dong):
        /// - `1000000` = 1,000,000 VND (~$40 USD)
        /// - `25000000` = 25,000,000 VND (~$1,000 USD)
        /// 
        /// ## Response
        /// Returns paginated results with:
        /// - Product list with summary information
        /// - Total records count
        /// - Current page number
        /// - Total pages
        /// - Page size
        /// 
        /// Each product includes:
        /// - Basic info (ID, name, slug, price)
        /// - Category and brand
        /// - Images (thumbnail)
        /// - Rating and review count
        /// - Stock status
        /// - Sale price (if applicable)
        /// </remarks>
        /// <param name="request">Search and filter parameters</param>
        /// <response code="200">Returns paginated list of products</response>
        /// <response code="400">Invalid parameters (e.g., negative page number)</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductSummaryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetPaged([FromQuery] ProductSearchRequestDto request)
        {
            var result = await _productService.GetPagedProductAsync(request);
            return Ok(ApiResponse<PagedResult<ProductSummaryDto>>.Ok(result));
        }

        /// <summary>
        /// Get detailed product information by slug
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Retrieves complete product details including:
        /// - Full product information
        /// - All available variants with prices and stock
        /// - All product images
        /// - Product specifications/attributes
        /// - Category and brand information
        /// - Average rating and review count
        /// 
        /// ## Use Cases
        /// - Product detail page
        /// - Add to cart (select variant)
        /// - View specifications
        /// - Check stock availability
        /// - Compare variants
        /// 
        /// ## Response Details
        /// 
        /// **Product Information:**
        /// - ID, name, slug, description
        /// - Base price and sale price
        /// - Category and brand
        /// - Specifications (RAM, Storage, Color, etc.)
        /// 
        /// **Variants:**
        /// Each variant includes:
        /// - Variant ID (use this for cart/wishlist)
        /// - Price (may differ from base price)
        /// - Stock quantity
        /// - Attribute values (e.g., Color: Black, RAM: 16GB)
        /// - SKU code
        /// 
        /// **Images:**
        /// - Primary image (for main display)
        /// - Additional images (for gallery)
        /// - Image URLs ready to use
        /// 
        /// **Rating:**
        /// - Average rating (0-5 stars)
        /// - Total review count
        /// 
        /// ## Validation
        /// - **slug**: Required, must be valid URL slug format
        /// - **slug**: Only lowercase letters, numbers, and hyphens allowed
        /// - **slug**: Example: `product-name-123`
        /// </remarks>
        /// <param name="slug">Product URL slug (e.g., "iphone-15-pro-max")</param>
        /// <response code="200">Returns detailed product information</response>
        /// <response code="404">Product not found with given slug</response>
        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> GetDetail(string slug)
        {
            var product = await _productService.GetProductDetailAsync(slug);
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found"));

            return Ok(ApiResponse<ProductDetailDto>.Ok(product));
        }

        /// <summary>
        /// Get specific product variant details by variant ID
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Retrieves detailed information for a specific product variant.
        /// 
        /// ## What is a Variant?
        /// A variant is a specific configuration of a product with unique:
        /// - Price
        /// - Stock quantity
        /// - Attribute combination (e.g., Color + Size + RAM)
        /// - SKU code
        /// 
        /// Example:
        /// - Product: "iPhone 15 Pro Max"
        /// - Variant 1: Black, 256GB, $1,199
        /// - Variant 2: White, 512GB, $1,399
        /// - Variant 3: Black, 1TB, $1,599
        /// 
        /// ## When to Use
        /// - When user selects specific attributes on product page
        /// - Before adding to cart (to get exact price and stock)
        /// - When updating cart item
        /// - To display variant-specific information
        /// 
        /// ## Examples
        /// 
        /// **Example 1: Get iPhone variant (Black, 256GB)**
        /// ```
        /// GET /api/products/variants/42
        /// ```
        /// 
        /// **Example 2: Get laptop variant (i7, 16GB RAM, 512GB SSD)**
        /// ```
        /// GET /api/products/variants/128
        /// ```
        /// 
        /// **Example 3: Check stock before adding to cart**
        /// ```
        /// GET /api/products/variants/256
        /// 
        /// Response:
        /// {
        ///   "variantId": 256,
        ///   "price": 15500000,
        ///   "stock": 15,
        ///   "attributeValues": {
        ///     "Color": "Black",
        ///     "Storage": "512GB",
        ///     "RAM": "16GB"
        ///   }
        /// }
        /// 
        /// // If stock > 0, can add to cart
        /// ```
        /// 
        /// ## Response Details
        /// 
        /// **Variant Information:**
        /// - Variant ID (use for cart operations)
        /// - Exact price for this variant
        /// - Current stock quantity
        /// - SKU code
        /// 
        /// **Product Information:**
        /// - Product ID, name, slug
        /// - Category and brand
        /// - Product description
        /// - Product images
        /// 
        /// **Attribute Values:**
        /// Dictionary of attribute name → value
        /// - Example: { "Color": "Black", "RAM": "16GB", "Storage": "512GB" }
        /// 
        /// ## Use in Cart
        /// ```javascript
        /// // 1. Get variant details
        /// const variant = await fetch('/api/products/variants/42').then(r => r.json());
        /// 
        /// // 2. Check stock
        /// if (variant.data.stock >= quantity) {
        ///   // 3. Add to cart with variantId
        ///   await fetch('/api/cart/items', {
        ///     method: 'POST',
        ///     body: JSON.stringify({
        ///       variantId: 42,
        ///       quantity: 2
        ///     })
        ///   });
        /// }
        /// ```
        /// 
        /// ## Validation
        /// - **variantId**: Required, must be positive integer
        /// - **variantId**: Valid range: 1 to 2147483647
        /// - **variantId**: Must exist in database
        /// </remarks>
        /// <param name="variantId">Product variant ID (positive integer)</param>
        /// <response code="200">Returns variant details with product information</response>
        /// <response code="404">Variant not found with given ID</response>
        /// <response code="400">Invalid variant ID format</response>
        [HttpGet("variants/{variantId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetVariantById(int variantId)
        {
            var variant = await _productService.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return NotFound(ApiResponse<ProductVariantDetailDto>.Fail("Product variant not found"));

            return Ok(ApiResponse<ProductVariantDetailDto>.Ok(variant));
        }

        /// <summary>
        /// Get newest products in a specific category
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Retrieves the most recently added products in a category.
        /// Perfect for "New Arrivals" sections on your website.
        /// 
        /// ## Use Cases
        /// - Homepage "New Arrivals" section
        /// - Category page "Latest Products"
        /// - "Recently Added" widget
        /// - Email newsletter content
        /// 
        /// ## Examples
        /// 
        /// **Example 1: Get 10 newest laptops**
        /// ```
        /// GET /api/products/new/laptops?count=10
        /// ```
        /// 
        /// **Example 2: Get 5 newest smartphones**
        /// ```
        /// GET /api/products/new/smartphones?count=5
        /// ```
        /// 
        /// **Example 3: Get 20 newest accessories (max recommended)**
        /// ```
        /// GET /api/products/new/accessories?count=20
        /// ```
        /// 
        /// **Example 4: Default count (10 products)**
        /// ```
        /// GET /api/products/new/gaming-laptops
        /// ```
        /// 
        /// ## Frontend Integration
        /// 
        /// **React Example:**
        /// ```jsx
        /// function NewArrivals({ categorySlug }) {
        ///   const [products, setProducts] = useState([]);
        ///   
        ///   useEffect(() => {
        ///     fetch(`/api/products/new/${categorySlug}?count=8`)
        ///       .then(res => res.json())
        ///       .then(data => setProducts(data.data));
        ///   }, [categorySlug]);
        ///   
        ///   return (
        ///     &lt;section className="new-arrivals"&gt;
        ///       &lt;h2&gt;New Arrivals&lt;/h2&gt;
        ///       &lt;div className="product-grid"&gt;
        ///         {products.map(product => (
        ///           &lt;ProductCard key={product.id} product={product} /&gt;
        ///         ))}
        ///       &lt;/div&gt;
        ///     &lt;/section&gt;
        ///   );
        /// }
        /// ```
        /// 
        /// **Vue Example:**
        /// ```vue
        /// &lt;template&gt;
        ///   &lt;div class="new-products"&gt;
        ///     &lt;h3&gt;Latest in {{ categoryName }}&lt;/h3&gt;
        ///     &lt;product-card 
        ///       v-for="product in products" 
        ///       :key="product.id"
        ///       :product="product"
        ///     /&gt;
        ///   &lt;/div&gt;
        /// &lt;/template&gt;
        /// 
        /// &lt;script&gt;
        /// export default {
        ///   data() {
        ///     return { products: [] }
        ///   },
        ///   async mounted() {
        ///     const response = await fetch(`/api/products/new/laptops?count=6`);
        ///     const result = await response.json();
        ///     this.products = result.data;
        ///   }
        /// }
        /// &lt;/script&gt;
        /// ```
        /// 
        /// ## Category Slugs
        /// Common category slugs:
        /// - `laptops` - Laptop computers
        /// - `smartphones` - Mobile phones
        /// - `tablets` - Tablet devices
        /// - `gaming-laptops` - Gaming laptops
        /// - `accessories` - Phone/laptop accessories
        /// - `headphones` - Audio devices
        /// - `smartwatches` - Wearable devices
        /// 
        /// ## Sorting
        /// Products are sorted by creation date (newest first).
        /// 
        /// ## Performance
        /// - Cached for better performance
        /// - Cache invalidated when new products are added
        /// - Fast response time (&lt;100ms typical)
        /// 
        /// ## Response
        /// Returns a list of product summaries including:
        /// - Product ID, name, slug
        /// - Price and sale price
        /// - Primary image
        /// - Rating and reviews
        /// - Stock status
        /// - Category and brand
        /// 
        /// ## Validation
        /// - **categorySlug**: Required, must be valid category slug
        /// - **categorySlug**: Only lowercase, numbers, hyphens allowed
        /// - **categorySlug**: Example: `gaming-laptops`
        /// - **count**: Optional, default is 10
        /// - **count**: Valid range: 1 to 100
        /// - **count**: Maximum of 100 products to prevent performance issues
        /// 
        /// ## Error Responses
        /// 
        /// **Category not found:**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Category not found",
        ///   "data": []
        /// }
        /// ```
        /// 
        /// **Invalid count:**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Count must be between 1 and 100"
        /// }
        /// ```
        /// </remarks>
        /// <param name="categorySlug">Category URL slug (e.g., "laptops", "smartphones")</param>
        /// <param name="count">Number of products to return (1-100, default: 10)</param>
        /// <response code="200">Returns list of newest products in category</response>
        /// <response code="404">Category not found with given slug</response>
        /// <response code="400">Invalid count value (must be 1-100)</response>
        [HttpGet("new/{categorySlug}")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductSummaryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetTopNewByCategory(
            string categorySlug,
            [FromQuery] int count = 10)
        {
            var products = await _productService.GetTopNewProductsByCategoryAsync(categorySlug, count);
            return Ok(ApiResponse<System.Collections.Generic.List<ProductSummaryDto>>.Ok(
                products, 
                $"Retrieved {products.Count} newest products"));
        }

        /// <summary>
        /// Get products on sale (hot sale)
        /// </summary>
        /// <param name="categorySlug">Optional category filter (e.g., "laptops")</param>
        /// <param name="count">Number of products to return (1-100, default: 20)</param>
        /// <response code="200">Returns list of products on sale</response>
        /// <response code="400">Invalid parameters</response>
        [HttpGet("on-sale")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductSummaryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetProductsOnSale(
            [FromQuery] string? categorySlug = null,
            [FromQuery] int count = 20)
        {
            if (count < 1 || count > 100)
            {
                return BadRequest(ApiResponse<string>.Fail("Count must be between 1 and 100"));
            }

            var products = await _productService.GetProductsOnSaleAsync(categorySlug, count);
            return Ok(ApiResponse<List<ProductSummaryDto>>.Ok(
                products,
                $"Retrieved {products.Count} products on sale"));
        }
    }
}
