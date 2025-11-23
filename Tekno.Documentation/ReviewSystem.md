# Product Review System Documentation

## ?? Overview
Complete review/comment system for products with **purchase verification** - only users who bought the product can leave reviews.

## ? Key Features

### For Customers
? **Purchase Verification** - Only verified buyers can review  
? **Rating System** - 1-5 stars  
? **Verified Purchase Badge** - Shows which reviews are from buyers  
? **Helpful Votes** - Mark reviews as helpful/not helpful  
? **Edit/Delete Own Reviews** - Full control over your reviews  
? **Review Summary** - See average rating and distribution  

### For Admins
? **Review Moderation** - Approve/reject reviews  
? **Pending Queue** - All new reviews need approval  
? **User Tracking** - See who wrote each review  

## ?? Security Features

###Purchase Verification Workflow

```
1. User browses products
   ?
2. User places order
   ?
3. Order is completed (status = Completed)
   ?
4. User can now review products from that order
   ?
5. Review is marked as "Verified Purchase" ?
```

### One Review Per Product Per User
- Unique index: `(user_id, product_id)`
- Users cannot submit multiple reviews
- Can edit existing review instead

### Moderation System
- All reviews start as **Pending**
- Admin must **Approve** before public display
- **Rejected** reviews are hidden from public

---

## ?? API Endpoints

### ?? **Public Endpoints** (`/api/products/{productId}/reviews`)

#### 1. Get Product Reviews
```http
GET /api/products/1/reviews?verifiedOnly=true&page=1&pageSize=20
```

**Query Parameters:**
- `verifiedOnly` (optional): Filter to verified purchases only
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20)

**Response:**
```json
{
  "success": true,
  "data": {
    "reviews": [
      {
        "id": 1,
        "productId": 1,
        "userId": 123,
        "userEmail": "user@example.com",
        "rating": 5,
        "title": "Excellent laptop!",
        "comment": "Best purchase ever. Fast, reliable...",
        "status": "Approved",
        "createdAt": "2025-01-15T10:00:00Z",
        "isVerifiedPurchase": true,
        "helpfulCount": 15,
        "notHelpfulCount": 2,
        "variantSku": "XPS13-I5-8-512"
      }
    ],
    "summary": {
      "productId": 1,
      "totalReviews": 50,
      "averageRating": 4.6,
      "ratingDistribution": {
        "5": 35,
        "4": 10,
        "3": 3,
        "2": 1,
        "1": 1
      },
      "verifiedPurchaseCount": 48
    },
    "totalCount": 50,
    "page": 1,
    "pageSize": 20
  }
}
```

#### 2. Get Review Summary
```http
GET /api/products/1/reviews/summary
```

**Response:**
```json
{
  "success": true,
  "data": {
    "productId": 1,
    "totalReviews": 50,
    "averageRating": 4.6,
    "ratingDistribution": {
      "5": 35,
      "4": 10,
      "3": 3,
      "2": 1,
      "1": 1
    },
    "verifiedPurchaseCount": 48
  }
}
```

#### 3. Check if User Can Review
```http
GET /api/products/1/reviews/can-review
Authorization: Bearer {token}
```

**Response (Can Review):**
```json
{
  "success": true,
  "data": {
    "canReview": true,
    "message": "You can review this product",
    "hasPurchased": true,
    "hasAlreadyReviewed": false,
    "eligibleOrders": [
      {
        "orderId": 456,
        "orderNumber": "ORD-2025-001",
        "purchaseDate": "2025-01-10T10:00:00Z",
        "variantId": 1,
        "variantSku": "XPS13-I5-8-512"
      }
    ]
  }
}
```

**Response (Cannot Review - No Purchase):**
```json
{
  "success": true,
  "data": {
    "canReview": false,
    "message": "You can only review products you have purchased",
    "hasPurchased": false,
    "hasAlreadyReviewed": false,
    "eligibleOrders": []
  }
}
```

**Response (Cannot Review - Already Reviewed):**
```json
{
  "success": true,
  "data": {
    "canReview": false,
    "message": "You have already reviewed this product",
    "hasPurchased": true,
    "hasAlreadyReviewed": true,
    "eligibleOrders": []
  }
}
```

#### 4. Create Review
```http
POST /api/products/1/reviews
Authorization: Bearer {token}
Content-Type: application/json

{
  "productId": 1,
  "rating": 5,
  "title": "Excellent laptop!",
  "comment": "This is the best laptop I've ever owned. Fast, reliable, and great battery life.",
  "orderId": 456
}
```

**Validation:**
- ? User must be authenticated
- ? User must have purchased the product
- ? User hasn't already reviewed this product
- ? Rating must be 1-5
- ? Comment must be 10-2000 characters

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "productId": 1,
    "userId": 123,
    "rating": 5,
    "title": "Excellent laptop!",
    "comment": "This is the best laptop...",
    "status": "Pending",
    "createdAt": "2025-01-15T10:00:00Z",
    "isVerifiedPurchase": true
  },
  "message": "Review submitted successfully. It will be visible after approval."
}
```

#### 5. Update Review
```http
PUT /api/products/1/reviews/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "rating": 4,
  "title": "Good laptop",
  "comment": "Updated my review after using for a month..."
}
```

**Note:** Updating a review resets it to "Pending" status.

#### 6. Delete Review
```http
DELETE /api/products/1/reviews/1
Authorization: Bearer {token}
```

#### 7. Vote on Review Helpfulness
```http
POST /api/products/1/reviews/1/vote
Authorization: Bearer {token}
Content-Type: application/json

{
  "isHelpful": true
}
```

**Behavior:**
- Users can change their vote
- Previous vote is removed before new vote is recorded
- Updates `helpfulCount` or `notHelpfulCount`

---

### ?? **Admin Endpoints** (`/api/admin/reviews`)

#### 1. Get Product Reviews (All Statuses)
```http
GET /api/admin/reviews/product/1?status=Pending&page=1&pageSize=20
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `status` (optional): Filter by Pending, Approved, or Rejected
- `page`: Page number
- `pageSize`: Items per page

#### 2. Approve Review
```http
PATCH /api/admin/reviews/1/approve
Authorization: Bearer {admin_token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "status": "Approved",
    "approvedAt": "2025-01-15T14:00:00Z",
    "approvedByUserId": 1
  },
  "message": "Review approved successfully"
}
```

#### 3. Reject Review
```http
PATCH /api/admin/reviews/1/reject
Authorization: Bearer {admin_token}
```

---

## ??? Database Schema

### product_reviews Table
```sql
Column                Type            Description
-------------------   ------------    ---------------------------
id                    INT             Primary key
product_id            INT             Foreign key to products
user_id               INT             Foreign key to users
order_id              INT             Which order (nullable)
variant_id            INT             Which variant purchased (nullable)
rating                INT             1-5 stars
title                 VARCHAR(200)    Review title
comment               VARCHAR(2000)   Review text
status                VARCHAR(20)     Pending, Approved, Rejected
is_verified_purchase  BOOLEAN         True if from completed order
helpful_count         INT             Helpful votes
not_helpful_count     INT             Not helpful votes
created_at            TIMESTAMPTZ     Creation time
updated_at            TIMESTAMPTZ     Last edit time (nullable)
approved_at           TIMESTAMPTZ     Approval time (nullable)
approved_by_user_id   INT             Admin who approved (nullable)

Unique Index: (user_id, product_id)  -- One review per user per product
Index: product_id
Index: user_id
Index: status
```

### review_helpfulness Table
```sql
Column        Type            Description
-----------   ------------    ---------------------------
id            INT             Primary key
review_id     INT             Foreign key to product_reviews
user_id       INT             User who voted
is_helpful    BOOLEAN         True = helpful, False = not helpful
voted_at      TIMESTAMPTZ     When vote was cast

Unique Index: (review_id, user_id)  -- One vote per user per review
```

### orders Table (Simplified)
```sql
Column          Type            Description
-------------   ------------    ---------------------------
id              INT             Primary key
user_id         INT             Foreign key to users
order_number    VARCHAR(50)     Unique order number
status          VARCHAR(20)     Pending, Processing, Completed, Cancelled
total_amount    DECIMAL(12,2)   Order total
created_at      TIMESTAMPTZ     Order creation
completed_at    TIMESTAMPTZ     When order completed (nullable)

Index: user_id
Index: status
Unique Index: order_number
```

### order_items Table
```sql
Column        Type            Description
-----------   ------------    ---------------------------
id            INT             Primary key
order_id      INT             Foreign key to orders
product_id    INT             Which product
variant_id    INT             Which variant
quantity      INT             How many
price         DECIMAL(12,2)   Price per item

Index: order_id
Index: product_id
```

---

## ?? User Workflow

### Customer Journey

```
1. User places order
   POST /api/orders
   ??> Order created with status = Pending

2. Payment processed
   ??> Order status = Processing

3. Order shipped & delivered
   ??> Order status = Completed

4. User can now review products from that order
   GET /api/products/1/reviews/can-review
   ??> canReview = true

5. User writes review
   POST /api/products/1/reviews
   {
     "rating": 5,
     "comment": "Great product!"
   }
   ??> Review created with:
       - status = Pending
       - is_verified_purchase = true
       - order_id = {order_id}

6. Admin approves review
   PATCH /api/admin/reviews/1/approve
   ??> status = Approved

7. Review now visible to public
   GET /api/products/1/reviews
   ??> Shows approved review
```

### Admin Moderation Workflow

```
1. View pending reviews
   GET /api/admin/reviews/product/1?status=Pending

2. Check review content
   ??> If appropriate: Approve
   ??> If inappropriate: Reject

3. Approve
   PATCH /api/admin/reviews/1/approve
   ??> Review appears on product page

4. Reject (spam, offensive, etc.)
   PATCH /api/admin/reviews/1/reject
   ??> Review hidden from public
```

---

## ?? Frontend Integration

### Product Detail Page

```javascript
// Load product reviews
const reviews = await fetch(`/api/products/${productId}/reviews?page=1`)
  .then(r => r.json());

// Display:
// - Average rating: reviews.data.summary.averageRating
// - Total reviews: reviews.data.summary.totalReviews
// - Rating distribution: reviews.data.summary.ratingDistribution
// - Reviews list: reviews.data.reviews
```

### Review Form

```javascript
// Check if user can review
const canReview = await fetch(`/api/products/${productId}/reviews/can-review`, {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

if (!canReview.data.canReview) {
  // Show message: canReview.data.message
  // Hide review form
  return;
}

// Show review form if canReview = true
// Submit review
const response = await fetch(`/api/products/${productId}/reviews`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    productId: productId,
    rating: 5,
    title: 'Great product!',
    comment: 'I love this product because...'
  })
});
```

### Helpful Votes

```javascript
// Vote helpful
await fetch(`/api/products/${productId}/reviews/${reviewId}/vote`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ isHelpful: true })
});

// Update UI:
// - Increment helpfulCount
// - Disable vote buttons (user already voted)
```

---

## ?? Business Rules

### Purchase Verification
1. Order must have `status = Completed`
2. Order must contain the product being reviewed
3. User can review any variant they purchased

### One Review Per Product
- User can only submit one review per product
- Can edit existing review (resets to Pending)
- Can delete existing review

### Review Moderation
- New reviews start as **Pending**
- Only **Approved** reviews shown to public
- **Rejected** reviews hidden but kept in database

### Helpful Votes
- Authenticated users can vote
- One vote per user per review
- Can change vote (removes old, adds new)

---

## ?? Statistics & Analytics

### Product Rating Calculation
```csharp
// Average rating (weighted)
var avgRating = reviews.Average(r => r.Rating);

// Distribution
var distribution = reviews.GroupBy(r => r.Rating)
    .ToDictionary(g => g.Key, g => g.Count());

// Verification rate
var verificationRate = (double)verifiedCount / totalReviews * 100;
```

### Review Quality Metrics
- **Verified Purchase Ratio**: High is good
- **Helpful Vote Ratio**: `helpful / (helpful + notHelpful)`
- **Response Rate**: How many reviews admin responds to

---

## ?? Migration

Run migrations to create tables:

```bash
cd Tekno.Infrastructure
dotnet ef migrations add AddReviewSystem --startup-project ../Tekno.Api
dotnet ef database update --startup-project ../Tekno.Api
```

---

## ?? Testing Scenarios

### Test Cases
```
? User without purchase cannot review
? User with purchase can review
? User cannot review same product twice
? Review starts as Pending
? Only approved reviews shown to public
? User can edit own review
? User can delete own review
? User cannot edit/delete others' reviews
? Admin can approve/reject any review
? Helpful votes recorded correctly
? Review summary calculates correctly
```

---

## ?? Future Enhancements

- [ ] Review images (photo uploads)
- [ ] Review replies (admin/seller responses)
- [ ] Review sorting (helpful, recent, rating)
- [ ] Review filtering (rating, verified, date)
- [ ] Automated moderation (profanity filter)
- [ ] Email notifications (review approved, helpful votes)
- [ ] Review rewards (points for verified reviews)
- [ ] Bulk approve/reject
- [ ] Review export (CSV/PDF)
- [ ] Review analytics dashboard

---

## ?? Related Documentation
- [Order System](./OrderSystem.md) - Full order implementation
- [Cart & Wishlist](./CartAndWishlist.md) - Shopping cart
- [Product Catalog](./ProductCatalog.md) - Product management
- [Authentication](./Authentication.md) - User auth system
