using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Tekno.Domain.Auth;
using Tekno.Domain.Catalog;
using Tekno.Domain.Promotion;
using Tekno.Domain.Cart;
using Tekno.Domain.Review;
using Tekno.Domain.Order;
using Tekno.Infrastructure.Persistence.Configurations;

namespace Tekno.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductAttribute> Attributes => Set<ProductAttribute>();
        public DbSet<AttributeValue> AttributeValues => Set<AttributeValue>();
        public DbSet<ProductVariantAttribute> ProductVariantAttributes => Set<ProductVariantAttribute>();
        
        // Coupon entities
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<CouponCategory> CouponCategories => Set<CouponCategory>();
        public DbSet<CouponProduct> CouponProducts => Set<CouponProduct>();
        public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();
        
        // Cart entities
        public DbSet<UserCart> UserCarts => Set<UserCart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        
        // Review entities
        public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
        public DbSet<ReviewHelpfulness> ReviewHelpfulness => Set<ReviewHelpfulness>();
        
        // Order entities (simplified for purchase verification)
        public DbSet<Tekno.Domain.Order.Order> Orders => Set<Tekno.Domain.Order.Order>();
        public DbSet<Tekno.Domain.Order.OrderItem> OrderItems => Set<Tekno.Domain.Order.OrderItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thứ tự quan trọng: các bảng độc lập phải được seed trước
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());

            // Các bảng phụ thuộc (product cần brand + category trước)
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new AttributeValueConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariantAttributeConfiguration());
            
            // Coupon configurations
            modelBuilder.ApplyConfiguration(new CouponConfiguration());
            modelBuilder.ApplyConfiguration(new CouponCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CouponProductConfiguration());
            modelBuilder.ApplyConfiguration(new CouponUsageConfiguration());
            
            // Cart configurations
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistConfiguration());
            
            // Review configurations
            modelBuilder.ApplyConfiguration(new ProductReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewHelpfulnessConfiguration());
            
            // Order configurations
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        }
    }
}
