using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blog_posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    FeaturedImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PublishedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LogoPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    IconPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png"),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_category_category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UsedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaxUsagePerUser = table.Column<int>(type: "integer", nullable: true),
                    MinPurchaseAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    MaxDiscountAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    TotalAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    CompletedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product_reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "integer", nullable: true),
                    HelpfulCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NotHelpfulCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsVerifiedPurchase = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    VariantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VariantId = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "blog_post_products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlogPostId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_post_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_blog_post_products_blog_posts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "blog_posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "blog_post_tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlogPostId = table.Column<int>(type: "integer", nullable: false),
                    Tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_post_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_blog_post_tags_blog_posts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "blog_posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "available"),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    Specs = table.Column<string>(type: "jsonb", nullable: true),
                    TotalSold = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_attribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InputType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "select"),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CategoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_attribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_attribute_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "coupon_categories",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_categories", x => new { x.CouponId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_coupon_categories_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coupon_products",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_products", x => new { x.CouponId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_coupon_products_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coupon_usages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_usages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_coupon_usages_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    VariantId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    GatewayResponse = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "review_helpfulness",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_helpfulness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_review_helpfulness_product_reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "product_reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fullname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    VariantId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cart_items_user_carts_CartId",
                        column: x => x.CartId,
                        principalTable: "user_carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_advertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "HomeTop"),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    StartDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_advertisements_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_image_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "available"),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    VariantSpecsJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_variant_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_value",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttributeId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute_value", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attribute_value_product_attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "product_attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RecipientName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "Vietnam"),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_addresses_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variant_attribute",
                columns: table => new
                {
                    VariantId = table.Column<int>(type: "integer", nullable: false),
                    AttributeId = table.Column<int>(type: "integer", nullable: false),
                    ValueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variant_attribute", x => new { x.VariantId, x.AttributeId });
                    table.ForeignKey(
                        name: "FK_product_variant_attribute_attribute_value_ValueId",
                        column: x => x.ValueId,
                        principalTable: "attribute_value",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_variant_attribute_product_attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "product_attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_variant_attribute_product_variant_VariantId",
                        column: x => x.VariantId,
                        principalTable: "product_variant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "blog_posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "FeaturedImageUrl", "PublishedAt", "Slug", "Status", "Summary", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 1, 1, "<h2>Thi?t k? cao c?p v?i khung Titanium</h2>\r\n<p>iPhone 15 Pro Max là chi?c iPhone ??u tiên s? d?ng khung Titanium thay vì thép không g?, giúp gi?m tr?ng l??ng ?áng k? nh?ng v?n ??m b?o ?? b?n cao. Thi?t k? vi?n m?ng h?n, c?m giác c?m n?m tho?i mái h?n so v?i th? h? tr??c.</p>\r\n\r\n<h2>Hi?u n?ng ??nh cao v?i A17 Pro</h2>\r\n<p>Chip A17 Pro ???c s?n xu?t trên ti?n trình 3nm mang l?i hi?u n?ng v??t tr?i, x? lý m?i tác v? m??t mà t? gaming ??n ch?nh s?a video 4K. GPU m?i h? tr? ray tracing, m? ra k? nguyên gaming mobile m?i.</p>\r\n\r\n<h2>Camera 48MP v?i zoom 5x</h2>\r\n<p>H? th?ng camera ba ?ng kính v?i camera chính 48MP, telephoto zoom quang 5x mang ??n ch?t l??ng ?nh tuy?t v?i trong m?i ?i?u ki?n ánh sáng. Ch? ?? ch?p ?êm Night Mode ???c c?i thi?n ?áng k?.</p>\r\n\r\n<h2>Pin trâu, s?c nhanh USB-C</h2>\r\n<p>Pin 4422mAh s? d?ng c? ngày dài, cu?i cùng Apple c?ng chuy?n sang c?ng USB-C theo chu?n châu Âu. H? tr? s?c nhanh 27W và s?c không dây MagSafe 15W.</p>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>iPhone 15 Pro Max x?ng ?áng là chi?c flagship ?áng mua nh?t n?m 2024 v?i thi?t k? cao c?p, hi?u n?ng m?nh m? và h? th?ng camera xu?t s?c. Giá 33,990,000 VND là h?p lý cho nh?ng gì Apple mang l?i.</p>", new DateTime(2025, 1, 9, 14, 30, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), "danh-gia-iphone-15-pro-max", "Published", "iPhone 15 Pro Max ?ánh d?u b??c ti?n m?i v?i chip A17 Pro, khung Titanium siêu b?n và camera 48MP v?i zoom quang 5x. Cùng khám phá chi ti?t flagship m?i nh?t t? Apple.", "?ánh giá chi ti?t iPhone 15 Pro Max: ??nh cao công ngh? t? Apple", new DateTime(2025, 1, 10, 8, 45, 0, 0, DateTimeKind.Utc), 1250 },
                    { 2, 2, "<h2>Thi?t k? và ch?t l??ng build</h2>\r\n<p>MacBook Air M2 v?i thi?t k? vuông v?n hi?n ??i, ?? m?ng ch? 11.3mm và tr?ng l??ng 1.24kg. Dell XPS 13 không kém c?nh v?i thi?t k? siêu m?ng 13.9mm, vi?n màn hình InfinityEdge ?n t??ng.</p>\r\n\r\n<h2>Hi?u n?ng và pin</h2>\r\n<p>Chip M2 8-core mang l?i hi?u n?ng v??t tr?i, ??c bi?t v?i các ?ng d?ng macOS native. Pin s? d?ng lên ??n 18 gi?. Dell XPS 13 v?i Intel Core i5/i7 th? h? 13 m?nh m? cho Windows, pin kho?ng 12 gi?.</p>\r\n\r\n<h2>Màn hình</h2>\r\n<p>MacBook Air M2: 13.6 inch Liquid Retina (2560x1664), ?? sáng 500 nits. Dell XPS 13: 13.4 inch FHD+ (1920x1200), màn hình InfinityEdge vi?n siêu m?ng.</p>\r\n\r\n<h2>Giá c?</h2>\r\n<p>MacBook Air M2 8GB/256GB: 28,990,000 VND. Dell XPS 13 i5/8GB/512GB: 25,990,000 VND. Dell có giá t?t h?n v?i SSD 512GB ngay t? ??u.</p>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>Ch?n MacBook Air M2 n?u b?n ?u tiên pin trâu, h? sinh thái Apple. Ch?n Dell XPS 13 n?u c?n Windows, giá t?t h?n và thi?t k? ??p m?t.</p>", new DateTime(2025, 1, 11, 15, 20, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", new DateTime(2025, 1, 12, 10, 30, 0, 0, DateTimeKind.Utc), "so-sanh-macbook-air-m2-vs-dell-xps-13", "Published", "Hai chi?c laptop cao c?p ???c yêu thích nh?t hi?n nay. MacBook Air M2 v?i hi?u n?ng chip ARM xu?t s?c hay Dell XPS 13 v?i thi?t k? tinh t? và Windows 11? Cùng phân tích chi ti?t.", "So sánh MacBook Air M2 vs Dell XPS 13: Nên ch?n laptop nào?", new DateTime(2025, 1, 12, 10, 15, 0, 0, DateTimeKind.Utc), 890 },
                    { 3, 1, "<h2>Thi?t k? sang tr?ng, S Pen tích h?p</h2>\r\n<p>Thi?t k? vuông v?n m?nh m?, khung nhôm cao c?p. S Pen tích h?p trong thân máy mang l?i tr?i nghi?m ghi chú, v? tuy?t v?i - tính n?ng ??c quy?n c?a dòng Ultra.</p>\r\n\r\n<h2>Màn hình ??nh cao</h2>\r\n<p>Màn hình 6.8 inch Dynamic AMOLED 2X v?i ?? phân gi?i QHD+ (3120x1440), t?n s? quét 120Hz, ?? sáng ??nh 2600 nits - rõ nét ngay c? d??i n?ng g?t.</p>\r\n\r\n<h2>Camera 200MP chuyên nghi?p</h2>\r\n<p>Camera chính 200MP v?i OIS, camera telephoto kép (3x và 5x), camera ultra wide 12MP. Ch?t l??ng ?nh xu?t s?c, zoom 10x v?n gi? chi ti?t t?t.</p>\r\n\r\n<h2>Hi?u n?ng m?nh m? Snapdragon 8 Gen 3</h2>\r\n<p>Chip Snapdragon 8 Gen 3 for Galaxy t?i ?u riêng cho Samsung, hi?u n?ng v??t tr?i, ch?i game m??t mà, pin 5000mAh s? d?ng tho?i mái c? ngày.</p>\r\n\r\n<h2>?ánh giá</h2>\r\n<p>Galaxy S24 Ultra là l?a ch?n hàng ??u cho ai mu?n flagship Android hoàn h?o nh?t. Giá 29,990,000 VND x?ng ?áng v?i nh?ng gì Samsung mang l?i.</p>", new DateTime(2025, 1, 12, 16, 45, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", new DateTime(2025, 1, 13, 11, 0, 0, 0, DateTimeKind.Utc), "samsung-galaxy-s24-ultra-review", "Published", "Galaxy S24 Ultra ti?p t?c kh?ng ??nh v? th? d?n ??u phân khúc flagship Android v?i camera 200MP, S Pen tích h?p và màn hình Dynamic AMOLED 2X tuy?t ??p.", "Samsung Galaxy S24 Ultra: Android flagship t?t nh?t v?i camera 200MP", new DateTime(2025, 1, 13, 10, 30, 0, 0, DateTimeKind.Utc), 1100 },
                    { 4, 2, "<h2>1. Razer BlackWidow V4 Pro - Bàn phím c? gaming cao c?p</h2>\r\n<p>Bàn phím c? v?i switch Razer Green Clicky, ?èn RGB Chroma per-key, Command Dial ti?n l?i. Giá 5,990,000 VND cho tr?i nghi?m gõ tuy?t v?i và ?? b?n cao.</p>\r\n\r\n<h2>2. Razer Viper V2 Pro - Chu?t gaming không dây siêu nh?</h2>\r\n<p>Ch? 58g nh?ng ??y ?? tính n?ng: sensor Focus Pro 30K DPI, optical switch Gen 3, pin 80 gi?. Thi?t k? ambidextrous phù h?p m?i ki?u c?m. Giá 3,990,000 VND.</p>\r\n\r\n<h2>3. LG UltraGear 27GN800 - Màn hình gaming QHD 144Hz</h2>\r\n<p>27 inch QHD (2560x1440) v?i t?n s? quét 144Hz, t?m n?n IPS Nano Color, h? tr? G-Sync. Giá 8,490,000 VND cho hình ?nh m??t mà, màu s?c chính xác.</p>\r\n\r\n<h2>4. Sony WH-1000XM5 - Tai nghe ch?ng ?n t?t nh?t</h2>\r\n<p>Tuy không ph?i tai nghe gaming chuyên d?ng nh?ng ch?t âm Hi-Res, ANC xu?t s?c phù h?p cho game single-player immersive. Giá 8,990,000 VND.</p>\r\n\r\n<h2>5. Anker 747 GaNPrime 150W - S?c nhanh ?a n?ng</h2>\r\n<p>S?c laptop gaming, ?i?n tho?i, tai nghe cùng lúc v?i 4 c?ng (3 USB-C + 1 USB-A). Công ngh? GaN nh? g?n, công su?t 150W. Giá 2,490,000 VND.</p>\r\n\r\n<h2>T?ng k?t</h2>\r\n<p>??u t? vào ph? ki?n ch?t l??ng s? nâng cao tr?i nghi?m gaming ?áng k?. T?ng chi phí setup hoàn h?o kho?ng 30 tri?u VND.</p>", new DateTime(2025, 1, 13, 17, 0, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", new DateTime(2025, 1, 14, 14, 0, 0, 0, DateTimeKind.Utc), "top-5-phu-kien-gaming-tot-nhat-2025", "Published", "T? bàn phím c? ??n chu?t gaming, tai nghe và màn hình - nh?ng ph? ki?n gaming này s? nâng t?m tr?i nghi?m ch?i game c?a b?n lên m?t level hoàn toàn m?i.", "Top 5 ph? ki?n gaming t?t nh?t cho setup chuyên nghi?p 2025", new DateTime(2025, 1, 14, 13, 45, 0, 0, DateTimeKind.Utc), 750 },
                    { 5, 1, "<h2>Xiaomi Pad 6 - Giá tr? t?t nh?t</h2>\r\n<p>Màn hình 11 inch LCD 2.8K v?i t?n s? quét 144Hz, chip Snapdragon 870 v?n m?nh m? cho ?a nhi?m và ch?i game. Pin 8840mAh s? d?ng c? ngày. Giá ch? 8,990,000 VND.</p>\r\n\r\n<h2>iPad Air M2 - Hi?u n?ng cao c?p</h2>\r\n<p>Chip M2 m?nh m? nh? MacBook, màn hình Liquid Retina 11 inch, h? tr? Apple Pencil 2. H? sinh thái iPadOS v?i hàng tri?u app t?i ?u. Giá 16,990,000 VND.</p>\r\n\r\n<h2>So sánh chi ti?t</h2>\r\n<ul>\r\n<li><strong>Hi?u n?ng:</strong> M2 m?nh h?n rõ r?t, nh?ng Snapdragon 870 ?? dùng</li>\r\n<li><strong>Màn hình:</strong> Xiaomi có t?n s? quét 144Hz, iPad có màu s?c chính xác h?n</li>\r\n<li><strong>H? ?i?u hành:</strong> iPadOS nhi?u app h?n, MIUI Pad tùy bi?n cao</li>\r\n<li><strong>Giá:</strong> Xiaomi r? h?n g?n m?t n?a</li>\r\n</ul>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>Ch?n Xiaomi Pad 6 n?u ngân sách h?n ch?, dùng ?? xem phim, ??c sách, ch?i game nh?. Ch?n iPad Air M2 n?u c?n hi?u n?ng cao cho công vi?c, v? digital art ho?c mu?n h? sinh thái Apple.</p>", new DateTime(2025, 1, 14, 18, 30, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "tablet-gia-re-dang-mua-2025", "Published", "So sánh hai chi?c tablet t?m trung hot nh?t: Xiaomi Pad 6 giá ch? 8,990,000 VND và iPad Air M2 giá 16,990,000 VND. ?âu là l?a ch?n phù h?p v?i b?n?", "Tablet giá r? ?áng mua 2025: Xiaomi Pad 6 vs iPad Air M2", new DateTime(2025, 1, 15, 8, 45, 0, 0, DateTimeKind.Utc), 520 }
                });

            migrationBuilder.InsertData(
                table: "blog_posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "FeaturedImageUrl", "PublishedAt", "Slug", "Status", "Summary", "Title", "UpdatedAt" },
                values: new object[] { 6, 2, "<h2>Nhu c?u theo ngành h?c</h2>\r\n<p>N?i dung ?ang ???c c?p nh?t...</p>", new DateTime(2025, 1, 15, 9, 15, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "bi-quyet-chon-laptop-cho-sinh-vien", "Draft", "H??ng d?n chi ti?t giúp sinh viên ch?n laptop phù h?p v?i ngành h?c và ngân sách. T? sinh viên v?n phòng ??n k? thu?t, thi?t k? - ??u có g?i ý c? th?.", "Bí quy?t ch?n laptop cho sinh viên: C?u hình nào là ???", new DateTime(2025, 1, 15, 9, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "brand",
                columns: new[] { "Id", "Country", "CreatedAt", "LogoPath", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/dell-2.svg", "Dell", "dell", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 2, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/apple-13.svg", "Apple", "apple", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 3, "Taiwan", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/asus-4.svg", "Asus", "asus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 4, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/HP-5.svg", "HP", "hp", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 5, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/lenovo-2.svg", "Lenovo", "lenovo", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 6, "Korea", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/samsung-8.svg", "Samsung", "samsung", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 7, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/google-1.svg", "Google", "google", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 8, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/xiaomi-1.svg", "Xiaomi", "xiaomi", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 9, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/oneplus-2.svg", "OnePlus", "oneplus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 10, "Korea", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/lg.svg", "LG", "lg", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 11, "Switzerland", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/logitech-gaming-2.svg", "Logitech", "logitech", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 12, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/razer-1.svg", "Razer", "razer", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 13, "Japan", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/sony-2svg", "Sony", "sony", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 14, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://cdn.worldvectorlogo.com/logos/anker-logo-1.svg", "Anker", "anker", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 15, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://mms.img.susercontent.com/vn-11134216-7r98o-lnicyi57m5x6fd", "Baseus", "baseus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 16, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://spigen.vn/wp-content/uploads/2023/09/Spigen_Header_New_Logo.png", "Spigen", "spigen", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 17, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlpSaYkZMxWktmvmvOx7mDurTEDu0KXqz1HQ&s", "UAG", "uag", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "IconPath", "ImageUrl", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of laptops", "https://www.svgrepo.com/show/525970/laptop.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/laptop_jchkjn.webp", "Laptop", null, "laptop", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of smartphones", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/mobile_qk5kuf.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/iphone_air-3_2_hfq1wl.webp", "Smartphone", null, "smartphone", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of tablets", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/tablet_mhhzhn.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337716/xiaomi-pad-mini-4_adg1r9.webp", "Tablet", null, "tablet", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[] { 4, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "External product that enhances main product experience", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp", "Accessory", null, "accessory", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "IconPath", "ImageUrl", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 5, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of cameras", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/camera_xmozh9.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/may-anh-canon-eos-r100_8__havbm2.webp", "Camera", null, "camera", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "PC and office related products", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/devices_kty5xc.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/pc_eoswm6.jpg", "Computer & Office", null, "computer-office", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Gaming products and accessories", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/game_opdnni.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ps5_lfmig6.webp", "Gaming", null, "gaming", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "EndDate", "MaxDiscountAmount", "MaxUsagePerUser", "MinPurchaseAmount", "Name", "Note", "Quantity", "StartDate", "Status", "Type", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "PHVC000001", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Holiday", null, 10, new DateTime(2023, 8, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Active", "FixedAmount", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 300000m },
                    { 2, "PHVC000002", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Summer", null, 10, new DateTime(2025, 8, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Active", "FixedAmount", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 300000m },
                    { 3, "PHVC000003", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Return", null, 10, new DateTime(2025, 8, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Active", "FixedAmount", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 300000m }
                });

            migrationBuilder.InsertData(
                table: "product_attribute",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "InputType", "IsGlobal", "Name" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", true, "Color" },
                    { 2, null, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", true, "Warranty Period" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "user_carts",
                columns: new[] { "Id", "CreatedAt", "UpdatedAt", "UserId" },
                values: new object[] { 1, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 14, 15, 20, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "wishlists",
                columns: new[] { "Id", "AddedAt", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), 2, 2 },
                    { 2, new DateTime(2025, 1, 11, 14, 30, 0, 0, DateTimeKind.Utc), 2, 27 },
                    { 3, new DateTime(2025, 1, 12, 16, 45, 0, 0, DateTimeKind.Utc), 2, 31 }
                });

            migrationBuilder.InsertData(
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 1, 1, "Black" },
                    { 2, 1, "White" },
                    { 3, 1, "Silver" },
                    { 4, 1, "Blue" },
                    { 5, 1, "Red" },
                    { 6, 2, "12" },
                    { 7, 2, "24" },
                    { 8, 2, "36" }
                });

            migrationBuilder.InsertData(
                table: "blog_post_products",
                columns: new[] { "Id", "BlogPostId", "ProductId" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 2, 1, 61 },
                    { 3, 1, 80 },
                    { 4, 2, 2 },
                    { 5, 2, 1 },
                    { 6, 2, 81 },
                    { 7, 3, 11 },
                    { 8, 3, 70 },
                    { 9, 3, 80 },
                    { 10, 4, 41 },
                    { 11, 4, 51 },
                    { 12, 4, 31 },
                    { 13, 4, 60 },
                    { 14, 4, 70 },
                    { 15, 5, 22 },
                    { 16, 5, 20 },
                    { 17, 5, 21 }
                });

            migrationBuilder.InsertData(
                table: "blog_post_tags",
                columns: new[] { "Id", "BlogPostId", "Tag" },
                values: new object[,]
                {
                    { 1, 1, "review" },
                    { 2, 1, "iphone" },
                    { 3, 1, "apple" },
                    { 4, 1, "flagship" },
                    { 5, 1, "smartphone" },
                    { 6, 2, "comparison" },
                    { 7, 2, "laptop" },
                    { 8, 2, "macbook" },
                    { 9, 2, "dell" },
                    { 10, 3, "review" },
                    { 11, 3, "samsung" },
                    { 12, 3, "android" },
                    { 13, 3, "flagship" },
                    { 14, 3, "camera" },
                    { 15, 4, "gaming" },
                    { 16, 4, "peripherals" },
                    { 17, 4, "accessories" },
                    { 18, 4, "top-list" },
                    { 19, 5, "comparison" },
                    { 20, 5, "tablet" },
                    { 21, 5, "budget" },
                    { 22, 5, "xiaomi" },
                    { 23, 5, "ipad" },
                    { 24, 6, "guide" },
                    { 25, 6, "student" },
                    { 26, 6, "laptop" }
                });

            migrationBuilder.InsertData(
                table: "cart_items",
                columns: new[] { "Id", "AddedAt", "CartId", "Price", "Quantity", "VariantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Utc), 1, 33990000m, 1, 11 },
                    { 2, new DateTime(2025, 1, 13, 14, 15, 0, 0, DateTimeKind.Utc), 1, 6490000m, 1, 42 },
                    { 3, new DateTime(2025, 1, 14, 15, 20, 0, 0, DateTimeKind.Utc), 1, 490000m, 1, 46 }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "IconPath", "ImageUrl", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All types of computer monitors", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/monitor_gfheqk.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/monitor_i9d6or.webp", "Monitor", 6, "monitor", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Processors and chips for computers", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/CPU_kfg2fy.webp", "CPU", 6, "cpu", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Graphics cards for PCs and laptops", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/GPU_ltzw4j.webp", "GPU", 6, "gpu", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Memory modules for PCs and laptops", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ram_kssmbv.webp", "RAM", 6, "ram", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Storage devices: SSD, HDD, memory cards", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337715/rom_sreazq.webp", "Storage (SSD / HDD)", 6, "storage", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Keyboards for PC, Laptop, and Tablet", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/banphim_iai2rn.jpg", "Keyboard", 4, "keyboard", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Computer and laptop mice (wired, wireless, gaming)", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/mouse_enodsx.png", "Mouse", 4, "mouse", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Audio accessories compatible with PC, Laptop, and Smartphone", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336342/headphone_pz0fkb.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/headphone_qn6mqd.jpg", "Headphone / Headset", 4, "headphone", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Chargers, adapters, and data cables for all devices", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp", "Charger & Cable", 4, "charger-cable", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Protective cases for phones, tablets, and laptops", null, "Case & Cover", 4, "case-cover", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product",
                columns: new[] { "Id", "BasePrice", "BrandId", "CategoryId", "CreatedAt", "Description", "DiscountPercent", "Name", "Overview", "Slug", "Specs", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 25990000m, 1, 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Dell XPS 13 với màn hình InfinityEdge, Intel Core thế hệ mới, vỏ nhôm nguyên khối cao cấp.", null, "Dell XPS 13", "Laptop cao cấp thiết kế siêu mỏng, hiệu suất mạnh mẽ cho doanh nhân.", "dell-xps-13", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.4 inch FHD+ (1920x1200)\",\"InfinityEdge\",\"Chống chói\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1340P\",\"Intel Core i7-1360P\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR5\",\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.24 kg\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"52Wh - Sử dụng 12 giờ\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]},\r\n    {\"Name\":\"Bảo hành\",\"Value\":[\"12 tháng chính hãng\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 28990000m, 2, 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "MacBook Air M2 13.6 inch, màn hình Liquid Retina, hiệu năng vượt trội, pin 18 giờ.", null, "MacBook Air M2", "MacBook Air với chip M2 mạnh mẽ, pin trâu, thiết kế sang trọng.", "macbook-air-m2", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.6 inch Liquid Retina\",\"2560 x 1664 pixels\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2 8-core CPU\",\"10-core GPU\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB Unified Memory\",\"16GB Unified Memory\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"256GB SSD\",\"512GB SSD\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"52.6Wh - Lên đến 18 giờ\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"macOS Sonoma\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.24 kg\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 22490000m, 3, 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Asus ZenBook 14 OLED màn hình 2.8K, Intel Core i5/i7 thế hệ mới, mỏng nhẹ 1.39kg.", null, "Asus ZenBook 14 OLED", "Ultrabook với màn hình OLED tuyệt đẹp, hiệu năng cao.", "asus-zenbook-14-oled", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"14 inch OLED 2.8K (2880x1800)\",\"90Hz\",\"400 nits\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-13500H\",\"Intel Core i7-13700H\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR5\",\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD PCIe 4.0\",\"1TB SSD PCIe 4.0\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.39 kg\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 42990000m, 4, 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "HP Spectre x360 14 màn hình OLED cảm ứng, Intel Core i7, hỗ trợ bút stylus.", null, "HP Spectre x360 14", "Laptop xoay 360° cao cấp, màn hình cảm ứng, thiết kế sang trọng.", "hp-spectre-x360-14", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.5 inch OLED cảm ứng\",\"3:2 (3000x2000)\",\"400 nits\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1335U\",\"Intel Core i7-1355U\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR4x\",\"16GB LPDDR4x\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Xoay 360°\",\"Value\":[\"Có - Chế độ Tablet\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 48990000m, 5, 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "ThinkPad X1 Carbon Gen 11 với màn hình 2.8K, Intel Core thế hệ 13, vỏ carbon siêu nhẹ.", null, "Lenovo ThinkPad X1 Carbon Gen 11", "Laptop doanh nghiệp cao cấp, bền bỉ, bảo mật tốt.", "thinkpad-x1-carbon-gen11", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"14 inch 2.8K (2880x1800)\",\"Low Blue Light\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1335U\",\"Intel Core i7-1355U\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Bảo mật\",\"Value\":[\"Vân tay\",\"TPM 2.0\",\"Webcam Privacy Shutter\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Pro\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 33990000m, 2, 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "iPhone 15 Pro Max 256GB với Dynamic Island, Always-On Display, camera zoom 5x.", null, "iPhone 15 Pro Max", "iPhone cao cấp nhất với khung Titanium, camera 48MP, chip A17 Pro.", "iphone-15-pro-max", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.7 inch Super Retina XDR OLED\",\"ProMotion 120Hz\",\"Always-On\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple A17 Pro 3nm\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"48MP Main\",\"12MP Ultra Wide\",\"12MP Telephoto 5x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"4422mAh\",\"29 giờ video\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"iOS 17\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 29990000m, 6, 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Galaxy S24 Ultra 256GB Snapdragon 8 Gen 3, màn hình 6.8 inch, pin 5000mAh.", null, "Samsung Galaxy S24 Ultra", "Android flagship với S Pen, camera 200MP, màn hình Dynamic AMOLED 2X.", "samsung-galaxy-s24-ultra", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.8 inch Dynamic AMOLED 2X\",\"QHD+ (3120x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3 for Galaxy\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"200MP chính\",\"12MP Ultra Wide\",\"50MP Periscope 5x\",\"10MP Telephoto 3x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5000mAh - Sạc nhanh 45W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 14 - One UI 6.1\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 24990000m, 7, 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Pixel 8 Pro 256GB Tensor G3, camera AI ma thuật, màn hình LTPO OLED 120Hz.", null, "Google Pixel 8 Pro", "Điện thoại AI với camera thông minh nhất, cập nhật 7 năm.", "google-pixel-8-pro", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.7 inch LTPO OLED\",\"QHD+ (3120x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Google Tensor G3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB LPDDR5X\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP Main\",\"48MP Ultra Wide\",\"48MP Telephoto 5x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5050mAh - Sạc nhanh 30W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 14 - Cập nhật 7 năm\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 18990000m, 8, 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Xiaomi 14 Snapdragon 8 Gen 3, màn hình AMOLED 120Hz, camera Leica Summilux.", null, "Xiaomi 14", "Flagship Xiaomi với camera Leica, hiệu năng mạnh mẽ, giá tốt.", "xiaomi-14", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.36 inch AMOLED\",\"FHD+ (2670x1200)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP Leica Main\",\"50MP Ultra Wide\",\"50MP Telephoto 3.2x\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"HyperOS (Android 14)\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 19990000m, 9, 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "OnePlus 12 Snapdragon 8 Gen 3, màn hình 120Hz, pin 5400mAh sạc siêu nhanh.", null, "OnePlus 12", "Flagship killer với sạc nhanh 100W, màn hình LTPO AMOLED.", "oneplus-12", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.82 inch LTPO AMOLED\",\"QHD+ (3168x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\",\"16GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5400mAh - Sạc nhanh 100W SuperVOOC\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"OxygenOS 14 (Android 14)\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 24990000m, 2, 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "iPad Pro 11 inch M2 với ProMotion 120Hz, hỗ trợ Apple Pencil 2 và Magic Keyboard.", null, "iPad Pro M2 11 inch", "Máy tính bảng mạnh nhất với chip M2, màn hình Liquid Retina XDR.", "ipad-pro-m2-11", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch Liquid Retina\",\"IPS LCD (2388x1668)\",\"ProMotion 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2 8-core CPU\",\"10-core GPU\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"iPadOS 17\"]},\r\n    {\"Name\":\"Phụ kiện\",\"Value\":[\"Apple Pencil 2\",\"Magic Keyboard\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 19990000m, 6, 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Galaxy Tab S9 11 inch Snapdragon 8 Gen 2, kháng nước IP68, S Pen kèm theo.", null, "Samsung Galaxy Tab S9", "Tablet Android cao cấp với màn hình Dynamic AMOLED 2X, S Pen.", "samsung-galaxy-tab-s9", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch Dynamic AMOLED 2X\",\"WQXGA (2560x1600)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 2\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 13 - One UI 5.1\"]},\r\n    {\"Name\":\"Kháng nước\",\"Value\":[\"IP68\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 8990000m, 8, 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Xiaomi Pad 6 11 inch Snapdragon 870, màn hình LCD 144Hz, pin 8840mAh.", null, "Xiaomi Pad 6", "Tablet tầm trung với màn hình 144Hz, loa 4 kênh Dolby Atmos.", "xiaomi-pad-6", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch LCD\",\"2.8K (2880x1800)\",\"144Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 870\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"6GB\",\"8GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"8840mAh - Sạc nhanh 33W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"MIUI Pad 14\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_attribute",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "InputType", "Name" },
                values: new object[,]
                {
                    { 10, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Screen Size" },
                    { 11, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "CPU" },
                    { 12, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "RAM" },
                    { 13, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Storage" },
                    { 14, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "GPU" },
                    { 20, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Screen Size" },
                    { 21, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", "Battery Capacity" },
                    { 22, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Camera Resolution" },
                    { 23, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "RAM" },
                    { 24, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Storage" },
                    { 30, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Screen Size" },
                    { 31, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", "Battery Capacity" },
                    { 32, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "RAM" },
                    { 33, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Storage" },
                    { 40, 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Processor Type" },
                    { 41, 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "RAM Type" },
                    { 42, 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "GPU Model" },
                    { 60, 4, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Compatibility" },
                    { 61, 4, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Connection Type" }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "CreatedAt", "Email", "Fullname", "PasswordHash", "PhoneNumber", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@tekno.com", "Admin User", "$2a$11$W/ZYaZwxFhbSWpJNtPMAfetjQIsqJ1rYdiP2GQoF1.Hr7aqFmtaya", "0901234567", 1, null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer@tekno.com", "Customer User", "$2a$11$ZKxnFd0g1qcrtOgFJrbYiOOnKrtsA6flk4msMC0Uf/qcmqYzoUlSq", "0912345678", 2, null }
                });

            migrationBuilder.InsertData(
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 10, 10, "13 inch" },
                    { 11, 10, "15 inch" },
                    { 12, 10, "17 inch" },
                    { 13, 11, "Intel i5" },
                    { 14, 11, "Intel i7" },
                    { 15, 11, "AMD Ryzen 5" },
                    { 16, 11, "AMD Ryzen 7" },
                    { 17, 12, "8GB" },
                    { 18, 12, "16GB" },
                    { 19, 12, "32GB" },
                    { 20, 13, "256GB SSD" },
                    { 21, 13, "512GB SSD" },
                    { 22, 13, "1TB SSD" },
                    { 23, 14, "RTX 4060" },
                    { 24, 14, "RTX 4070" },
                    { 25, 14, "GTX 1650" },
                    { 30, 20, "5.5 inch" },
                    { 31, 20, "6.1 inch" },
                    { 32, 20, "6.7 inch" },
                    { 33, 21, "3000" },
                    { 34, 21, "4000" },
                    { 35, 21, "5000" },
                    { 36, 22, "12MP" },
                    { 37, 22, "48MP" },
                    { 38, 22, "108MP" },
                    { 39, 23, "4GB" },
                    { 40, 23, "6GB" },
                    { 41, 23, "8GB" },
                    { 42, 24, "64GB" },
                    { 43, 24, "128GB" },
                    { 44, 24, "256GB" },
                    { 60, 60, "USB" },
                    { 61, 60, "USB-C" },
                    { 62, 61, "Wired" },
                    { 63, 61, "Wireless" }
                });

            migrationBuilder.InsertData(
                table: "product",
                columns: new[] { "Id", "BasePrice", "BrandId", "CategoryId", "CreatedAt", "Description", "DiscountPercent", "Name", "Overview", "Slug", "Specs", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 30, 17990000m, 1, 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Dell U2723DE 27 inch QHD IPS Black, độ chính xác màu cao cho thiết kế đồ họa.", null, "Dell UltraSharp U2723DE", "Màn hình 27 inch QHD IPS, độ phủ màu 95% DCI-P3, USB-C 90W.", "dell-ultrasharp-u2723de", "[\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"27 inch\"]},\r\n    {\"Name\":\"Độ phân giải\",\"Value\":[\"QHD 2560x1440\"]},\r\n    {\"Name\":\"Tấm nền\",\"Value\":[\"IPS Black\"]},\r\n    {\"Name\":\"Tần số quét\",\"Value\":[\"60Hz\"]},\r\n    {\"Name\":\"Cổng kết nối\",\"Value\":[\"HDMI 2.0\",\"DisplayPort 1.4\",\"USB-C 90W\"]},\r\n    {\"Name\":\"Độ phủ màu\",\"Value\":[\"95% DCI-P3\",\"100% sRGB\"]},\r\n    {\"Name\":\"Bảo hành\",\"Value\":[\"36 tháng\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, 8490000m, 10, 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "LG 27GN800 27 inch QHD IPS 144Hz, thời gian phản hồi 1ms, HDR10.", null, "LG UltraGear 27GN800-B", "Màn hình gaming 27 inch QHD 144Hz, tấm nền IPS Nano, G-Sync.", "lg-ultragear-27gn800", "[\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"27 inch\"]},\r\n    {\"Name\":\"Độ phân giải\",\"Value\":[\"QHD 2560x1440\"]},\r\n    {\"Name\":\"Tấm nền\",\"Value\":[\"IPS Nano Color\"]},\r\n    {\"Name\":\"Tần số quét\",\"Value\":[\"144Hz\"]},\r\n    {\"Name\":\"Cổng kết nối\",\"Value\":[\"HDMI 2.0 x2\",\"DisplayPort 1.4\"]},\r\n    {\"Name\":\"Đồng bộ\",\"Value\":[\"G-Sync Compatible\",\"FreeSync Premium\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, 2990000m, 11, 13, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Logitech MX Keys với phím Perfect Stroke, kết nối multi-device, pin 10 ngày.", null, "Logitech MX Keys", "Bàn phím không dây cao cấp cho văn phòng, đèn nền thông minh.", "logitech-mx-keys", "[\r\n    {\"Name\":\"Kiểu kết nối\",\"Value\":[\"Bluetooth\",\"USB Receiver\"]},\r\n    {\"Name\":\"Layout\",\"Value\":[\"Full-size\"]},\r\n    {\"Name\":\"Đèn nền\",\"Value\":[\"Có - Tự động điều chỉnh\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"Sạc USB-C - 10 ngày với đèn\"]},\r\n    {\"Name\":\"Multi-device\",\"Value\":[\"3 thiết bị\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 41, 5990000m, 12, 13, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Razer BlackWidow V4 Pro với Command Dial, switch cơ học, RGB per-key.", null, "Razer BlackWidow V4 Pro", "Bàn phím cơ gaming cao cấp RGB Chroma, switch Green Clicky.", "razer-blackwidow-v4-pro", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"Cơ học\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Razer Green Clicky\"]},\r\n    {\"Name\":\"Đèn nền\",\"Value\":[\"RGB Chroma per-key\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"USB Type-C có dây\"]},\r\n    {\"Name\":\"Tính năng\",\"Value\":[\"Command Dial\",\"Media keys\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 50, 2490000m, 11, 14, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Logitech MX Master 3S với MagSpeed Wheel, kết nối multi-device, pin 70 ngày.", null, "Logitech MX Master 3S", "Chuột không dây ergonomic cho năng suất cao, sensor 8K DPI.", "logitech-mx-master-3s", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Darkfield 8000 DPI\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"Bluetooth\",\"USB Receiver\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"70 ngày\"]},\r\n    {\"Name\":\"Số nút\",\"Value\":[\"7 nút có thể tùy chỉnh\"]},\r\n    {\"Name\":\"Multi-device\",\"Value\":[\"3 thiết bị\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 51, 3990000m, 12, 14, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Razer Viper V2 Pro với HyperSpeed Wireless, optical switch Gen 3, pin 80 giờ.", null, "Razer Viper V2 Pro", "Chuột gaming không dây siêu nhẹ 58g, sensor Focus Pro 30K.", "razer-viper-v2-pro", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 60, 8990000m, 13, 15, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Sony WH-1000XM5 với ANC thế hệ mới, 8 micro, hỗ trợ LDAC và DSEE Extreme.", null, "Sony WH-1000XM5", "Tai nghe chống ồn hàng đầu, chất âm Hi-Res, pin 30 giờ.", "sony-wh-1000xm5", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"Over-ear\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Có - 8 micro\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"30 giờ\"]},\r\n    {\"Name\":\"Sạc\",\"Value\":[\"USB-C - Sạc nhanh 3 phút = 3 giờ\"]},\r\n    {\"Name\":\"Codec\",\"Value\":[\"LDAC\",\"AAC\",\"SBC\"]},\r\n    {\"Name\":\"Mic\",\"Value\":[\"Có - AI Noise Reduction\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 61, 6490000m, 2, 15, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "AirPods Pro 2 với Adaptive Audio, Transparency Mode, case sạc MagSafe và loa tìm kiếm.", null, "Apple AirPods Pro 2", "Tai nghe true wireless với ANC tốt nhất, chip H2, sạc MagSafe.", "apple-airpods-pro-2", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"In-ear True Wireless\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple H2\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Có - Adaptive\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"6 giờ tai nghe\",\"30 giờ với case\"]},\r\n    {\"Name\":\"Sạc\",\"Value\":[\"MagSafe\",\"USB-C\",\"Apple Watch charger\"]},\r\n    {\"Name\":\"Kháng nước\",\"Value\":[\"IPX4\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 70, 2490000m, 14, 16, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Anker 747 Charger với công nghệ GaN, ActiveShield 2.0, sạc laptop, điện thoại đồng thời.", null, "Anker 747 GaNPrime 150W", "Sạc nhanh GaN 150W, 4 cổng (3 USB-C + 1 USB-A), sạc cùng lúc 4 thiết bị.", "anker-747-ganprime-150w", "[\r\n    {\"Name\":\"Công suất\",\"Value\":[\"150W tối đa\"]},\r\n    {\"Name\":\"Cổng\",\"Value\":[\"3x USB-C (100W+45W+45W)\",\"1x USB-A (22.5W)\"]},\r\n    {\"Name\":\"Công nghệ\",\"Value\":[\"GaN III\",\"ActiveShield 2.0\"]},\r\n    {\"Name\":\"Điện áp\",\"Value\":[\"100-240V\"]},\r\n    {\"Name\":\"Bảo vệ\",\"Value\":[\"Quá nhiệt\",\"Quá dòng\",\"Ngắn mạch\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 71, 290000m, 15, 16, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Baseus 100W Cable với E-Marker chip, hỗ trợ PD 3.0, QC 4.0, data transfer 480Mbps.", null, "Baseus 100W USB-C Cable 2m", "Cáp sạc nhanh USB-C to USB-C 100W, dây bện nylon bền, dài 2m.", "baseus-100w-usb-c-cable", "[\r\n    {\"Name\":\"Chiều dài\",\"Value\":[\"2 mét\"]},\r\n    {\"Name\":\"Đầu nối\",\"Value\":[\"USB-C to USB-C\"]},\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"Nylon braided + Zinc alloy\"]},\r\n    {\"Name\":\"Công suất\",\"Value\":[\"100W (20V/5A)\"]},\r\n    {\"Name\":\"Tốc độ dữ liệu\",\"Value\":[\"480Mbps\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 80, 490000m, 16, 17, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Spigen Rugged Armor với vật liệu TPU mềm, viền chống trầy, tản nhiệt tốt.", null, "Spigen Rugged Armor iPhone 15 Pro", "Ốp lưng chống sốc cho iPhone 15 Pro, thiết kế Carbon Fiber.", "spigen-rugged-armor-iphone-15-pro", "[\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"TPU dẻo\"]},\r\n    {\"Name\":\"Chống sốc\",\"Value\":[\"Có - Air Cushion Technology\"]},\r\n    {\"Name\":\"Tương thích\",\"Value\":[\"iPhone 15 Pro (6.1 inch)\"]},\r\n    {\"Name\":\"Thiết kế\",\"Value\":[\"Carbon Fiber texture\"]},\r\n    {\"Name\":\"Viền camera\",\"Value\":[\"Nổi bảo vệ camera\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 81, 790000m, 17, 17, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Tomtoc 360° Protective Laptop Sleeve với CornerArmor bảo vệ góc, YKK zipper.", null, "Tomtoc Laptop Sleeve 13-14 inch", "Túi chống sốc laptop 13-14 inch, lót CornerArmor, chống nước.", "tomtoc-laptop-sleeve-13-14", "[\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"Ballistic Nylon 1680D\"]},\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"13-14 inch laptops\"]},\r\n    {\"Name\":\"Chống nước\",\"Value\":[\"Có - Lớp phủ water-resistant\"]},\r\n    {\"Name\":\"Lót đệm\",\"Value\":[\"CornerArmor + Foam padding\"]},\r\n    {\"Name\":\"Khóa kéo\",\"Value\":[\"YKK RC Fuse\"]}\r\n]", "available", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "IsActive", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "HomeTop", 100, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "HomeTop", 90, 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "HomeTop", 85, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "HomeMiddle", 80, 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "HomeMiddle", 75, 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "CategoryTop", 90, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "CategoryTop", 85, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "SearchTop", 85, 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "SearchTop", 80, 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[] { 17, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", "HomeTop", 95, 12, new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "IsActive", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[] { 18, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "CategoryTop", 88, 4, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_attribute",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "InputType", "Name" },
                values: new object[,]
                {
                    { 50, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Screen Size" },
                    { 51, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Refresh Rate" },
                    { 52, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Resolution" },
                    { 53, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Panel Type" },
                    { 70, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Switch Type" },
                    { 71, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Backlight" },
                    { 72, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Layout" },
                    { 73, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Connection Type" },
                    { 80, 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", "DPI" },
                    { 81, 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Connection Type" },
                    { 82, 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "RGB Lighting" },
                    { 90, 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Type" },
                    { 91, 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Connection Type" },
                    { 92, 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Has Microphone" },
                    { 100, 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Connector Type" },
                    { 101, 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", "Power Output (W)" },
                    { 102, 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "number", "Cable Length (m)" },
                    { 110, 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Material" },
                    { 111, 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "select", "Device Type" }
                });

            migrationBuilder.InsertData(
                table: "product_attribute",
                columns: new[] { "Id", "CategoryId", "InputType", "Name" },
                values: new object[] { 112, 17, "select", "Shock Resistant" });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 1, "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_659_40.png", true, 1, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 2, "https://cdn.tgdd.vn/Products/Images/44/314838/dell-xps-13-plus-9320-i5-71013325-1-750x500.jpg", 1, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 3, "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/m/a/macbook_1__1_8.png", true, 2, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 4, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034", 2, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 5, "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/t/e/text_ng_n_24__3_5.png", true, 3, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 6, "https://dlcdnwebimgs.asus.com/gain/838fbdac-6d10-4190-8e52-d4b9463f5d23/", 3, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 7, "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/l/a/laptop-hp-spectre-x360-14-ef0030tu-6k773pa-cu-dep-1_4.png", true, 4, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 8, "https://www.hp.com/content/dam/sites/worldwide/personal-computers/consumer/laptops-and-2-n-1s/spectre/version-2023/HP%20Spectre%20x360%2014__Mobile@2x.png", 4, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 9, "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_744_2__7.png", true, 5, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 10, "https://mac24h.vn/images/detailed/94/ThinkPad_X1_Carbon_Gen_11.png", 5, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 11, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-max-bluetitanium-select?wid=470&hei=556&fmt=png-alpha&.v=1692845702781", true, 10, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 12, "https://www.apple.com/v/iphone-17-pro/d/images/overview/contrast/iphone_air__fe2gdmh5u5qy_large_2x.jpg", 10, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 13, "https://baotinmobile.vn/uploads/2024/02/s24-ultra-tim.jpg", true, 11, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 14, "https://happyphone.vn/wp-content/uploads/2024/04/SAMSUNG-GALAXY-S24-ULTRA-12GB-512GB-Cam.jpg", 11, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 15, "https://www.didongmy.com/vnt_upload/product/10_2023/pixel8/thumbs/600_crop_google-pixel-8-pro-obsidian-thumb-didongmy-600x600.jpg", true, 12, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 16, "https://cdn.tgdd.vn/Products/Images/42/307188/google-pixel-8-pro-600x600.jpg", 12, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 17, "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/x/i/xiaomi-14_4.png", true, 13, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 18, "https://cdn.mobilecity.vn/mobilecity-vn/images/2023/10/xiaomi-14-hong.jpg.webp", 13, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 19, "https://www.duchuymobile.com/images/detailed/65/oneplus-12-trang_ucuo-lm.jpg", true, 14, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 20, "https://cdn2.cellphones.com.vn/x/media/catalog/product/o/n/oneplus-12_1_.jpg", 14, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 21, "https://traidepbaniphone.com/upload/product/ipadpro11-inwi-fisilver2-upscreenusen-3047.png", true, 20, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 22, "https://phucanhcdn.com/media/product/49293_cellular_512gb.jpg", 20, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 23, "https://product.hstatic.net/1000379731/product/mul3dutway0g35ul8rlp_bc0427d5a0594b43820baccffe69c71b.png", true, 21, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 24, "https://lh6.googleusercontent.com/proxy/mbdwj7VJ7KF0K0HYg2U_TBtAhgH4BBwl8w4ngxSsHSzI1psonPR0hpi1Jf7hFerv42m6zMkx5XEuXnhvggCUs5E8SiWyL7bjXC9f0iOa0i_vOotYHaCd71ywDccS", 21, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 25, "https://phukienpico.com/wp-content/uploads/2023/09/Op-lung-bao-da-xiaomi-pad-6-pro-10.jpg", true, 22, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 26, "https://cdn.tgdd.vn/Products/Images/522/309848/Kit/xiaomi-pad-6-note-1-1.jpg", 22, 2 });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "CreatedAt", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 25990000m, 1, "XPS13-I5-8-512", "available", 15, null },
                    { 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 32990000m, 1, "XPS13-I7-16-1TB", "available", 8, null },
                    { 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 2, "MBA-M2-8-256", "available", 12, null },
                    { 4, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 36990000m, 2, "MBA-M2-16-512", "available", 7, null },
                    { 5, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 22490000m, 3, "ZEN14-I5-8-512", "available", 20, null },
                    { 6, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 3, "ZEN14-I7-16-1TB", "available", 10, null },
                    { 7, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 42990000m, 4, "HPX360-I5-8-512", "available", 12, null },
                    { 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 52990000m, 4, "HPX360-I7-16-1TB", "available", 6, null },
                    { 9, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 48990000m, 5, "X1C11-I5-16-512", "available", 10, null },
                    { 10, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 58990000m, 5, "X1C11-I7-16-1TB", "available", 5, null },
                    { 11, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 33990000m, 10, "IP15PM-256-NAT", "available", 25, null },
                    { 12, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 39990000m, 10, "IP15PM-512-NAT", "available", 15, null },
                    { 13, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 33990000m, 10, "IP15PM-256-BLK", "available", 20, null },
                    { 14, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 29990000m, 11, "S24U-256-GRAY", "available", 30, null },
                    { 15, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 34990000m, 11, "S24U-512-GRAY", "available", 18, null },
                    { 16, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 29990000m, 11, "S24U-256-VIOL", "available", 25, null },
                    { 17, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 12, "PIX8P-128-OBSI", "available", 20, null },
                    { 18, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 27990000m, 12, "PIX8P-256-OBSI", "available", 15, null },
                    { 19, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 27990000m, 12, "PIX8P-256-BAY", "available", 12, null },
                    { 20, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 18990000m, 13, "MI14-12-256-BLK", "available", 35, null },
                    { 21, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 21990000m, 13, "MI14-12-512-BLK", "available", 22, null },
                    { 22, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 19990000m, 14, "OP12-12-256-BLK", "available", 28, null },
                    { 23, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 23990000m, 14, "OP12-16-512-GRN", "available", 18, null },
                    { 24, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 20, "IPADPRO11-128-SIL", "available", 18, null },
                    { 25, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 20, "IPADPRO11-256-SIL", "available", 12, null },
                    { 26, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 20, "IPADPRO11-256-SPC", "available", 10, null },
                    { 27, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 19990000m, 21, "TABS9-8-128-GRAY", "available", 25, null },
                    { 28, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 21, "TABS9-12-256-GRAY", "available", 15, null },
                    { 29, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 22, "MIPAD6-6-128-GRAY", "available", 40, null },
                    { 30, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 10990000m, 22, "MIPAD6-8-256-BLUE", "available", 30, null }
                });

            migrationBuilder.InsertData(
                table: "user_addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CreatedAt", "IsDefault", "PhoneNumber", "PostalCode", "RecipientName", "State", "UpdatedAt", "UserId" },
                values: new object[] { 1, "123 Nguyen Hue Street", "Ben Nghe Ward", "District 1", "Vietnam", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), true, "0912345678", "700000", "Customer User", "Ho Chi Minh City", null, 2 });

            migrationBuilder.InsertData(
                table: "user_addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CreatedAt", "PhoneNumber", "PostalCode", "RecipientName", "State", "UpdatedAt", "UserId" },
                values: new object[] { 2, "456 Le Loi Boulevard", "Ben Thanh Ward", "District 1", "Vietnam", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), "0912345678", "700000", "Customer User", "Ho Chi Minh City", null, 2 });

            migrationBuilder.InsertData(
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 50, 50, "21 inch" },
                    { 51, 50, "24 inch" },
                    { 52, 50, "27 inch" },
                    { 53, 51, "60Hz" },
                    { 54, 51, "120Hz" },
                    { 55, 51, "144Hz" },
                    { 56, 52, "1080p" },
                    { 57, 52, "1440p" },
                    { 58, 52, "4K" },
                    { 70, 70, "Red" },
                    { 71, 70, "Blue" },
                    { 72, 70, "Brown" },
                    { 73, 73, "Wired" },
                    { 74, 73, "Wireless" },
                    { 80, 80, "800" },
                    { 81, 80, "1600" },
                    { 82, 81, "Wired" },
                    { 83, 81, "Wireless" },
                    { 90, 90, "In-Ear" },
                    { 91, 90, "Over-Ear" },
                    { 92, 91, "3.5mm" },
                    { 93, 91, "USB-C" },
                    { 94, 91, "Bluetooth" },
                    { 100, 100, "USB-C" },
                    { 101, 101, "65" },
                    { 110, 110, "Silicone" },
                    { 111, 110, "Leather" },
                    { 112, 110, "Plastic" },
                    { 113, 111, "IPhone 17" },
                    { 114, 111, "Samsung Galaxy S24" },
                    { 115, 112, "Yes" }
                });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "IsActive", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[,]
                {
                    { 6, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "HomeBottom", 70, 60, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "HomeBottom", 65, 61, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "CategoryMiddle", 80, 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "CategoryMiddle", 75, 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "ProductSidebar", 70, 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "ProductSidebar", 65, 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "ProductSidebar", 60, 70, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 27, "https://product.hstatic.net/200000637319/product/1_dcfa8a17409f453cae523f6894013556_master.jpg", true, 30, 1 },
                    { 28, "https://pcmarket.vn/media/lib/29-06-2022/27gn800-b4.jpg", true, 31, 1 },
                    { 29, "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/a/gaming_8_14__1.png", true, 40, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 30, "https://product.hstatic.net/200000637319/product/mx-keys-mini-top-rose-us_a6b9661eb3424f1c8d79503e2cc3e0e7_master.png", 40, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 31, "https://product.hstatic.net/200000637319/product/81eeknarvil._ac_sl1500__b82e73d82da2451ca567fb128494d6aa_master.jpg", true, 41, 1 },
                    { 32, "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/c/h/chuot-khong-day-logitech-mx-master-3s-for-mac_2.png", true, 50, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 33, "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-3s/gallery/mx-master-3s-mouse-side-view-graphite.png?v=1", 50, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 34, "https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/86/357719/chuot-sac-khong-day-gaming-razer-viper-v3-pro-thumb-638967440293053901-600x600.jpg", true, 51, 1 },
                    { 35, "https://www.sony.com.vn/image/5d02da5df552836db894cead8a68f5f3?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF", true, 60, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 36, "https://cdn.tgdd.vn/Products/Images/54/313692/tai-nghe-bluetooth-chup-tai-sony-wh1000xm5-trang-1-750x500.jpg", 60, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 37, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/MQD83?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1660803972361", true, 61, 1 },
                    { 38, "https://photo2.tinhte.vn/data/attachment-files/2022/07/6065972_anker-GaNPrime-tinhte-4.png", true, 70, 1 },
                    { 39, "https://bizweb.dktcdn.net/thumb/large/100/462/529/products/0-1-1713511388939.jpg?v=1713511395120", true, 71, 1 },
                    { 40, "https://m.media-amazon.com/images/I/81osE87mFrL.jpg", true, 80, 1 },
                    { 41, "https://cdn2.cellphones.com.vn/x/media/catalog/product/t/o/tomtoc-slim-tui-chong-soc-1.png", true, 81, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "CreatedAt", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[,]
                {
                    { 31, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 17990000m, 30, "U2723DE-27QHD", "available", 15, null },
                    { 32, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8490000m, 31, "27GN800-QHD144", "available", 22, null },
                    { 33, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2990000m, 40, "MXKEYS-GRAY", "available", 35, null },
                    { 34, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2990000m, 40, "MXKEYS-WHT", "available", 28, null },
                    { 35, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 5990000m, 41, "BW-V4PRO-GRN", "available", 20, null },
                    { 36, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 50, "MXM3S-GRAPH", "available", 40, null },
                    { 37, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 50, "MXM3S-PALE", "available", 32, null },
                    { 38, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3990000m, 51, "VIPER-V2PRO-BLK", "available", 25, null },
                    { 39, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3990000m, 51, "VIPER-V2PRO-WHT", "available", 20, null },
                    { 40, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 60, "XM5-BLK", "available", 22, null },
                    { 41, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 60, "XM5-SLV", "available", 18, null },
                    { 42, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 6490000m, 61, "AIRPODSPRO2-USBC", "available", 35, null },
                    { 43, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 70, "ANKER747-150W", "available", 45, null },
                    { 44, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 290000m, 71, "BASEUS-C2C-2M-BLK", "available", 120, null },
                    { 45, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 290000m, 71, "BASEUS-C2C-2M-WHT", "available", 100, null },
                    { 46, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 490000m, 80, "SPIGEN-IP15P-BLK", "available", 80, null },
                    { 47, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 490000m, 80, "SPIGEN-IP15P-CLEAR", "available", 70, null },
                    { 48, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 790000m, 81, "TOMTOC-13-BLK", "available", 50, null },
                    { 49, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 790000m, 81, "TOMTOC-14-BLK", "available", 45, null }
                });

            migrationBuilder.InsertData(
                table: "product_variant_attribute",
                columns: new[] { "AttributeId", "VariantId", "ValueId" },
                values: new object[,]
                {
                    { 1, 1, 3 },
                    { 12, 1, 17 },
                    { 13, 1, 21 },
                    { 1, 2, 1 },
                    { 12, 2, 18 },
                    { 13, 2, 22 },
                    { 1, 3, 3 },
                    { 12, 3, 17 },
                    { 13, 3, 20 },
                    { 1, 4, 3 },
                    { 12, 4, 18 },
                    { 13, 4, 21 },
                    { 1, 11, 3 },
                    { 24, 11, 43 },
                    { 1, 12, 1 },
                    { 24, 12, 44 },
                    { 1, 13, 3 },
                    { 24, 13, 43 },
                    { 1, 14, 4 },
                    { 24, 14, 44 },
                    { 1, 17, 3 },
                    { 33, 17, 43 },
                    { 1, 18, 1 },
                    { 33, 18, 44 },
                    { 50, 21, 52 },
                    { 52, 21, 58 },
                    { 70, 23, 70 },
                    { 73, 23, 74 },
                    { 80, 25, 80 },
                    { 81, 25, 83 },
                    { 90, 27, 91 },
                    { 91, 27, 94 },
                    { 100, 29, 100 },
                    { 101, 29, 101 },
                    { 110, 31, 110 },
                    { 111, 31, 113 },
                    { 112, 31, 115 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_attribute_value_AttributeId_Value",
                table: "attribute_value",
                columns: new[] { "AttributeId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_products_BlogPostId",
                table: "blog_post_products",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_products_BlogPostId_ProductId",
                table: "blog_post_products",
                columns: new[] { "BlogPostId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_products_ProductId",
                table: "blog_post_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_tags_BlogPostId_Tag",
                table: "blog_post_tags",
                columns: new[] { "BlogPostId", "Tag" });

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_tags_Tag",
                table: "blog_post_tags",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_blog_posts_CreatedAt",
                table: "blog_posts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_blog_posts_PublishedAt",
                table: "blog_posts",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_blog_posts_Slug",
                table: "blog_posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blog_posts_Status",
                table: "blog_posts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_brand_Slug",
                table: "brand",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_CartId_VariantId",
                table: "cart_items",
                columns: new[] { "CartId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_category_ParentId",
                table: "category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_category_Slug",
                table: "category",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupon_usages_CouponId_UserId",
                table: "coupon_usages",
                columns: new[] { "CouponId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_coupons_Code",
                table: "coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_OrderId",
                table: "order_items",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ProductId",
                table: "order_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderNumber",
                table: "orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status",
                table: "orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserId",
                table: "orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_CreatedAt",
                table: "payment",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payment_OrderId",
                table: "payment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_Status",
                table: "payment",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_payment_TransactionId",
                table: "payment",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_UserId",
                table: "payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_product_BrandId",
                table: "product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_product_CategoryId",
                table: "product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_Slug",
                table: "product",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_IsActive",
                table: "product_advertisements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_IsActive_Position_Priority",
                table: "product_advertisements",
                columns: new[] { "IsActive", "Position", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_Position",
                table: "product_advertisements",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_ProductId",
                table: "product_advertisements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_attribute_CategoryId",
                table: "product_attribute",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_image_ProductId",
                table: "product_image",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_ProductId",
                table: "product_reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_Status",
                table: "product_reviews",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_UserId",
                table: "product_reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_UserId_ProductId",
                table: "product_reviews",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_ProductId",
                table: "product_variant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_Sku",
                table: "product_variant",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attribute_AttributeId",
                table: "product_variant_attribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attribute_ValueId",
                table: "product_variant_attribute",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_review_helpfulness_ReviewId_UserId",
                table: "review_helpfulness",
                columns: new[] { "ReviewId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_Email",
                table: "user",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_RoleId",
                table: "user",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_addresses_UserId",
                table: "user_addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_carts_UserId",
                table: "user_carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_UserId_VariantId",
                table: "wishlists",
                columns: new[] { "UserId", "VariantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_post_products");

            migrationBuilder.DropTable(
                name: "blog_post_tags");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "coupon_categories");

            migrationBuilder.DropTable(
                name: "coupon_products");

            migrationBuilder.DropTable(
                name: "coupon_usages");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "product_advertisements");

            migrationBuilder.DropTable(
                name: "product_image");

            migrationBuilder.DropTable(
                name: "product_variant_attribute");

            migrationBuilder.DropTable(
                name: "review_helpfulness");

            migrationBuilder.DropTable(
                name: "user_addresses");

            migrationBuilder.DropTable(
                name: "wishlists");

            migrationBuilder.DropTable(
                name: "blog_posts");

            migrationBuilder.DropTable(
                name: "user_carts");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "attribute_value");

            migrationBuilder.DropTable(
                name: "product_variant");

            migrationBuilder.DropTable(
                name: "product_reviews");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "product_attribute");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
