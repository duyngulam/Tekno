# Top N New Products by Category - API Documentation

## ?? Overview

Public API endpoint to get the newest products in a specific category. Perfect for displaying "New Arrivals" sections on your e-commerce website.

---

## ?? API Endpoint

### Get Top N Newest Products by Category

```http
GET /api/products/new/{categorySlug}?count={count}
```

**Parameters:**
- `categorySlug` (path, required): Category slug (e.g., "laptops", "smartphones")
- `count` (query, optional): Number of products to return
  - Default: 10
  - Maximum: 100
  - Minimum: 1

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 15,
      "name": "Dell XPS 13 (2025)",
      "slug": "dell-xps-13-2025",
      "brandName": "Dell",
      "categoryName": "Laptops",
      "basePrice": 1299.99,
      "discountPercent": 10,
      "finalPrice": 1169.99,
      "overview": "Latest ultrabook with 13th Gen Intel processor",
      "primaryImagePath": "https://res.cloudinary.com/.../dell-xps-13.jpg",
      "createdAt": "2025-01-20T10:30:00Z"
    },
    {
      "id": 14,
      "name": "HP Spectre x360",
      "slug": "hp-spectre-x360",
      "brandName": "HP",
      "categoryName": "Laptops",
      "basePrice": 1499.99,
      "discountPercent": null,
      "finalPrice": 1499.99,
      "overview": "2-in-1 convertible laptop",
      "primaryImagePath": "https://res.cloudinary.com/.../hp-spectre.jpg",
      "createdAt": "2025-01-19T14:20:00Z"
    }
    // ... more products (sorted by newest first)
  ],
  "message": "Retrieved 10 newest products"
}
```

---

## ?? Usage Examples

### Example 1: Get 5 Newest Laptops

```bash
GET /api/products/new/laptops?count=5
```

**Response:**
```json
{
  "success": true,
  "data": [
    // 5 newest laptop products
  ],
  "message": "Retrieved 5 newest products"
}
```

### Example 2: Get 10 Newest Smartphones (Default)

```bash
GET /api/products/new/smartphones
```

**Response:**
```json
{
  "success": true,
  "data": [
    // 10 newest smartphone products (default count)
  ],
  "message": "Retrieved 10 newest products"
}
```

### Example 3: Get 20 Newest TVs

```bash
GET /api/products/new/televisions?count=20
```

### Example 4: All New Products (No Category Filter)

```bash
GET /api/products/new/all?count=15
```

---

## ?? Sorting Logic

Products are sorted by:
1. **Creation date** (newest first)
2. Only shows **available** products (status = "available")

Example order:
```
Product A - Created: 2025-01-20 ? Shown first
Product B - Created: 2025-01-19 ? Shown second
Product C - Created: 2025-01-18 ? Shown third
...
```

---

## ?? Frontend Integration

### React Component

```jsx
import { useState, useEffect } from 'react';

const NewArrivals = ({ categorySlug = 'laptops', count = 5 }) => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchNewProducts = async () => {
      try {
        const response = await fetch(
          `/api/products/new/${categorySlug}?count=${count}`
        );
        const result = await response.json();
        setProducts(result.data);
      } catch (error) {
        console.error('Error loading new products:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchNewProducts();
  }, [categorySlug, count]);

  if (loading) return <div>Loading...</div>;

  return (
    <div className="new-arrivals">
      <h2>New Arrivals in {categorySlug}</h2>
      <div className="product-grid">
        {products.map(product => (
          <div key={product.id} className="product-card">
            <img src={product.primaryImagePath} alt={product.name} />
            <h3>{product.name}</h3>
            <p className="brand">{product.brandName}</p>
            <div className="price">
              {product.discountPercent && (
                <span className="original">${product.basePrice}</span>
              )}
              <span className="final">${product.finalPrice}</span>
              {product.discountPercent && (
                <span className="discount">-{product.discountPercent}%</span>
              )}
            </div>
            <span className="badge">NEW</span>
          </div>
        ))}
      </div>
    </div>
  );
};

// Usage
<NewArrivals categorySlug="laptops" count={5} />
<NewArrivals categorySlug="smartphones" count={8} />
```

### Vanilla JavaScript

```javascript
// Fetch new products
async function loadNewProducts(categorySlug, count = 10) {
  try {
    const response = await fetch(
      `/api/products/new/${categorySlug}?count=${count}`
    );
    const result = await response.json();
    
    if (result.success) {
      displayProducts(result.data);
    }
  } catch (error) {
    console.error('Error:', error);
  }
}

// Display products
function displayProducts(products) {
  const container = document.getElementById('new-products');
  
  products.forEach(product => {
    const productCard = `
      <div class="product-card">
        <span class="new-badge">NEW</span>
        <img src="${product.primaryImagePath}" alt="${product.name}">
        <h3>${product.name}</h3>
        <p class="brand">${product.brandName}</p>
        <div class="price">
          ${product.discountPercent ? 
            `<span class="original">$${product.basePrice}</span>` : ''}
          <span class="final">$${product.finalPrice}</span>
        </div>
        <a href="/products/${product.slug}" class="view-btn">View Details</a>
      </div>
    `;
    container.innerHTML += productCard;
  });
}

// Load on page load
loadNewProducts('laptops', 5);
```

### TypeScript (Angular/React)

```typescript
interface Product {
  id: number;
  name: string;
  slug: string;
  brandName: string;
  categoryName: string;
  basePrice: number;
  discountPercent: number | null;
  finalPrice: number;
  overview: string;
  primaryImagePath: string;
  createdAt: string;
}

async function getNewProducts(
  categorySlug: string, 
  count: number = 10
): Promise<Product[]> {
  const response = await fetch(
    `/api/products/new/${categorySlug}?count=${count}`
  );
  const result = await response.json();
  return result.data;
}

// Usage
const laptops = await getNewProducts('laptops', 5);
const smartphones = await getNewProducts('smartphones', 10);
```

---

## ?? UI Implementation Ideas

### Homepage "New Arrivals" Section

```html
<section class="new-arrivals">
  <h2>New Arrivals - Laptops</h2>
  <div id="new-laptops" class="product-grid">
    <!-- Products loaded via JavaScript -->
  </div>
  <a href="/category/laptops" class="see-all">See All Laptops ?</a>
</section>

<script>
  loadNewProducts('laptops', 4);
</script>
```

### Category Page "Latest Products"

```html
<div class="category-page">
  <h1>Smartphones</h1>
  
  <section class="latest-products">
    <h3>Latest Arrivals</h3>
    <div id="latest-smartphones">
      <!-- Load 8 newest smartphones -->
    </div>
  </section>
  
  <section class="all-products">
    <h3>All Smartphones</h3>
    <!-- Regular paginated list -->
  </section>
</div>

<script>
  loadNewProducts('smartphones', 8);
</script>
```

### Multiple Categories on Homepage

```javascript
// Load new products for multiple categories
const categories = ['laptops', 'smartphones', 'headphones'];

categories.forEach(category => {
  loadNewProducts(category, 4)
    .then(products => {
      displayInSection(`new-${category}`, products);
    });
});
```

---

## ?? Use Cases

### Use Case 1: Homepage Featured Section
```
Display 4-6 newest products across all categories
to show customers what's new in the store
```

### Use Case 2: Category Landing Page
```
Show 8-10 newest products in that category
at the top of the category page
```

### Use Case 3: "New This Week" Banner
```
Highlight newest products added in the last 7 days
with special badges or banners
```

### Use Case 4: Email Newsletter
```
Fetch newest products via API to include
in weekly/monthly newsletter
```

---

## ?? Backend Logic

### Repository Method

```csharp
public async Task<List<Product>> GetTopNewProductsByCategoryAsync(
    string categorySlug, 
    int count)
{
    var query = _context.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.Images)
        .AsNoTracking()
        .Where(p => p.Status == "available");

    // Filter by category
    if (!string.IsNullOrWhiteSpace(categorySlug))
    {
        query = query.Where(p => p.Category.Slug == categorySlug);
    }

    // Get top N newest
    return await query
        .OrderByDescending(p => p.CreatedAt)
        .Take(count)
        .ToListAsync();
}
```

### Service Method

```csharp
public async Task<List<ProductSummaryDto>> GetTopNewProductsByCategoryAsync(
    string categorySlug, 
    int count = 10)
{
    // Validate count (1-100)
    if (count <= 0 || count > 100)
    {
        count = 10;
    }

    var products = await _productRepository
        .GetTopNewProductsByCategoryAsync(categorySlug, count);
    
    _logger.LogInformation(
        "Retrieved {Count} newest products for category {CategorySlug}", 
        products.Count, 
        categorySlug);

    return _mapper.Map<List<ProductSummaryDto>>(products);
}
```

---

## ?? Query Parameters

| Parameter | Type | Required | Default | Max | Description |
|-----------|------|----------|---------|-----|-------------|
| `count` | int | No | 10 | 100 | Number of products to return |

---

## ? Response Fields

Each product in the response includes:

| Field | Type | Description |
|-------|------|-------------|
| `id` | int | Product ID |
| `name` | string | Product name |
| `slug` | string | URL-friendly slug |
| `brandName` | string | Brand name |
| `categoryName` | string | Category name |
| `basePrice` | decimal | Original price |
| `discountPercent` | decimal? | Discount percentage (null if no discount) |
| `finalPrice` | decimal | Final price after discount |
| `overview` | string | Short description |
| `primaryImagePath` | string | Main product image URL |
| `createdAt` | datetime | Product creation date |

---

## ?? Security & Performance

### Security
- ? No authentication required (public endpoint)
- ? Only returns available products (status = "available")
- ? Count limited to maximum 100 to prevent abuse

### Performance
- ? Uses `AsNoTracking()` for read-only queries
- ? Indexed on `CreatedAt` for fast sorting
- ? Includes only necessary data (Brand, Category, Images)
- ? Limited result set (max 100)

### Caching (Optional)
```csharp
// Can be cached with short TTL (5-10 minutes)
var cacheKey = $"new_products_{categorySlug}_{count}";
var cachedProducts = await _cache.GetAsync<List<ProductSummaryDto>>(cacheKey);

if (cachedProducts != null)
{
    return cachedProducts;
}

// Fetch from database
var products = await _productRepository.GetTopNewProductsByCategoryAsync(...);

// Cache for 5 minutes
await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
```

---

## ?? Testing

### Manual Testing

```bash
# Test 1: Get 5 newest laptops
curl http://localhost:5000/api/products/new/laptops?count=5

# Test 2: Get 10 newest smartphones (default count)
curl http://localhost:5000/api/products/new/smartphones

# Test 3: Get 20 newest products across all categories
curl http://localhost:5000/api/products/new/all?count=20

# Test 4: Invalid count (should default to 10)
curl http://localhost:5000/api/products/new/laptops?count=500

# Test 5: Invalid category (should return empty array)
curl http://localhost:5000/api/products/new/invalid-category
```

### Unit Test Example

```csharp
[Fact]
public async Task GetTopNewProductsByCategory_ShouldReturnNewestProducts()
{
    // Arrange
    var categorySlug = "laptops";
    var count = 5;

    // Act
    var result = await _productService.GetTopNewProductsByCategoryAsync(
        categorySlug, 
        count);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(5, result.Count);
    Assert.True(result[0].CreatedAt >= result[1].CreatedAt); // Newest first
}
```

---

## ?? Example Scenarios

### Scenario 1: E-commerce Homepage

```html
<!-- New Arrivals Section -->
<section class="new-arrivals-section">
  <div class="container">
    <h2>New Arrivals</h2>
    
    <!-- Laptops -->
    <div class="category-section">
      <h3>Latest Laptops</h3>
      <div id="new-laptops" class="product-grid">
        <!-- 4 newest laptops -->
      </div>
    </div>
    
    <!-- Smartphones -->
    <div class="category-section">
      <h3>Latest Smartphones</h3>
      <div id="new-smartphones" class="product-grid">
        <!-- 4 newest smartphones -->
      </div>
    </div>
  </div>
</section>

<script>
  loadNewProducts('laptops', 4);
  loadNewProducts('smartphones', 4);
</script>
```

### Scenario 2: Category Page with "New" Tab

```jsx
const CategoryPage = ({ categorySlug }) => {
  const [activeTab, setActiveTab] = useState('new');
  const [products, setProducts] = useState([]);

  useEffect(() => {
    if (activeTab === 'new') {
      // Load newest products
      fetch(`/api/products/new/${categorySlug}?count=12`)
        .then(res => res.json())
        .then(data => setProducts(data.data));
    } else {
      // Load all products (paginated)
      fetch(`/api/products?category=${categorySlug}&page=1`)
        .then(res => res.json())
        .then(data => setProducts(data.data));
    }
  }, [activeTab, categorySlug]);

  return (
    <div>
      <div className="tabs">
        <button onClick={() => setActiveTab('new')}>New Arrivals</button>
        <button onClick={() => setActiveTab('all')}>All Products</button>
      </div>
      <ProductGrid products={products} />
    </div>
  );
};
```

---

## ?? Summary

**Endpoint:** `GET /api/products/new/{categorySlug}?count={count}`

**Purpose:** Get the newest products in a category

**Key Features:**
- ? Sorted by creation date (newest first)
- ? Only shows available products
- ? Supports count parameter (1-100)
- ? Category filtering via slug
- ? Includes product images, pricing, discounts
- ? Public endpoint (no auth required)

**Perfect for:**
- Homepage "New Arrivals" sections
- Category page featured products
- "What's New" banners
- Email newsletters
- Mobile app "New" tab

---

**Status:** ? Ready to use  
**Build:** ? Success  
**Performance:** ?? Optimized with AsNoTracking
