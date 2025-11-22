# Dependency Injection Registration - Coupon System

## Services Registered

The following services have been registered in `Program.cs` for the Coupon/Promotion system:

### Service Registration

```csharp
// Coupon/Promotion services
builder.Services.AddScoped<Tekno.Application.Promotion.Services.CouponService>();
builder.Services.AddScoped<Tekno.Application.Promotion.Interface.ICouponRepository, Tekno.Infrastructure.Promotion.CouponRepository>();
```

### Service Lifetimes

| Service | Interface | Implementation | Lifetime | Reason |
|---------|-----------|----------------|----------|---------|
| `CouponService` | - | `CouponService` | Scoped | Per-request lifecycle, works with EF Core DbContext |
| `ICouponRepository` | Interface | `CouponRepository` | Scoped | Per-request lifecycle, uses EF Core DbContext |

## Dependencies Chain

```
CouponController (Transient)
    ?
CouponService (Scoped)
    ?
??? ICouponRepository (Scoped)
?   ??? AppDbContext (Scoped)
??? IMapper (Singleton)
??? ILogger<CouponService> (Singleton)
```

## Usage in Controllers

### Public Controller
```csharp
public class CouponController : ControllerBase
{
    private readonly CouponService _couponService;

    public CouponController(CouponService couponService)
    {
        _couponService = couponService;
    }
}
```

### Admin Controller
```csharp
public class AdminCouponController : ControllerBase
{
    private readonly CouponService _couponService;

    public AdminCouponController(CouponService couponService)
    {
        _couponService = couponService;
    }
}
```

## Troubleshooting

### Error: "Unable to resolve service for type 'CouponService'"

**Cause:** Service not registered in DI container

**Solution:** Ensure the following line is in `Program.cs`:
```csharp
builder.Services.AddScoped<Tekno.Application.Promotion.Services.CouponService>();
```

### Error: "Unable to resolve service for type 'ICouponRepository'"

**Cause:** Repository interface not mapped to implementation

**Solution:** Ensure the following line is in `Program.cs`:
```csharp
builder.Services.AddScoped<Tekno.Application.Promotion.Interface.ICouponRepository, 
    Tekno.Infrastructure.Promotion.CouponRepository>();
```

## Related Services

The coupon system also depends on these existing services:

| Service | Purpose | Already Registered |
|---------|---------|-------------------|
| `AppDbContext` | Database access | ? Via `AddInfrastructure()` |
| `IMapper` | AutoMapper for DTOs | ? Via `AddAutoMapper()` |
| `ILogger<T>` | Logging | ? Framework default |

## Service Scope Best Practices

### ? Correct Usage
```csharp
// Service is injected via constructor
public class CouponController : ControllerBase
{
    private readonly CouponService _couponService;
    
    public CouponController(CouponService couponService)
    {
        _couponService = couponService; // ? Scoped per request
    }
}
```

### ? Incorrect Usage
```csharp
// DON'T create service manually
public class CouponController : ControllerBase
{
    public async Task<IActionResult> GetCoupons()
    {
        var service = new CouponService(...); // ? Wrong!
        // This bypasses DI and creates issues with DbContext lifecycle
    }
}
```

## Testing

For unit testing, you can mock the services:

```csharp
[Fact]
public async Task GetActiveCoupons_ShouldReturnCoupons()
{
    // Arrange
    var mockRepo = new Mock<ICouponRepository>();
    var mockMapper = new Mock<IMapper>();
    var mockLogger = new Mock<ILogger<CouponService>>();
    
    var service = new CouponService(
        mockRepo.Object,
        mockMapper.Object,
        mockLogger.Object
    );
    
    // Act & Assert
    // ...
}
```

## Migration Checklist

When adding new services to the Coupon system:

- [ ] Create service class in `Tekno.Application/Promotion/Services`
- [ ] Create interface (if needed) in `Tekno.Application/Promotion/Interface`
- [ ] Create implementation (if repository) in `Tekno.Infrastructure/Promotion`
- [ ] Register in `Program.cs` with appropriate lifetime
- [ ] Verify build succeeds
- [ ] Test endpoint functionality

## Complete Registration Block in Program.cs

```csharp
// =======================================================
// 4. APPLICATION & INFRASTRUCTURE DEPENDENCIES
// =======================================================
builder.Services.AddInfrastructure(builder.Configuration);

// Auth
builder.Services.AddScoped<AuthService>();

// Catalog
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductService>();

// Media
builder.Services.AddScoped<MediaService>();

// Coupon/Promotion
builder.Services.AddScoped<Tekno.Application.Promotion.Services.CouponService>();
builder.Services.AddScoped<Tekno.Application.Promotion.Interface.ICouponRepository, 
    Tekno.Infrastructure.Promotion.CouponRepository>();
```

## Verification

After registration, verify the services are working:

```bash
# Test public endpoint
curl http://localhost:5000/api/coupons/active

# Test admin endpoint (requires auth)
curl -H "Authorization: Bearer {token}" \
     http://localhost:5000/api/admin/coupons
```

If you see errors like:
- `401 Unauthorized` ? Authentication issue (expected for admin endpoints)
- `500 Internal Server Error: Unable to resolve service` ? DI registration issue
- `404 Not Found` ? Routing issue

## Performance Notes

- **Scoped lifetime** ensures one instance per HTTP request
- Services are disposed automatically at the end of the request
- DbContext lifecycle is managed properly
- No memory leaks from unclosed connections

## See Also

- [ASP.NET Core Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Service Lifetimes](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Coupon System Documentation](./CouponSystem.md)
