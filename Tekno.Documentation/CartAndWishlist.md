# Cart & Wishlist System Documentation

## Overview
Complete shopping cart and wishlist system for Tekno e-commerce platform that works with **ProductVariants** (not just products).

## Why ProductVariants?
Users don't buy "iPhone 15" - they buy "iPhone 15 Pro - 256GB - Black". Each variant has:
- Unique SKU
- Specific price
- Individual stock level
- Distinct attributes (Color, RAM, Storage, etc.)

## Features
? Cart management (add, update, remove, clear)  
? Wishlist management  
? Stock validation  
? Price locking (price at time of adding)  
? User-specific carts  
? Variant details included in responses  
? Authentication required  

---

## API Endpoints

### ?? **Cart Endpoints** (`/api/cart`)

#### 1. Get Current User's Cart
```http
GET /api/cart
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "userId": 123,
    "subtotal": 2598.00,
    "totalItems": 3,
    "createdAt": "2025-01-15T10:00:00Z",
    "updatedAt": "2025-01-15T14:30:00Z",
    "items": [
      {
        "id": 1,
        "cartId": 1,
        "variantId": 1,
        "quantity": 1,
        "price": 1099.00,
        "totalPrice": 1099.00,
        "addedAt": "2025-01-15T10:00:00Z",
        "productName": "Dell XPS 13",
        "productSlug": "dell-xps-13",
        "sku": "XPS13-I5-8-512",
        "brandName": "Dell",
        "categoryName": "Laptop",
        "primaryImage": "https://cdn.example.com/xps13.jpg",
        "availableStock": 20,
        "attributes": [
          {
            "attributeName": "Color",
            "attributeValue": "Silver"
          },
          {
            "attributeName": "RAM",
            "attributeValue": "8GB"
          },
          {
            "attributeName": "Storage",
            "attributeValue": "512GB"
          }
        ]
      }
    ]
  }
}
```

#### 2. Add Item to Cart
```http
POST /api/cart/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "variantId": 1,
  "quantity": 2
}
```

**Validation:**
- ? Variant must exist
- ? Stock must be available
- ? Quantity > 0

**Behavior:**
- If variant already in cart ? increases quantity
- If new variant ? adds new cart item

#### 3. Update Cart Item Quantity
```http
PUT /api/cart/items/{variantId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 3
}
```

#### 4. Remove Item from Cart
```http
DELETE /api/cart/items/{variantId}
Authorization: Bearer {token}
```

#### 5. Clear Cart
```http
DELETE /api/cart
Authorization: Bearer {token}
```

---

### ?? **Wishlist Endpoints** (`/api/wishlist`)

#### 1. Get Current User's Wishlist
```http
GET /api/wishlist
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "userId": 123,
      "variantId": 5,
      "addedAt": "2025-01-10T10:00:00Z",
      "productName": "MacBook Air M2",
      "productSlug": "macbook-air-m2",
      "sku": "MBA-M2-8-256",
      "brandName": "Apple",
      "categoryName": "Laptop",
      "price": 999.00,
      "stock": 15,
      "primaryImage": "https://cdn.example.com/mba.jpg",
      "attributes": [
        {
          "attributeName": "Color",
          "attributeValue": "Silver"
        },
        {
          "attributeName": "RAM",
          "attributeValue": "8GB"
        }
      ]
    }
  ]
}
```

#### 2. Add Item to Wishlist
```http
POST /api/wishlist/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "variantId": 5
}
```

**Validation:**
- ? Variant must exist
- ? Cannot add duplicate (returns 409 Conflict)

#### 3. Remove Item from Wishlist
```http
DELETE /api/wishlist/items/{variantId}
Authorization: Bearer {token}
```

#### 4. Check if Item is in Wishlist
```http
GET /api/wishlist/check/{variantId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": true
}
```

---

## Database Schema

### user_carts Table
```sql
Column        Type            Description
----------    ------------    -------------------------
id            INT             Primary key
user_id       INT             User ID (unique per user)
created_at    TIMESTAMPTZ     When cart was created
updated_at    TIMESTAMPTZ     Last modification time
```

### cart_items Table
```sql
Column        Type            Description
----------    ------------    -------------------------
id            INT             Primary key
cart_id       INT             Foreign key to user_carts
variant_id    INT             Foreign key to product_variant
quantity      INT             Number of items
price         DECIMAL(12,2)   Price at time of adding
added_at      TIMESTAMPTZ     When item was added

Unique Index: (cart_id, variant_id)  -- One variant per cart
```

### wishlists Table
```sql
Column        Type            Description
----------    ------------    -------------------------
id            INT             Primary key
user_id       INT             User ID
variant_id    INT             Foreign key to product_variant
added_at      TIMESTAMPTZ     When item was added

Unique Index: (user_id, variant_id)  -- One variant per user wishlist
```

---

## Business Logic

### Cart Behavior

#### Adding Items
1. Check if variant exists and has stock
2. Get user's cart (create if doesn't exist)
3. If variant already in cart:
   - Add quantities together
   - Keep original price
4. If new variant:
   - Add new cart item
   - Lock current price

#### Stock Validation
- Stock check happens on:
  - Add to cart
  - Update quantity
- Real-time validation prevents overselling

#### Price Locking
- Price saved at time of adding to cart
- Even if product price changes, cart price remains
- Useful for:
  - Abandoned cart recovery
  - Price protection during checkout

### Wishlist Behavior

#### Adding Items
1. Check if variant exists
2. Check if already in wishlist (prevent duplicates)
3. Add to wishlist

#### Use Cases
- Save for later
- Price tracking
- Gift ideas
- Comparison shopping

---

## Frontend Integration

### Cart Workflow

```javascript
// 1. User selects variant on product page
const selectedVariant = {
  id: 1,
  sku: "XPS13-I5-8-512",
  price: 1099.00,
  stock: 20
};

// 2. Add to cart
const response = await fetch('/api/cart/items', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    variantId: selectedVariant.id,
    quantity: 1
  })
});

// 3. Display cart
const cart = await fetch('/api/cart', {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

console.log(`Cart has ${cart.data.totalItems} items`);
console.log(`Subtotal: $${cart.data.subtotal}`);
```

### Wishlist Workflow

```javascript
// 1. Check if in wishlist (for heart icon)
const isInWishlist = await fetch(`/api/wishlist/check/${variantId}`, {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

// 2. Toggle wishlist
if (isInWishlist.data) {
  // Remove from wishlist
  await fetch(`/api/wishlist/items/${variantId}`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` }
  });
} else {
  // Add to wishlist
  await fetch('/api/wishlist/items', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ variantId })
  });
}
```

---

## Search Results & Variants

### Current Search Response
**Product search returns `ProductSummaryDto`** (product-level data):
```json
{
  "id": 1,
  "name": "Dell XPS 13",
  "slug": "dell-xps-13",
  "basePrice": 1099.00,
  "primaryImagePath": "...",
  "brandName": "Dell",
  "categoryName": "Laptop"
}
```

**Variants are NOT included in search results** because:
1. **Performance**: Including all variants would make responses huge
2. **UI/UX**: Product cards show base product info
3. **Workflow**: User clicks product ? sees variants on detail page

### Product Detail Page Flow
```
1. Search Results (GET /api/products)
   ??> Shows: Product cards with base info
   
2. User clicks product
   ??> Navigates to: /products/{slug}
   
3. Product Detail (GET /api/products/{slug})
   ??> Returns: ProductDetailDto with ALL variants
   
4. User selects variant
   ??> Selects: Color, RAM, Storage, etc.
   
5. Add to Cart/Wishlist
   ??> Uses: Specific variantId
```

### Getting Variants

**Option 1: Product Detail Endpoint**
```http
GET /api/products/dell-xps-13
```
Returns `ProductDetailDto` with `variants` array.

**Option 2: Direct Variant Endpoint**
```http
GET /api/products/variants/1
```
Returns specific variant details.

---

## Error Handling

### Common Errors

#### 401 Unauthorized
```json
{
  "success": false,
  "message": "User not authenticated",
  "statusCode": 401
}
```
**Cause**: Missing or invalid JWT token

#### 404 Not Found
```json
{
  "success": false,
  "message": "ProductVariant with key '999' was not found",
  "statusCode": 404,
  "errorCode": "NOT_FOUND"
}
```
**Cause**: Variant doesn't exist

#### 409 Conflict
```json
{
  "success": false,
  "message": "This item is already in your wishlist",
  "statusCode": 409,
  "errorCode": "WISHLIST_DUPLICATE"
}
```
**Cause**: Trying to add duplicate to wishlist

#### 400 Bad Request
```json
{
  "success": false,
  "message": "Insufficient stock. Only 5 items available.",
  "statusCode": 400
}
```
**Cause**: Requested quantity exceeds available stock

---

## Testing Scenarios

### Cart Tests
```
? Add item to empty cart
? Add item to existing cart
? Add same variant twice (increases quantity)
? Update quantity
? Remove item
? Clear cart
? Stock validation
? Price locking
```

### Wishlist Tests
```
? Add item to wishlist
? Remove item from wishlist
? Check if item in wishlist
? Prevent duplicate additions
? Handle deleted variants gracefully
```

---

## Migration

Run the following to create database tables:

```bash
cd Tekno.Infrastructure
dotnet ef migrations add AddCartAndWishlist --startup-project ../Tekno.Api
dotnet ef database update --startup-project ../Tekno.Api
```

---

## Performance Considerations

### Optimizations
1. **Eager Loading**: Variant details loaded with Product, Brand, Category, Attributes
2. **AsNoTracking**: Read-only queries use no-tracking for better performance
3. **Indexing**: Unique indexes on (cart_id, variant_id) and (user_id, variant_id)
4. **Caching**: Consider caching variant details (future enhancement)

### Scalability
- Cart stored in database (not session/cookies)
- Stateless API design
- Can scale horizontally

---

## Security

### Authentication
- All endpoints require JWT authentication
- User ID extracted from JWT claims
- Users can only access their own cart/wishlist

### Validation
- Stock validation on every add/update
- Variant existence validation
- Quantity > 0 validation

### Best Practices
- No sensitive data in cart
- Price locked at add time (prevents manipulation)
- Soft deletes (mark as deleted, don't actually delete)

---

## Future Enhancements

- [ ] Cart expiration (auto-clear after 30 days)
- [ ] Move wishlist item to cart
- [ ] Share wishlist
- [ ] Cart sync across devices
- [ ] Price drop notifications for wishlist
- [ ] Bulk operations (add multiple items)
- [ ] Cart recovery (save abandoned carts)
- [ ] Guest cart (anonymous users)

---

## Related Documentation
- [Product Search Documentation](./ProductSearch.md)
- [Product Variant System](./ProductVariants.md)
- [Authentication Guide](./Authentication.md)
