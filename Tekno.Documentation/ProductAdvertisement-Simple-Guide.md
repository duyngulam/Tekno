# Simple Product Advertisement Banner - Quick Guide

## ?? What Is This?

**Super simple product banner system:**
- Upload an image
- Link it to a product
- Display on your website
- When clicked ? goes to product detail page

That's it! No complicated features, just what you need.

---

## ? Features

- ? Upload banner image (JPEG, PNG, WebP)
- ? Link to any product
- ? Multiple display positions (Homepage, Category, etc.)
- ? Priority sorting (control display order)
- ? Schedule campaigns (start/end dates)
- ? Easy activate/deactivate

---

## ?? Quick Start

### 1. Create a Banner (Admin)

```http
POST /api/admin/advertisements
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

image: [banner-file.jpg]
productId: 5
position: "HomeTop"
priority: 10
```

**That's it!** Just an image and a product ID.

### 2. Display on Frontend

```javascript
// Get banners for homepage
const response = await fetch('/api/advertisements/position/HomeTop');
const banners = await response.json();

// Show banners
banners.data.forEach(banner => {
  document.innerHTML += `
    <a href="/products/${banner.productSlug}">
      <img src="${banner.imageUrl}" alt="Banner" />
    </a>
  `;
});
```

---

## ?? API Endpoints

### Admin (Create/Manage)

```
POST   /api/admin/advertisements          Create banner
PUT    /api/admin/advertisements/{id}     Update banner
DELETE /api/admin/advertisements/{id}     Delete banner
GET    /api/admin/advertisements           List banners
PATCH  /api/admin/advertisements/{id}/activate    Turn on
PATCH  /api/admin/advertisements/{id}/deactivate  Turn off
```

### Public (Display)

```
GET /api/advertisements/position/HomeTop        Homepage banners
GET /api/advertisements/position/CategoryTop    Category banners
GET /api/advertisements/active                   All active banners
```

---

## ?? Positions

Where you can display banners:

- **HomeTop** - Homepage hero (big banner)
- **HomeMiddle** - Homepage middle section
- **HomeBottom** - Homepage bottom
- **CategoryTop** - Category page top
- **ProductSidebar** - Product detail sidebar

---

## ?? Simple Examples

### Example 1: Homepage Hero Banner

**Admin creates:**
```javascript
FormData:
  image: summer-sale.jpg
  productId: 5
  position: "HomeTop"
  priority: 10
```

**Frontend displays:**
```javascript
// Get homepage banners
const response = await fetch('/api/advertisements/position/HomeTop');
const banners = await response.json();

// Show first banner (highest priority)
const banner = banners.data[0];
<a href={`/products/${banner.productSlug}`}>
  <img src={banner.imageUrl} />
</a>
```

### Example 2: Category Page Banner

**Admin creates:**
```javascript
FormData:
  image: laptop-deals.jpg
  productId: 12
  position: "CategoryTop"
  priority: 5
```

**Frontend displays:**
```javascript
const response = await fetch('/api/advertisements/position/CategoryTop');
const banners = await response.json();
// Show on category pages
```

---

## ?? Request Examples

### Create Banner

```bash
POST /api/admin/advertisements
Authorization: Bearer eyJhbGc...

Form Data:
- image: [file]
- productId: 5
- position: "HomeTop"
- priority: 10
- startDate: "2025-06-01"  (optional)
- endDate: "2025-08-31"    (optional)
- isActive: true           (optional, default: true)
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "productId": 5,
    "productName": "Dell XPS 13",
    "productSlug": "dell-xps-13",
    "imageUrl": "https://res.cloudinary.com/.../banner.jpg",
    "position": "HomeTop",
    "priority": 10,
    "isActive": true,
    "isCurrentlyActive": true
  }
}
```

### Update Banner (Change Image)

```bash
PUT /api/admin/advertisements/1
Authorization: Bearer eyJhbGc...

Form Data:
- image: [new-banner.jpg]  (optional - only if changing)
- productId: 5
- position: "HomeTop"
- priority: 15             (increased priority)
```

### Get Banners for Position

```bash
GET /api/advertisements/position/HomeTop
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "productId": 5,
      "productSlug": "dell-xps-13",
      "imageUrl": "https://res.cloudinary.com/.../banner.jpg",
      "position": "HomeTop",
      "priority": 10
    }
  ]
}
```

---

## ??? Database

Simple table with just the essentials:

```sql
product_advertisements
??? id              (auto)
??? product_id      (required) ? links to product
??? image_url       (required) ? Cloudinary URL
??? position        (required) ? HomeTop, CategoryTop, etc.
??? priority        (0-100)    ? higher = shown first
??? is_active       (true/false)
??? start_date      (optional) ? campaign start
??? end_date        (optional) ? campaign end
??? created_at      (auto)
```

---

## ?? Priority System

Control which banner shows first:

```
Priority 100 ? Shown first
Priority 50  ? Shown second
Priority 0   ? Shown last
```

Example:
```javascript
// Create high priority banner
POST /api/admin/advertisements
{
  productId: 5,
  priority: 100  // This shows first!
}

// Create normal priority banner
POST /api/admin/advertisements
{
  productId: 8,
  priority: 50   // This shows second
}
```

---

## ?? Campaign Scheduling

Optional: Set start and end dates

```javascript
// Summer sale (June 1 - August 31)
POST /api/admin/advertisements
{
  productId: 5,
  startDate: "2025-06-01T00:00:00Z",
  endDate: "2025-08-31T23:59:59Z"
}

// Banner only shows during these dates
// Before June 1: Not shown
// June 1 - August 31: Shown
// After August 31: Not shown
```

---

## ?? Workflow

### Admin Side

```
1. Upload banner image
2. Select product to link to
3. Choose position (HomeTop, etc.)
4. Set priority (optional)
5. Set dates (optional)
6. Click "Create"
```

### User Side

```
1. Visit website
2. See banner
3. Click banner
4. Go to product page
5. Add to cart
```

---

## ?? Frontend Integration

### React Component

```jsx
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const BannerSlider = ({ position = 'HomeTop' }) => {
  const [banners, setBanners] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    fetch(`/api/advertisements/position/${position}`)
      .then(res => res.json())
      .then(data => setBanners(data.data));
  }, [position]);

  return (
    <div className="banner-slider">
      {banners.map(banner => (
        <img
          key={banner.id}
          src={banner.imageUrl}
          alt="Banner"
          onClick={() => navigate(`/products/${banner.productSlug}`)}
          style={{ cursor: 'pointer' }}
        />
      ))}
    </div>
  );
};

// Usage
<BannerSlider position="HomeTop" />
```

### Vanilla JavaScript

```javascript
// Load banners
async function loadBanners(position) {
  const response = await fetch(`/api/advertisements/position/${position}`);
  const result = await response.json();
  
  const container = document.getElementById('banner-container');
  
  result.data.forEach(banner => {
    const link = document.createElement('a');
    link.href = `/products/${banner.productSlug}`;
    
    const img = document.createElement('img');
    img.src = banner.imageUrl;
    img.alt = 'Banner';
    
    link.appendChild(img);
    container.appendChild(link);
  });
}

// Load on page load
loadBanners('HomeTop');
```

---

## ?? Setup

### 1. Run Migration

```bash
cd Tekno.Infrastructure
dotnet ef migrations add AddSimpleProductAdvertisements --startup-project ../Tekno.Api
dotnet ef database update --startup-project ../Tekno.Api
```

### 2. Test

```bash
# Login as admin
POST /api/auth/login
{
  "email": "admin@tekno.com",
  "password": "admin"
}

# Create first banner
POST /api/admin/advertisements
Authorization: Bearer {token}
Form-Data:
  image: banner.jpg
  productId: 1
  position: "HomeTop"
```

---

## ?? Summary

| What You Need | What You Get |
|---------------|--------------|
| Image file | Upload to Cloudinary |
| Product ID | Link to product |
| Position | Where to show |
| Priority (optional) | Display order |
| Dates (optional) | Campaign schedule |

**That's all!** Simple and focused.

---

## ?? Image Recommendations

### Sizes by Position

| Position | Recommended Size |
|----------|------------------|
| HomeTop | 1920x600px |
| HomeMiddle | 1200x400px |
| CategoryTop | 1200x300px |
| ProductSidebar | 300x600px |

### File Requirements

- **Formats:** JPEG, PNG, WebP
- **Max Size:** 5MB
- **Quality:** 80-90% compression recommended

---

## ? Checklist

Before going live:

- [ ] Migration run successfully
- [ ] Can create banner via admin API
- [ ] Can upload image
- [ ] Image saved to Cloudinary
- [ ] Banner links to correct product
- [ ] Frontend displays banners
- [ ] Click navigates to product page
- [ ] Priority sorting works
- [ ] Can activate/deactivate

---

**Status:** ? Ready to use!  
**Build:** ? Success  
**Complexity:** ?? Simple & Easy
