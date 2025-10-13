using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("category");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasIndex(c => c.Slug).IsUnique();

            builder.Property(c => c.Description)
                .HasColumnType("text");

            // Self-reference (Category - SubCategory)
            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("NOW()");
            builder.HasData(
                // ===== Cấp cha =====
                new
                {
                    id = 1,
                    name = "Laptop",
                    slug = "laptop",
                    description = "All kinds of laptops",
                    parentId = (int?)null
                },
                new
                {
                    id = 2,
                    name = "Smartphone",
                    slug = "smartphone",
                    description = "All kinds of smartphones",
                    parentId = (int?)null
                },
                new
                {
                    id = 3,
                    name = "Tablet",
                    slug = "tablet",
                    description = "All kinds of tablets",
                    parentId = (int?)null
                },
                new
                {
                    id = 4,
                    name = "Accessory",
                    slug = "accessory",
                    description = "External product that enhances main product experience",
                    parentId = (int?)null
                },
                new
                {
                    id = 5,
                    name = "Camera",
                    slug = "camera",
                    description = "All kinds of cameras",
                    parentId = (int?)null
                },
                new
                {
                    id = 6,
                    name = "Computer & Office",
                    slug = "computer-office",
                    description = "PC and office related products",
                    parentId = (int?)null
                },
                new
                {
                    id = 7,
                    name = "Gaming",
                    slug = "gaming",
                    description = "Gaming products and accessories",
                    parentId = (int?)null
                },

                // ===== Subcategories of Computer & Office =====
                new
                {
                    id = 8,
                    name = "Monitor",
                    slug = "monitor",
                    description = "All types of computer monitors",
                    parentId = 6
                },
                new
                {
                    id = 9,
                    name = "CPU",
                    slug = "cpu",
                    description = "Processors and chips for computers",
                    parentId = 6
                },
                new
                {
                    id = 10,
                    name = "GPU",
                    slug = "gpu",
                    description = "Graphics cards for PCs and laptops",
                    parentId = 6
                },
                new
                {
                    id = 11,
                    name = "RAM",
                    slug = "ram",
                    description = "Memory modules for PCs and laptops",
                    parentId = 6
                },
                new
                {
                    id = 12,
                    name = "Storage (SSD / HDD)",
                    slug = "storage",
                    description = "Storage devices: SSD, HDD, memory cards",
                    parentId = 6
                },

                // ===== Accessories (Global, dùng chung nhiều loại thiết bị) =====
                new
                {
                    id = 13,
                    name = "Keyboard",
                    slug = "keyboard",
                    description = "Keyboards for PC, Laptop, and Tablet",
                    parentId = 4 // thuộc nhóm Accessory
                },
                new
                {
                    id = 14,
                    name = "Mouse",
                    slug = "mouse",
                    description = "Computer and laptop mice (wired, wireless, gaming)",
                    parentId = 4 // thuộc nhóm Accessory
                },
                new
                {
                    id = 15,
                    name = "Headphone / Headset",
                    slug = "headphone",
                    description = "Audio accessories compatible with PC, Laptop, and Smartphone",
                    parentId = 4 // thuộc nhóm Accessory
                },
                new
                {
                    id = 16,
                    name = "Charger & Cable",
                    slug = "charger-cable",
                    description = "Chargers, adapters, and data cables for all devices",
                    parentId = 4
                },
                new
                {
                    id = 17,
                    name = "Case & Cover",
                    slug = "case-cover",
                    description = "Protective cases for phones, tablets, and laptops",
                    parentId = 4
                }
            );
        }
    }
}
