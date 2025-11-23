# Coupon System - API Split Summary

## Overview
The coupon system endpoints have been split into two separate controllers for better organization and security:

## ?? Controller Structure

### 1. **CouponController** (Public/Customer Endpoints)
**Route:** `/api/coupons`  
**Access:** Public (some endpoints require authentication)

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| GET | `/active` | No | Get all active coupons |
| GET | `/{code}` | No | Get coupon by code |
| GET | `/check/{code}` | No | Quick availability check |
| POST | `/validate` | No | Validate coupon for cart |
| GET | `/my-usage` | Yes (User) | Get user's usage history |
| GET | `/for-product/{productId}` | No | Get coupons for product |
| GET | `/for-category/{categoryId}` | No | Get coupons for category |

### 2. **AdminCouponController** (Admin Management)
**Route:** `/api/admin/coupons`  
**Access:** Admin only (all endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get paginated coupons with filters |
| GET | `/{id}` | Get coupon by ID |
| POST | `/` | Create new coupon |
| PUT | `/{id}` | Update existing coupon |
| DELETE | `/{id}` | Delete coupon |
| GET | `/{id}/usage` | Get usage history |
| GET | `/{id}/statistics` | Get coupon statistics |
| PATCH | `/{id}/activate` | Activate coupon |
| PATCH | `/{id}/deactivate` | Deactivate coupon |

## ?? Security & Authorization

### Public Endpoints
- No authentication required for browsing and validation
- Authentication optional for `/validate` (auto-fills userId if logged in)
- User authentication required for `/my-usage`

### Admin Endpoints
- **ALL endpoints require `Admin` role**
- Applied via `[Authorize(Roles = "Admin")]` attribute at controller level
- Returns `401 Unauthorized` if not authenticated
- Returns `403 Forbidden` if authenticated but not admin

## ?? Use Cases

### Customer Flow
```
1. Browse active coupons
   GET /api/coupons/active

2. Check specific coupon
   GET /api/coupons/check/PHVC000003

3. Add items to cart

4. Validate coupon before checkout
   POST /api/coupons/validate
   {
     "code": "PHVC000003",
     "orderAmount": 500000,
     "productIds": [1, 2],
     "categoryIds": [5]
   }

5. Apply discount if valid

6. View usage history (after checkout)
   GET /api/coupons/my-usage
```

### Admin Flow
```
1. View all coupons with filtering
   GET /api/admin/coupons?search=summer&status=Active

2. Create new coupon
   POST /api/admin/coupons
   { code, name, type, value, ... }

3. Monitor usage
   GET /api/admin/coupons/{id}/statistics
   GET /api/admin/coupons/{id}/usage

4. Update or deactivate as needed
   PUT /api/admin/coupons/{id}
   PATCH /api/admin/coupons/{id}/deactivate
```

## ?? Key Features by Endpoint

### Customer Features
? Browse available promotions  
? Real-time validation  
? Product/category-specific filtering  
? Personal usage tracking  
? Anonymous validation support  

### Admin Features
? Full CRUD operations  
? Advanced filtering & search  
? Usage analytics  
? Status management  
? Usage history tracking  
? Statistics & reporting  

## ?? Migration Notes

### Breaking Changes
If upgrading from the combined controller:

**Before:**
```http
GET /api/coupons?search=summer&page=1
Authorization: Bearer {admin_token}
```

**After:**
```http
GET /api/admin/coupons?search=summer&page=1
Authorization: Bearer {admin_token}
```

### Non-Breaking Changes
All public endpoints remain the same:
- `/api/coupons/active` ?
- `/api/coupons/{code}` ?
- `/api/coupons/validate` ?

## ?? Benefits of Split Architecture

1. **Clear Separation of Concerns**
   - Customer functionality separate from admin operations
   - Easier to maintain and extend

2. **Better Security**
   - Admin functions protected at controller level
   - Reduced risk of accidentally exposing admin features

3. **Improved API Documentation**
   - Clearer Swagger/OpenAPI documentation
   - Separate sections for different user roles

4. **Performance**
   - Can apply different caching strategies
   - Rate limiting can be role-specific

5. **Scalability**
   - Admin and public endpoints can scale independently
   - Easier to add middleware per role

## ?? Example Requests

### Customer: Validate Coupon
```bash
curl -X POST "https://api.tekno.com/api/coupons/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "PHVC000003",
    "orderAmount": 500000,
    "productIds": [1, 2, 3],
    "categoryIds": [5]
  }'
```

### Admin: Create Coupon
```bash
curl -X POST "https://api.tekno.com/api/admin/coupons" \
  -H "Authorization: Bearer {admin_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "BLACKFRIDAY25",
    "name": "Black Friday 2025",
    "type": "Percentage",
    "value": 25,
    "quantity": 1000,
    "startDate": "2025-11-25T00:00:00Z",
    "endDate": "2025-11-30T23:59:59Z"
  }'
```

### Admin: Get Statistics
```bash
curl -X GET "https://api.tekno.com/api/admin/coupons/3/statistics" \
  -H "Authorization: Bearer {admin_token}"
```

## ?? Testing Checklist

### Public Endpoints
- [ ] Can access `/active` without auth
- [ ] Can validate coupon without auth
- [ ] Can check coupon without auth
- [ ] Cannot access admin endpoints without admin token
- [ ] Can access `/my-usage` with user auth
- [ ] userId auto-fills from token in `/validate`

### Admin Endpoints
- [ ] Cannot access without authentication
- [ ] Cannot access with user (non-admin) token
- [ ] Can access all CRUD operations with admin token
- [ ] Can view statistics
- [ ] Can view usage history
- [ ] Pagination works correctly

## ?? Related Documentation
- [Full Coupon System Documentation](./CouponSystem.md)
- [API Authentication Guide](./Authentication.md)
- [Admin Role Setup](./AdminSetup.md)
