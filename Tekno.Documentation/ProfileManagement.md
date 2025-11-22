# User Profile Management System Documentation

## ?? Overview
Complete user profile management system for the Tekno e-commerce platform with support for:
- ? Profile management (fullname, phone number)
- ? Email management (with password verification)
- ? Password change (with current password verification)
- ? Multiple address management
- ? Default address selection

---

## ?? API Endpoints

### ?? **All endpoints require authentication** (`Authorization: Bearer {token}`)

Base URL: `/api/profile`

---

## ?? **Profile Management**

### 1. Get Current User Profile
```http
GET /api/profile
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "fullname": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "+84987654321",
    "role": "Customer",
    "createdAt": "2025-01-01T00:00:00Z",
    "updatedAt": "2025-01-15T10:30:00Z",
    "addresses": [
      {
        "id": 1,
        "recipientName": "John Doe",
        "phoneNumber": "+84987654321",
        "addressLine1": "123 Nguyen Hue Street",
        "addressLine2": "Apartment 5B",
        "city": "Ho Chi Minh City",
        "state": "Ho Chi Minh",
        "postalCode": "700000",
        "country": "Vietnam",
        "isDefault": true,
        "createdAt": "2025-01-01T00:00:00Z"
      }
    ]
  }
}
```

### 2. Update Profile (Fullname & Phone Number)
```http
PUT /api/profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "fullname": "John Doe",
  "phoneNumber": "+84987654321"
}
```

**Validation:**
- ? Fullname: 2-100 characters (required)
- ? Phone Number: Valid phone format (optional)

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "fullname": "John Doe",
    "phoneNumber": "+84987654321",
    // ...other profile fields
  },
  "message": "Profile updated successfully"
}
```

### 3. Update Email
```http
PUT /api/profile/email
Authorization: Bearer {token}
Content-Type: application/json

{
  "newEmail": "newemail@example.com",
  "currentPassword": "MyPassword123"
}
```

**Validation:**
- ? New email must be valid format
- ? New email must not already exist
- ? Current password must be correct

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "newemail@example.com",
    // ...other profile fields
  },
  "message": "Email updated successfully"
}
```

**Error Responses:**
```json
// Wrong password
{
  "success": false,
  "message": "Current password is incorrect",
  "statusCode": 400
}

// Email already in use
{
  "success": false,
  "message": "Email 'newemail@example.com' is already in use",
  "statusCode": 409,
  "errorCode": "EMAIL_EXISTS"
}
```

### 4. Change Password
```http
PUT /api/profile/password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword456",
  "confirmPassword": "NewPassword456"
}
```

**Validation:**
- ? Current password must be correct
- ? New password must be at least 6 characters
- ? Confirm password must match new password

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Password changed successfully"
}
```

**Error Response:**
```json
// Wrong current password
{
  "success": false,
  "message": "Current password is incorrect",
  "statusCode": 400
}

// Passwords don't match
{
  "success": false,
  "message": "Passwords do not match",
  "statusCode": 400
}
```

---

## ?? **Address Management**

### 5. Get All Addresses
```http
GET /api/profile/addresses
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "recipientName": "John Doe",
      "phoneNumber": "+84987654321",
      "addressLine1": "123 Nguyen Hue Street",
      "addressLine2": "Apartment 5B",
      "city": "Ho Chi Minh City",
      "state": "Ho Chi Minh",
      "postalCode": "700000",
      "country": "Vietnam",
      "isDefault": true,
      "createdAt": "2025-01-01T00:00:00Z"
    },
    {
      "id": 2,
      "recipientName": "John Doe",
      "phoneNumber": "+84123456789",
      "addressLine1": "456 Le Loi Boulevard",
      "addressLine2": null,
      "city": "Hanoi",
      "state": "Hanoi",
      "postalCode": "100000",
      "country": "Vietnam",
      "isDefault": false,
      "createdAt": "2025-01-05T00:00:00Z"
    }
  ]
}
```

**Note:** Addresses are sorted by:
1. Default address first
2. Then by creation date (newest first)

### 6. Add New Address
```http
POST /api/profile/addresses
Authorization: Bearer {token}
Content-Type: application/json

{
  "recipientName": "John Doe",
  "phoneNumber": "+84987654321",
  "addressLine1": "123 Nguyen Hue Street",
  "addressLine2": "Apartment 5B",
  "city": "Ho Chi Minh City",
  "state": "Ho Chi Minh",
  "postalCode": "700000",
  "country": "Vietnam",
  "isDefault": true
}
```

**Validation:**
- ? Recipient Name: 2-100 characters (required)
- ? Phone Number: Valid phone format (required)
- ? Address Line 1: 5-200 characters (required)
- ? Address Line 2: Max 200 characters (optional)
- ? City: Max 100 characters (required)
- ? State: Max 100 characters (required)
- ? Postal Code: Max 20 characters (required)
- ? Country: Max 100 characters (required, default: Vietnam)

**Business Rules:**
- If this is the first address, it's automatically set as default
- If `isDefault: true`, other addresses are set to non-default
- User can have multiple addresses but only one default

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 3,
    "recipientName": "John Doe",
    "phoneNumber": "+84987654321",
    "addressLine1": "123 Nguyen Hue Street",
    "addressLine2": "Apartment 5B",
    "city": "Ho Chi Minh City",
    "state": "Ho Chi Minh",
    "postalCode": "700000",
    "country": "Vietnam",
    "isDefault": true,
    "createdAt": "2025-01-15T10:30:00Z"
  },
  "message": "Address added successfully"
}
```

### 7. Update Address
```http
PUT /api/profile/addresses/{addressId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "recipientName": "John Doe",
  "phoneNumber": "+84987654321",
  "addressLine1": "456 Updated Street",
  "addressLine2": null,
  "city": "Ho Chi Minh City",
  "state": "Ho Chi Minh",
  "postalCode": "700000",
  "country": "Vietnam"
}
```

**Validation:**
- ? Same as Add Address
- ? User can only update their own addresses

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "recipientName": "John Doe",
    "phoneNumber": "+84987654321",
    "addressLine1": "456 Updated Street",
    // ...other fields
  },
  "message": "Address updated successfully"
}
```

### 8. Set Default Address
```http
PATCH /api/profile/addresses/{addressId}/default
Authorization: Bearer {token}
```

**Behavior:**
- Sets the specified address as default
- Unsets all other addresses as non-default

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Default address updated"
}
```

### 9. Delete Address
```http
DELETE /api/profile/addresses/{addressId}
Authorization: Bearer {token}
```

**Business Rules:**
- ? User can only delete their own addresses
- ? Cannot delete the only address if it's default
- ? If deleting default address (with multiple addresses), another address is automatically set as default

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Address deleted successfully"
}
```

**Error Response:**
```json
// Trying to delete only address
{
  "success": false,
  "message": "Cannot delete the only address",
  "statusCode": 400
}

// Not found
{
  "success": false,
  "message": "Address not found",
  "statusCode": 404
}
```

---

## ??? Database Schema

### users Table
```sql
Column          Type            Description
-------------   ------------    ---------------------------
id              INT             Primary key
fullname        VARCHAR(100)    User's full name
email           VARCHAR(255)    Email (unique)
phone_number    VARCHAR(20)     Phone number (optional)
password_hash   VARCHAR         Hashed password
role_id         INT             Foreign key to roles
created_at      TIMESTAMPTZ     Account creation time
updated_at      TIMESTAMPTZ     Last update time (nullable)

Unique Index: email
Foreign Key: role_id ? roles(id)
```

### user_addresses Table
```sql
Column          Type            Description
-------------   ------------    ---------------------------
id              INT             Primary key
user_id         INT             Foreign key to users
recipient_name  VARCHAR(100)    Recipient name
phone_number    VARCHAR(20)     Contact phone
address_line1   VARCHAR(200)    Primary address line
address_line2   VARCHAR(200)    Secondary address line (nullable)
city            VARCHAR(100)    City
state           VARCHAR(100)    State/Province
postal_code     VARCHAR(20)     Postal/ZIP code
country         VARCHAR(100)    Country (default: Vietnam)
is_default      BOOLEAN         Is default address (default: false)
created_at      TIMESTAMPTZ     Creation time
updated_at      TIMESTAMPTZ     Last update time (nullable)

Index: user_id
Foreign Key: user_id ? users(id) ON DELETE CASCADE
```

---

## ?? User Workflow

### Profile Management Flow

```
1. User logs in
   POST /api/auth/login
   ??> Receives JWT token

2. Get current profile
   GET /api/profile
   Authorization: Bearer {token}
   ??> Returns profile with addresses

3. Update profile information
   PUT /api/profile
   {
     "fullname": "New Name",
     "phoneNumber": "+84987654321"
   }
   ??> Profile updated

4. Change email (requires password)
   PUT /api/profile/email
   {
     "newEmail": "new@example.com",
     "currentPassword": "current password"
   }
   ??> Email updated (must verify with password)

5. Change password
   PUT /api/profile/password
   {
     "currentPassword": "old password",
     "newPassword": "new password",
     "confirmPassword": "new password"
   }
   ??> Password changed
```

### Address Management Flow

```
1. View all addresses
   GET /api/profile/addresses
   ??> Returns list of addresses (default first)

2. Add new address
   POST /api/profile/addresses
   {
     "recipientName": "John Doe",
     "phoneNumber": "+84987654321",
     "addressLine1": "123 Street",
     "city": "HCMC",
     "state": "HCMC",
     "postalCode": "700000",
     "country": "Vietnam",
     "isDefault": false
   }
   ??> Address created

3. Set as default (for checkout)
   PATCH /api/profile/addresses/2/default
   ??> Address 2 is now default

4. Update address
   PUT /api/profile/addresses/1
   {
     "recipientName": "Updated Name",
     // ...other fields
   }
   ??> Address updated

5. Delete address
   DELETE /api/profile/addresses/3
   ??> Address deleted
```

---

## ?? Frontend Integration

### Profile Management

```javascript
// Get user profile
const getProfile = async () => {
  const response = await fetch('/api/profile', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  const result = await response.json();
  return result.data;
};

// Update profile
const updateProfile = async (fullname, phoneNumber) => {
  const response = await fetch('/api/profile', {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      fullname,
      phoneNumber
    })
  });
  const result = await response.json();
  return result.data;
};

// Change password
const changePassword = async (currentPassword, newPassword) => {
  const response = await fetch('/api/profile/password', {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      currentPassword,
      newPassword,
      confirmPassword: newPassword
    })
  });
  return await response.json();
};
```

### Address Management

```javascript
// Get addresses
const getAddresses = async () => {
  const response = await fetch('/api/profile/addresses', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  const result = await response.json();
  return result.data;
};

// Add address
const addAddress = async (addressData) => {
  const response = await fetch('/api/profile/addresses', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(addressData)
  });
  const result = await response.json();
  return result.data;
};

// Set default address
const setDefaultAddress = async (addressId) => {
  const response = await fetch(`/api/profile/addresses/${addressId}/default`, {
    method: 'PATCH',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  return await response.json();
};

// Delete address
const deleteAddress = async (addressId) => {
  const response = await fetch(`/api/profile/addresses/${addressId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  return await response.json();
};
```

---

## ?? Security Features

### Password Verification
- Email changes require current password
- Password changes require current password
- Prevents unauthorized account modifications

### Ownership Verification
- Users can only view/edit their own profile
- Users can only manage their own addresses
- JWT token contains user ID

### Input Validation
- All inputs validated on both client and server side
- Email format validation
- Phone number format validation
- String length constraints

### Database Security
- Passwords hashed with BCrypt
- Foreign key constraints enforced
- Cascade delete on user deletion

---

## ?? Business Rules

### Profile Updates
1. ? Email must be unique across all users
2. ? Email changes require password verification
3. ? Password changes require current password verification
4. ? Phone number is optional

### Address Management
1. ? First address is automatically set as default
2. ? Only one default address per user
3. ? Setting an address as default unsets others
4. ? Cannot delete the only address
5. ? Deleting default address auto-sets another as default
6. ? Addresses deleted when user is deleted (CASCADE)

---

## ?? Testing Scenarios

### Profile Tests
```
? Get user profile with addresses
? Update fullname and phone number
? Update email with correct password
? Update email with wrong password (fail)
? Update email to existing email (fail)
? Change password with correct current password
? Change password with wrong current password (fail)
? Change password with non-matching confirmation (fail)
```

### Address Tests
```
? Get all addresses (ordered by default, then date)
? Add first address (auto-default)
? Add second address (non-default)
? Set address as default
? Update address details
? Delete non-default address
? Delete default address (auto-sets new default)
? Cannot delete only address
? Cannot update other user's address
```

---

## ?? Migration

Run migrations to create user_addresses table:

```bash
cd Tekno.Infrastructure
dotnet ef migrations add AddUserProfileManagement --startup-project ../Tekno.Api
dotnet ef database update --startup-project ../Tekno.Api
```

---

## ?? Future Enhancements

- [ ] Email verification (send verification email)
- [ ] Phone number verification (OTP)
- [ ] Profile picture upload
- [ ] Address autocomplete (Google Maps API)
- [ ] Address validation (verify postal code)
- [ ] Two-factor authentication (2FA)
- [ ] Login history tracking
- [ ] Account deletion request
- [ ] Export personal data (GDPR compliance)
- [ ] Social media account linking

---

## ?? Related Documentation
- [Authentication System](./Authentication.md)
- [Order System](./OrderSystem.md) - Uses addresses for shipping
- [Cart System](./CartAndWishlist.md)

---

**Date:** 2025-01-15  
**Status:** ? Production Ready  
**Security Level:** High (Password verification required for sensitive operations)
