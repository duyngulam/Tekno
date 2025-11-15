using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Tekno.Domain.Auth;
using Tekno.Domain.Catalog;
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
        }

    }
}
