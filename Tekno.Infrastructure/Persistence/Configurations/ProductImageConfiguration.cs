using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("product_image");
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageUrl).IsRequired();
            builder.Property(pi => pi.IsPrimary).HasDefaultValue(false);
            builder.Property(pi => pi.SortOrder).HasDefaultValue(0);

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId);

            // ========== SEED PRODUCT IMAGES (Real Product URLs) ==========$
            builder.HasData(
                // ===== Dell XPS 13 Images =====
                new { Id = 1, ProductId = 1, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_659_40.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 2, ProductId = 1, ImageUrl = "https://cdn.tgdd.vn/Products/Images/44/314838/dell-xps-13-plus-9320-i5-71013325-1-750x500.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== MacBook Air M2 Images =====
                new { Id = 3, ProductId = 2, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/m/a/macbook_1__1_8.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 4, ProductId = 2, ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034", IsPrimary = false, SortOrder = 2 },

                // ===== Asus ZenBook 14 OLED Images =====
                new { Id = 5, ProductId = 3, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/t/e/text_ng_n_24__3_5.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 6, ProductId = 3, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/838fbdac-6d10-4190-8e52-d4b9463f5d23/", IsPrimary = false, SortOrder = 2 },

                // ===== HP Spectre x360 14 Images =====
                new { Id = 7, ProductId = 4, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/l/a/laptop-hp-spectre-x360-14-ef0030tu-6k773pa-cu-dep-1_4.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 8, ProductId = 4, ImageUrl = "https://www.hp.com/content/dam/sites/worldwide/personal-computers/consumer/laptops-and-2-n-1s/spectre/version-2023/HP%20Spectre%20x360%2014__Mobile@2x.png", IsPrimary = false, SortOrder = 2 },

                // ===== ThinkPad X1 Carbon Gen 11 Images =====
                new { Id = 9, ProductId = 5, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_744_2__7.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 10, ProductId = 5, ImageUrl = "https://mac24h.vn/images/detailed/94/ThinkPad_X1_Carbon_Gen_11.png", IsPrimary = false, SortOrder = 2 },

                // ===== iPhone 15 Pro Max Images =====
                new { Id = 11, ProductId = 10, ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-max-bluetitanium-select?wid=470&hei=556&fmt=png-alpha&.v=1692845702781", IsPrimary = true, SortOrder = 1 },
                new { Id = 12, ProductId = 10, ImageUrl = "https://www.apple.com/v/iphone-17-pro/d/images/overview/contrast/iphone_air__fe2gdmh5u5qy_large_2x.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== Samsung Galaxy S24 Ultra Images =====
                new { Id = 13, ProductId = 11, ImageUrl = "https://baotinmobile.vn/uploads/2024/02/s24-ultra-tim.jpg", IsPrimary = true, SortOrder = 1 },
                new { Id = 14, ProductId = 11, ImageUrl = "https://happyphone.vn/wp-content/uploads/2024/04/SAMSUNG-GALAXY-S24-ULTRA-12GB-512GB-Cam.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== Google Pixel 8 Pro Images =====
                new { Id = 15, ProductId = 12, ImageUrl = "https://www.didongmy.com/vnt_upload/product/10_2023/pixel8/thumbs/600_crop_google-pixel-8-pro-obsidian-thumb-didongmy-600x600.jpg", IsPrimary = true, SortOrder = 1 },
                new { Id = 16, ProductId = 12, ImageUrl = "https://cdn.tgdd.vn/Products/Images/42/307188/google-pixel-8-pro-600x600.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== Xiaomi 14 Images =====
                new { Id = 17, ProductId = 13, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/x/i/xiaomi-14_4.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 18, ProductId = 13, ImageUrl = "https://cdn.mobilecity.vn/mobilecity-vn/images/2023/10/xiaomi-14-hong.jpg.webp", IsPrimary = false, SortOrder = 2 },

                // ===== OnePlus 12 Images =====
                new { Id = 19, ProductId = 14, ImageUrl = "https://www.duchuymobile.com/images/detailed/65/oneplus-12-trang_ucuo-lm.jpg", IsPrimary = true, SortOrder = 1 },
                new { Id = 20, ProductId = 14, ImageUrl = "https://cdn2.cellphones.com.vn/x/media/catalog/product/o/n/oneplus-12_1_.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== iPad Pro M2 11 inch Images =====
                new { Id = 21, ProductId = 20, ImageUrl = "https://traidepbaniphone.com/upload/product/ipadpro11-inwi-fisilver2-upscreenusen-3047.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 22, ProductId = 20, ImageUrl = "https://phucanhcdn.com/media/product/49293_cellular_512gb.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== Galaxy Tab S9 Images =====
                new { Id = 23, ProductId = 21, ImageUrl = "https://product.hstatic.net/1000379731/product/mul3dutway0g35ul8rlp_bc0427d5a0594b43820baccffe69c71b.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 24, ProductId = 21, ImageUrl = "https://lh6.googleusercontent.com/proxy/mbdwj7VJ7KF0K0HYg2U_TBtAhgH4BBwl8w4ngxSsHSzI1psonPR0hpi1Jf7hFerv42m6zMkx5XEuXnhvggCUs5E8SiWyL7bjXC9f0iOa0i_vOotYHaCd71ywDccS", IsPrimary = false, SortOrder = 2 },

                // ===== Xiaomi Pad 6 Images =====
                new { Id = 25, ProductId = 22, ImageUrl = "https://phukienpico.com/wp-content/uploads/2023/09/Op-lung-bao-da-xiaomi-pad-6-pro-10.jpg", IsPrimary = true, SortOrder = 1 },
                new { Id = 26, ProductId = 22, ImageUrl = "https://cdn.tgdd.vn/Products/Images/522/309848/Kit/xiaomi-pad-6-note-1-1.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== Dell UltraSharp U2723DE Images =====
                new { Id = 27, ProductId = 30, ImageUrl = "https://product.hstatic.net/200000637319/product/1_dcfa8a17409f453cae523f6894013556_master.jpg", IsPrimary = true, SortOrder = 1 },

                // ===== LG UltraGear 27GN800 Images =====
                new { Id = 28, ProductId = 31, ImageUrl = "https://pcmarket.vn/media/lib/29-06-2022/27gn800-b4.jpg", IsPrimary = true, SortOrder = 1 },

                // ===== Logitech MX Keys Images =====
                
                new { Id = 29, ProductId = 40, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/a/gaming_8_14__1.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 30, ProductId = 40, ImageUrl = "https://product.hstatic.net/200000637319/product/mx-keys-mini-top-rose-us_a6b9661eb3424f1c8d79503e2cc3e0e7_master.png", IsPrimary = false, SortOrder = 2 },

                // ===== Razer BlackWidow V4 Pro Images =====
                new { Id = 31, ProductId = 41, ImageUrl = "https://product.hstatic.net/200000637319/product/81eeknarvil._ac_sl1500__b82e73d82da2451ca567fb128494d6aa_master.jpg", IsPrimary = true, SortOrder = 1 },

                // ===== Logitech MX Master 3S Images =====
                new { Id = 32, ProductId = 50, ImageUrl = "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/c/h/chuot-khong-day-logitech-mx-master-3s-for-mac_2.png", IsPrimary = true, SortOrder = 1 },
                new { Id = 33, ProductId = 50, ImageUrl = "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-3s/gallery/mx-master-3s-mouse-side-view-graphite.png?v=1", IsPrimary = false, SortOrder = 2 },

                // ===== Razer Viper V2 Pro Images =====
                new { Id = 34, ProductId = 51, ImageUrl = "https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/86/357719/chuot-sac-khong-day-gaming-razer-viper-v3-pro-thumb-638967440293053901-600x600.jpg", IsPrimary = true, SortOrder = 1 },

                // ===== Sony WH-1000XM5 Images =====
                new { Id = 35, ProductId = 60, ImageUrl = "https://www.sony.com.vn/image/5d02da5df552836db894cead8a68f5f3?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF", IsPrimary = true, SortOrder = 1 },
                new { Id = 36, ProductId = 60, ImageUrl = "https://cdn.tgdd.vn/Products/Images/54/313692/tai-nghe-bluetooth-chup-tai-sony-wh1000xm5-trang-1-750x500.jpg", IsPrimary = false, SortOrder = 2 },

                // ===== AirPods Pro 2 Images =====
                new { Id = 37, ProductId = 61, ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/MQD83?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1660803972361", IsPrimary = true, SortOrder = 1 },

                // ===== Anker 747 GaNPrime Images =====
                new { Id = 38, ProductId = 70, ImageUrl = "https://photo2.tinhte.vn/data/attachment-files/2022/07/6065972_anker-GaNPrime-tinhte-4.png", IsPrimary = true, SortOrder = 1 },

                // ===== Baseus Cable Images =====
                new { Id = 39, ProductId = 71, ImageUrl = "https://bizweb.dktcdn.net/thumb/large/100/462/529/products/0-1-1713511388939.jpg?v=1713511395120", IsPrimary = true, SortOrder = 1 },

                // ===== Spigen Rugged Armor Images =====
                new { Id = 40, ProductId = 80, ImageUrl = "https://m.media-amazon.com/images/I/81osE87mFrL.jpg", IsPrimary = true, SortOrder = 1 },

                // ===== Tomtoc Laptop Sleeve Images =====
                new { Id = 41, ProductId = 81, ImageUrl = "https://cdn2.cellphones.com.vn/x/media/catalog/product/t/o/tomtoc-slim-tui-chong-soc-1.png", IsPrimary = true, SortOrder = 1 }
            );
        }
    }
}
