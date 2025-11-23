# Coupon System Documentation

## Overview
Complete coupon/voucher system for Tekno e-commerce platform with validation, usage tracking, and flexible rule-based discounts.

## Features
? Fixed amount & percentage discounts
? Category & product-specific coupons  
? Usage limits (global & per-user)
? Minimum purchase requirements
? Maximum discount caps
? Active date range validation
? Usage history tracking
? Admin CRUD operations
? Separate Admin & User endpoints

## Architecture

### Controllers
The coupon system is split into two controllers:

1. **`CouponController`** (`/api/coupons`) - Public/Customer endpoints
   - Browse active coupons
   - Validate coupons
   - Check coupon availability
   - View usage history (authenticated users)

2. **`AdminCouponController`** (`/api/admin/coupons`) - Admin-only endpoints
   - Full CRUD operations
   - Coupon management
   - Usage analytics
   - Status management

## API Endpoints

### ?? Public Endpoints (`/api/coupons`)

#### 1. Get Active Coupons
```http
GET /api/coupons/active
```
Returns all currently active and valid coupons (public access).

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "code": "PHVC000003",
      "name": "Return",
      "type": "FixedAmount",
      "value": 300000,
      "status": "Active",
      ...
    }
  ],
  "message": "Found 3 active coupon(s)"
}
```

#### 2. Get Coupon by Code
```http
GET /api/coupons/{code}
```
Get coupon details by code (e.g., `PHVC000003`).

#### 3. Check Coupon Exists (Quick)
```http
GET /api/coupons/check/{code}
```
Quick check without full validation - returns basic info and usability status.

**Response:**
```json
{
  "success": true,
  "data": {
    "exists": true,
    "usable": true,
    "name": "Return",
    "type": "FixedAmount",
    "value": 300000,
    "minPurchaseAmount": null,
    "message": "Coupon is available"
  }
}
```

#### 4. Validate Coupon
```http
POST /api/coupons/validate
Content-Type: application/json

{
  "code": "PHVC000003",
  "orderAmount": 500000,
  "userId": 1,
  "productIds": [1, 2, 3],
  "categoryIds": [5, 8]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "message": "Coupon applied! You save 300,000 VND",
    "discountAmount": 300000,
    "coupon": { /* coupon details */ }
  }
}
```

#### 5. Get My Coupon Usage
```http
GET /api/coupons/my-usage?page=1&pageSize=20
Authorization: Bearer {token}
```
View authenticated user's coupon usage history.

#### 6. Get Coupons for Product
```http
GET /api/coupons/for-product/{productId}
```
Get all applicable coupons for a specific product.

#### 7. Get Coupons for Category
```http
GET /api/coupons/for-category/{categoryId}
```
Get all applicable coupons for a specific category.

---

### ?? Admin Endpoints (`/api/admin/coupons`)
**All endpoints require `Admin` role authorization**

#### 1. Get Paginated Coupons
```http
GET /api/admin/coupons?search=summer&status=Active&page=1&pageSize=20
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `search`: Search by code or name
- `status`: Filter by Active, Inactive, or Expired
- `startDate`: Filter by start date (from)
- `endDate`: Filter by end date (to)
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)

#### 2. Get Coupon by ID
```http
GET /api/admin/coupons/{id}
Authorization: Bearer {admin_token}
```

#### 3. Create Coupon
```http
POST /api/admin/coupons
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "code": "PHVC000004",
  "name": "Black Friday",
  "type": "Percentage",
  "value": 20,
  "quantity": 100,
  "maxUsagePerUser": 1,
  "minPurchaseAmount": 1000000,
  "maxDiscountAmount": 500000,
  "startDate": "2025-11-25T00:00:00Z",
  "endDate": "2025-11-30T23:59:59Z",
  "note": "Black Friday sale",
  "applicableCategoryIds": [1, 2],
  "applicableProductIds": []
}
```

#### 4. Update Coupon
```http
PUT /api/admin/coupons/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Black Friday Extended",
  "value": 25,
  "quantity": 150,
  ...
}
```

**Note:** Cannot update coupon code. Create new coupon if code needs to change.

#### 5. Delete Coupon
```http
DELETE /api/admin/coupons/{id}
Authorization: Bearer {admin_token}
```

**Warning:** Deletes usage history. Consider deactivating instead.

#### 6. Get Usage History
```http
GET /api/admin/coupons/{id}/usage?page=1&pageSize=20
Authorization: Bearer {admin_token}
```

#### 7. Get Coupon Statistics
```http
GET /api/admin/coupons/{id}/statistics
Authorization: Bearer {admin_token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "code": "PHVC000003",
    "name": "Return",
    "totalAvailable": 10,
    "usedCount": 3,
    "remainingCount": 7,
    "usageRate": 30.0,
    "status": "Active",
    "isActive": true,
    "isExpired": false,
    "daysRemaining": 45
  }
}
```

#### 8. Activate Coupon
```http
PATCH /api/admin/coupons/{id}/activate
Authorization: Bearer {admin_token}
```

#### 9. Deactivate Coupon
```http
PATCH /api/admin/coupons/{id}/deactivate
Authorization: Bearer {admin_token}
```

## Coupon Types

### 1. Fixed Amount
Subtracts a fixed value from the order total.
```json
{
  "type": "FixedAmount",
  "value": 300000  // 300,000 VND off
}
```

### 2. Percentage
Applies a percentage discount with optional cap.
```json
{
  "type": "Percentage",
  "value": 20,  // 20% off
  "maxDiscountAmount": 500000  // Cap at 500,000 VND
}
```

### 3. Free Shipping
Provides free shipping (value represents shipping cost).
```json
{
  "type": "FreeShipping",
  "value": 50000  // Shipping cost waived
}
```

## Validation Rules

Coupons are validated against the following conditions:

1. **Active Status**: Coupon must have `Status = Active`
2. **Date Range**: Current date must be between `StartDate` and `EndDate`
3. **Quantity**: `RemainingQuantity > 0`
4. **Minimum Purchase**: `OrderAmount >= MinPurchaseAmount`
5. **User Usage Limit**: User hasn't exceeded `MaxUsagePerUser`
6. **Product/Category Filter**: Cart items must match applicable products or categories (if specified)

## Database Schema

### coupons Table
```sql
Column                Type            Description
-------------------   ------------    --------------------------
id                    INT             Primary key
code                  VARCHAR(50)     Unique coupon code (e.g., PHVC000003)
name                  VARCHAR(200)    Display name (e.g., "Return")
type                  VARCHAR(20)     FixedAmount, Percentage, FreeShipping
value                 DECIMAL(12,2)   Discount value
quantity              INT             Total available coupons
used_count            INT             How many times used
max_usage_per_user    INT             Limit per user (NULL = unlimited)
min_purchase_amount   DECIMAL(12,2)   Minimum order value
max_discount_amount   DECIMAL(12,2)   Cap for percentage discounts
start_date            TIMESTAMPTZ     Valid from
end_date              TIMESTAMPTZ     Valid until
status                VARCHAR(20)     Active, Inactive, Expired
note                  VARCHAR(500)    Admin notes
created_at            TIMESTAMPTZ     Creation timestamp
updated_at            TIMESTAMPTZ     Last update timestamp
```

### coupon_categories Table
Many-to-many relationship between coupons and categories.

### coupon_products Table
Many-to-many relationship between coupons and products.

### coupon_usages Table
Tracks every coupon redemption with user, order, and discount amount.

## Usage in Order Flow

### 1. Add to Cart
User adds products to cart.

### 2. Apply Coupon
```javascript
// Frontend: User enters coupon code
const response = await fetch('/api/coupons/validate', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    code: 'PHVC000003',
    orderAmount: cartTotal,
    userId: currentUser.id,
    productIds: cart.items.map(i => i.productId),
    categoryIds: cart.items.map(i => i.categoryId)
  })
});

if (response.data.isValid) {
  // Show discount applied message
  // Update cart total
}
```

### 3. Checkout
When order is created, call `ApplyCouponAsync` to:
- Record usage in `coupon_usages`
- Increment `used_count`
- Apply discount to order

```csharp
// In OrderService.cs
await _couponService.ApplyCouponAsync(
    couponCode, 
    userId, 
    orderId, 
    orderAmount
);
```

## Security Considerations

1. **Rate Limiting**: Implement rate limiting on `/validate` endpoint
2. **Code Format**: Enforce uppercase, no special characters
3. **Audit Trail**: All usage is logged in `coupon_usages`
4. **Admin Only**: Create/Update/Delete requires Admin role
5. **Atomic Operations**: Usage count updates use database transactions

## Example Scenarios

### Scenario 1: New Customer Discount
```json
{
  "code": "WELCOME10",
  "type": "Percentage",
  "value": 10,
  "maxUsagePerUser": 1,
  "minPurchaseAmount": 500000,
  "applicableCategoryIds": [],  // All categories
  "applicableProductIds": []    // All products
}
```

### Scenario 2: Laptop Category Sale
```json
{
  "code": "LAPTOP20",
  "type": "Percentage",
  "value": 20,
  "maxDiscountAmount": 2000000,
  "applicableCategoryIds": [1],  // Laptop category
  "applicableProductIds": []
}
```

### Scenario 3: Flash Sale - Specific Product
```json
{
  "code": "FLASH500",
  "type": "FixedAmount",
  "value": 500000,
  "quantity": 50,
  "maxUsagePerUser": 1,
  "applicableCategoryIds": [],
  "applicableProductIds": [5, 8, 12]  // Specific products
}
```

## Testing

### Unit Tests
```csharp
[Fact]
public async Task ValidateCoupon_Expired_ShouldReturnInvalid()
{
    // Arrange
    var coupon = new Coupon(
        code: "EXPIRED",
        endDate: DateTime.UtcNow.AddDays(-1),
        // ...
    );
    
    // Act
    var result = await _service.ValidateCouponAsync(dto);
    
    // Assert
    Assert.False(result.IsValid);
    Assert.Contains("expired", result.Message.ToLower());
}
```

## Migration

Run the following command to create database tables:

```bash
cd Tekno.Infrastructure
dotnet ef migrations add AddCouponSystem --startup-project ../Tekno.Api
dotnet ef database update --startup-project ../Tekno.Api
```

## Future Enhancements

- [ ] Coupon stacking rules
- [ ] Referral coupons (generate unique codes per user)
- [ ] Bulk coupon generation
- [ ] Coupon bundles (buy X get coupon for Y)
- [ ] Scheduled activation/deactivation
- [ ] Email notifications for expiring coupons
- [ ] Analytics dashboard (redemption rates, revenue impact)
