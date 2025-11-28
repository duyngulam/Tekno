using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;
using static System.Net.WebRequestMethods;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 10, 12, 9, 34, 0, DateTimeKind.Utc);
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
            builder.Property(c => c.IconPath)
                .HasMaxLength(255).HasDefaultValue("https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");
            
            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500)
                .IsRequired(false); // Allow null

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
                    Id = 1,
                    Name = "Laptop",
                    Slug = "laptop",
                    Description = "All kinds of laptops",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://www.svgrepo.com/show/525970/laptop.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/laptop_jchkjn.webp"
                },
                new
                {
                    Id = 2,
                    Name = "Smartphone",
                    Slug = "smartphone",
                    Description = "All kinds of smartphones",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/mobile_qk5kuf.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/iphone_air-3_2_hfq1wl.webp"
                },
                new
                {
                    Id = 3,
                    Name = "Tablet",
                    Slug = "tablet",
                    Description = "All kinds of tablets",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/tablet_mhhzhn.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337716/xiaomi-pad-mini-4_adg1r9.webp"
                },
                new
                {
                    Id = 4,
                    Name = "Accessory",
                    Slug = "accessory",
                    Description = "External product that enhances main product experience",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp"
                },
                new
                {
                    Id = 5,
                    Name = "Camera",
                    Slug = "camera",
                    Description = "All kinds of cameras",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath= "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/camera_xmozh9.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/may-anh-canon-eos-r100_8__havbm2.webp"
                },
                new
                {
                    Id = 6,
                    Name = "Computer & Office",
                    Slug = "computer-office",
                    Description = "PC and office related products",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/devices_kty5xc.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/pc_eoswm6.jpg"
                },
                new
                {
                    Id = 7,
                    Name = "Gaming",
                    Slug = "gaming",
                    Description = "Gaming products and accessories",
                    ParentId = (int?)null,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/game_opdnni.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ps5_lfmig6.webp"
                },

                // ===== Subcategories of Computer & Office =====
                new
                {
                    Id = 8,
                    Name = "Monitor",
                    Slug = "monitor",
                    Description = "All types of computer monitors",
                    ParentId = 6,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/monitor_gfheqk.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/monitor_i9d6or.webp"
                },
                new
                {
                    Id = 9,
                    Name = "CPU",
                    Slug = "cpu",
                    Description = "Processors and chips for computers",
                    ParentId = 6,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/CPU_kfg2fy.webp"
                },
                new
                {
                    Id = 10,
                    Name = "GPU",
                    Slug = "gpu",
                    Description = "Graphics cards for PCs and laptops",
                    ParentId = 6,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/GPU_ltzw4j.webp"
                },
                new
                {
                    Id = 11,
                    Name = "RAM",
                    Slug = "ram",
                    Description = "Memory modules for PCs and laptops",
                    ParentId = 6,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ram_kssmbv.webp"
                },
                new
                {
                    Id = 12,
                    Name = "Storage (SSD / HDD)",
                    Slug = "storage",
                    Description = "Storage devices: SSD, HDD, memory cards",
                    ParentId = 6,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337715/rom_sreazq.webp"
                },

                // ===== Accessories (Global, dùng chung nhiều loại thiết bị) =====
                new
                {
                    Id = 13,
                    Name = "Keyboard",
                    Slug = "keyboard",
                    Description = "Keyboards for PC, Laptop, and Tablet",
                    ParentId = 4, // thuộc nhóm Accessory
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/banphim_iai2rn.jpg"
                },
                new
                {
                    Id = 14,
                    Name = "Mouse",
                    Slug = "mouse",
                    Description = "Computer and laptop mice (wired, wireless, gaming)",
                    ParentId = 4, // thuộc nhóm Accessory
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/mouse_enodsx.png"
                },
                new
                {
                    Id = 15,
                    Name = "Headphone / Headset",
                    Slug = "headphone",
                    Description = "Audio accessories compatible with PC, Laptop, and Smartphone",
                    ParentId = 4, // thuộc nhóm Accessory
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    IconPath = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336342/headphone_pz0fkb.svg",
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/headphone_qn6mqd.jpg"
                },
                new
                {
                    Id = 16,
                    Name = "Charger & Cable",
                    Slug = "charger-cable",
                    Description = "Chargers, adapters, and data cables for all devices",
                    ParentId = 4,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime,
                    ImageUrl = "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp"
                },
                new
                {
                    Id = 17,
                    Name = "Case & Cover",
                    Slug = "case-cover",
                    Description = "Protective cases for phones, tablets, and laptops",
                    ParentId = 4,
                    CreatedAt = SeedTime,
                    UpdatedAt = SeedTime
                }
            );
        }
    }
}
