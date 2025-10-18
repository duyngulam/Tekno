using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                name: "product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "available"),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
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
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
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
                name: "product_detail",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    LongDescription = table.Column<string>(type: "text", nullable: true),
                    WarrantyInfo = table.Column<string>(type: "text", nullable: true),
                    Specs = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_detail", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_product_detail_product_ProductId",
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
                table: "brand",
                columns: new[] { "Id", "Country", "CreatedAt", "LogoUrl", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/dell-2", "Dell", "dell", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 2, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/apple-13", "Apple", "apple", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 3, "Taiwan", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/asus-4", "Asus", "asus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 4, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/HP-5", "HP", "hp", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 5, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/lenovo-2", "Lenovo", "lenovo", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 6, "Korea", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/samsung-8", "Samsung", "samsung", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 7, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/google-1", "Google", "google", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 8, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/xiaomi-5", "Xiaomi", "xiaomi", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 9, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/oneplus-2", "OnePlus", "oneplus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 10, "Korea", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/lg", "LG", "lg", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 11, "Switzerland", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/logitech-gaming-2", "Logitech", "logitech", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 12, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/razer-1", "Razer", "razer", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 13, "Japan", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/sony-2", "Sony", "sony", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 14, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://worldvectorlogo.com/logo/anker-logo-1", "Anker", "anker", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 15, "China", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://mms.img.susercontent.com/vn-11134216-7r98o-lnicyi57m5x6fd", "Baseus", "baseus", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 16, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://spigen.vn/wp-content/uploads/2023/09/Spigen_Header_New_Logo.png", "Spigen", "spigen", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 17, "USA", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlpSaYkZMxWktmvmvOx7mDurTEDu0KXqz1HQ&s", "UAG", "uag", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of laptops", "Laptop", null, "laptop", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of smartphones", "Smartphone", null, "smartphone", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of tablets", "Tablet", null, "tablet", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "External product that enhances main product experience", "Accessory", null, "accessory", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All kinds of cameras", "Camera", null, "camera", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "PC and office related products", "Computer & Office", null, "computer-office", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Gaming products and accessories", "Gaming", null, "gaming", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
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
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 1, 1, "Black" },
                    { 2, 1, "White" },
                    { 3, 1, "Silver" },
                    { 4, 1, "Blue" },
                    { 5, 1, "Red" },
                    { 10, 2, "Apple" },
                    { 11, 2, "Samsung" },
                    { 12, 2, "Asus" },
                    { 13, 2, "HP" },
                    { 14, 2, "Dell" }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "ParentId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "All types of computer monitors", "Monitor", 6, "monitor", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Processors and chips for computers", "CPU", 6, "cpu", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Graphics cards for PCs and laptops", "GPU", 6, "gpu", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Memory modules for PCs and laptops", "RAM", 6, "ram", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Storage devices: SSD, HDD, memory cards", "Storage (SSD / HDD)", 6, "storage", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Keyboards for PC, Laptop, and Tablet", "Keyboard", 4, "keyboard", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Computer and laptop mice (wired, wireless, gaming)", "Mouse", 4, "mouse", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Audio accessories compatible with PC, Laptop, and Smartphone", "Headphone / Headset", 4, "headphone", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Chargers, adapters, and data cables for all devices", "Charger & Cable", 4, "charger-cable", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Protective cases for phones, tablets, and laptops", "Case & Cover", 4, "case-cover", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product",
                columns: new[] { "Id", "BasePrice", "BrandId", "CategoryId", "CreatedAt", "Description", "Name", "Overview", "Slug", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1699.00m, 1, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Dell XPS 13 series laptops designed for professionals.", "Dell XPS 13", "Premium ultrabook with compact design.", "dell-xps-13", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 2, 1199.00m, 2, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "MacBook Air powered by Apple M-series chips.", "MacBook Air", "Ultra-thin and lightweight laptop by Apple.", "macbook-air", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 3, 1450.00m, 3, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "ZenBook series with Intel and AMD variants.", "Asus ZenBook", "Portable productivity ultrabook.", "asus-zenbook", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 4, 1799.00m, 4, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "2-in-1 design with touch and pen support.", "HP Spectre x360", "Convertible premium laptop.", "hp-spectre-x360", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 5, 1999.00m, 5, 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "ThinkPad X1 Carbon for professionals.", "Lenovo ThinkPad X1 Carbon", "Business ultrabook with robust build.", "thinkpad-x1-carbon", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 10, 999.00m, 2, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "iPhone 15 Pro with titanium frame and A17 Pro chip.", "iPhone 15 Pro", "Apple flagship smartphone.", "iphone-15-pro", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 11, 899.00m, 6, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Galaxy S24 with AMOLED display and powerful camera.", "Samsung Galaxy S24", "Next-gen Android flagship.", "galaxy-s24", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 12, 799.00m, 7, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Pixel 9 with Tensor G4 chip and AI photography.", "Google Pixel 9", "Pure Android experience.", "pixel-9", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 13, 850.00m, 8, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Xiaomi 14 Pro with Leica cameras.", "Xiaomi 14 Pro", "High-end performance smartphone.", "xiaomi-14-pro", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 14, 749.00m, 9, 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "OnePlus 12 offers fast charging and high refresh rate.", "OnePlus 12", "Performance-focused smartphone.", "oneplus-12", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 20, 899.00m, 2, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "iPad Pro with M2 chip and ProMotion display.", "iPad Pro", "Powerful tablet for creators.", "ipad-pro", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 21, 799.00m, 6, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Galaxy Tab S9 series with AMOLED display.", "Samsung Galaxy Tab S9", "Android flagship tablet.", "galaxy-tab-s9", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 22, 419.00m, 5, 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Tab P12 supports stylus and multi-tasking.", "Lenovo Tab P12", "Affordable productivity tablet.", "lenovo-tab-p12", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
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
                columns: new[] { "Id", "Email", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, "admin@tekno.com", "$2a$11$W/ZYaZwxFhbSWpJNtPMAfetjQIsqJ1rYdiP2GQoF1.Hr7aqFmtaya", 1 },
                    { 2, "customer@tekno.com", "$2a$11$ZKxnFd0g1qcrtOgFJrbYiOOnKrtsA6flk4msMC0Uf/qcmqYzoUlSq", 2 }
                });

            migrationBuilder.InsertData(
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 20, 11, "Intel i5" },
                    { 21, 11, "Intel i7" },
                    { 22, 11, "AMD Ryzen 5" },
                    { 23, 11, "AMD Ryzen 7" },
                    { 30, 12, "8GB" },
                    { 31, 12, "16GB" },
                    { 32, 12, "32GB" },
                    { 40, 13, "256GB SSD" },
                    { 41, 13, "512GB SSD" },
                    { 42, 13, "1TB SSD" },
                    { 50, 10, "13 inch" },
                    { 51, 10, "15 inch" },
                    { 52, 10, "17 inch" },
                    { 60, 14, "RTX 4060" },
                    { 61, 14, "RTX 4070" },
                    { 62, 14, "GTX 1650" },
                    { 80, 61, "Wired" },
                    { 81, 61, "Wireless" }
                });

            migrationBuilder.InsertData(
                table: "product",
                columns: new[] { "Id", "BasePrice", "BrandId", "CategoryId", "CreatedAt", "Description", "Name", "Overview", "Slug", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 30, 699.00m, 1, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Color-accurate UltraSharp series for designers.", "Dell UltraSharp 27", "Professional 4K monitor.", "dell-ultrasharp-27", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 31, 450.00m, 10, 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "High-performance display for gamers.", "LG Ultragear 32", "Gaming monitor with 165Hz refresh rate.", "lg-ultragear-32", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 40, 119.99m, 11, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Backlit keyboard with multi-device support.", "Logitech MX Keys", "Wireless keyboard for professionals.", "logitech-mx-keys", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 41, 169.99m, 12, 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "RGB lighting and tactile feedback.", "Razer BlackWidow V4", "Mechanical gaming keyboard.", "razer-blackwidow-v4", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 50, 99.99m, 11, 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Supports multiple devices with customizable buttons.", "Logitech MX Master 3S", "Ergonomic productivity mouse.", "logitech-mx-master-3s", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 51, 149.99m, 12, 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Wireless mouse with high precision sensor.", "Razer Viper V2 Pro", "Ultra-light gaming mouse.", "razer-viper-v2-pro", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 60, 399.99m, 13, 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Industry-leading noise cancellation for audio lovers.", "Sony WH-1000XM5", "Noise-cancelling wireless headphones.", "sony-wh-1000xm5", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 61, 249.00m, 2, 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Compact design and adaptive sound control.", "Apple AirPods Pro 2", "Wireless earbuds with active noise cancellation.", "airpods-pro-2", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 70, 49.99m, 14, 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Compact USB-C charger for laptops and phones.", "Anker 65W GaN Charger", "Fast charger with GaN technology.", "anker-65w-gan", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 71, 14.99m, 15, 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Supports fast charging and data transfer.", "Baseus USB-C Cable 1.5m", "Durable braided charging cable.", "baseus-usb-c-cable", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 80, 29.99m, 16, 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Shock-absorbing TPU material.", "Spigen Rugged Armor Case", "Protective phone case.", "spigen-rugged-armor", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) },
                    { 81, 49.99m, 17, 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Designed for 13–15 inch laptops.", "UAG Plasma Laptop Sleeve", "Durable protective sleeve.", "uag-laptop-sleeve", "available", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) }
                });

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
                table: "product_detail",
                columns: new[] { "ProductId", "LongDescription", "Specs", "WarrantyInfo" },
                values: new object[,]
                {
                    { 1, null, "{\r\n    \"Display\": \"13.4-inch FHD+ InfinityEdge\",\r\n    \"CPU\": \"Intel Core i5 / i7\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"512GB / 1TB SSD\",\r\n    \"Weight\": \"1.2kg\",\r\n    \"Battery\": \"52Wh\",\r\n    \"OS\": \"Windows 11\",\r\n    \"Warranty\": \"12 months\"\r\n}", null },
                    { 2, null, "{\r\n    \"Display\": \"13.6-inch Liquid Retina\",\r\n    \"Chip\": \"Apple M2\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"256GB / 512GB\",\r\n    \"Battery\": \"52.6Wh up to 18h\",\r\n    \"OS\": \"macOS\",\r\n    \"Weight\": \"1.24kg\"\r\n}", null },
                    { 3, null, "{\r\n    \"Display\": \"14-inch OLED 2.8K\",\r\n    \"CPU\": \"Intel i5 / i7 or Ryzen 7\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"512GB / 1TB SSD\",\r\n    \"OS\": \"Windows 11\",\r\n    \"Weight\": \"1.3kg\"\r\n}", null },
                    { 4, null, "{\r\n    \"Display\": \"13.5-inch 2-in-1 Touch OLED\",\r\n    \"CPU\": \"Intel Core i5 / i7\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"512GB / 1TB\",\r\n    \"Convertible\": true,\r\n    \"OS\": \"Windows 11 Home\"\r\n}", null },
                    { 5, null, "{\r\n    \"Display\": \"14-inch IPS 2.8K\",\r\n    \"CPU\": \"Intel i5 / i7\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"512GB / 1TB\",\r\n    \"Security\": \"Fingerprint + TPM 2.0\",\r\n    \"OS\": \"Windows 11 Pro\"\r\n}", null },
                    { 10, null, "{\r\n    \"Display\": \"6.1-inch OLED 120Hz\",\r\n    \"Chip\": \"Apple A17 Pro\",\r\n    \"RAM\": \"6GB\",\r\n    \"Storage\": \"128GB / 256GB\",\r\n    \"Camera\": \"48MP + 12MP + 12MP\",\r\n    \"Battery\": \"3279mAh\",\r\n    \"OS\": \"iOS 17\"\r\n}", null },
                    { 11, null, "{\r\n    \"Display\": \"6.7-inch Dynamic AMOLED 120Hz\",\r\n    \"Chip\": \"Snapdragon 8 Gen 3\",\r\n    \"RAM\": \"8GB\",\r\n    \"Storage\": \"128GB / 256GB\",\r\n    \"Camera\": \"200MP + 12MP + 10MP\",\r\n    \"Battery\": \"5000mAh\",\r\n    \"OS\": \"Android 14\"\r\n}", null },
                    { 12, null, "{\r\n    \"Display\": \"6.3-inch AMOLED 120Hz\",\r\n    \"Chip\": \"Google Tensor G4\",\r\n    \"RAM\": \"8GB\",\r\n    \"Storage\": \"128GB / 256GB\",\r\n    \"Camera\": \"50MP + 12MP\",\r\n    \"Battery\": \"4700mAh\",\r\n    \"OS\": \"Android 14\"\r\n}", null },
                    { 13, null, "{\r\n    \"Display\": \"6.7-inch AMOLED QHD+\",\r\n    \"Chip\": \"Snapdragon 8 Gen 3\",\r\n    \"RAM\": \"12GB\",\r\n    \"Storage\": \"256GB / 512GB\",\r\n    \"Camera\": \"50MP + 50MP + 50MP (Leica)\",\r\n    \"OS\": \"HyperOS (Android 14)\"\r\n}", null },
                    { 14, null, "{\r\n    \"Display\": \"6.8-inch AMOLED 120Hz\",\r\n    \"Chip\": \"Snapdragon 8 Gen 3\",\r\n    \"RAM\": \"12GB\",\r\n    \"Storage\": \"256GB / 512GB\",\r\n    \"Battery\": \"5400mAh 100W charging\",\r\n    \"OS\": \"OxygenOS 14\"\r\n}", null },
                    { 20, null, "{\r\n    \"Display\": \"12.9-inch Liquid Retina XDR\",\r\n    \"Chip\": \"Apple M2\",\r\n    \"RAM\": \"8GB / 16GB\",\r\n    \"Storage\": \"128GB / 256GB\",\r\n    \"OS\": \"iPadOS 17\",\r\n    \"PencilSupport\": \"Apple Pencil 2\"\r\n}", null },
                    { 21, null, "{\r\n    \"Display\": \"11-inch AMOLED 120Hz\",\r\n    \"Chip\": \"Snapdragon 8 Gen 2\",\r\n    \"RAM\": \"8GB / 12GB\",\r\n    \"Storage\": \"128GB / 256GB\",\r\n    \"OS\": \"Android 14\"\r\n}", null },
                    { 22, null, "{\r\n    \"Display\": \"12.7-inch LCD 144Hz\",\r\n    \"Chip\": \"MediaTek Dimensity 7050\",\r\n    \"RAM\": \"8GB\",\r\n    \"Storage\": \"128GB\",\r\n    \"Battery\": \"10200mAh\",\r\n    \"OS\": \"Android 13\"\r\n}", null }
                });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "CreatedAt", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 1, "XPS13-I5-8-512", "available", 20, null },
                    { 2, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1499.00m, 1, "XPS13-I7-16-1TB", "available", 10, null },
                    { 3, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 2, "MBA-M2-8-256", "available", 15, null },
                    { 4, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, 2, "MBA-M2-16-512", "available", 8, null },
                    { 5, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, 3, "ZEN-I5-8-512", "available", 25, null },
                    { 6, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, 3, "ZEN-I7-16-1TB", "available", 12, null },
                    { 7, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 4, "HPX360-I5-8-512", "available", 18, null },
                    { 8, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1399.00m, 4, "HPX360-I7-16-1TB", "available", 10, null },
                    { 9, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, 5, "X1-I5-8-512", "available", 14, null },
                    { 10, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1499.00m, 5, "X1-I7-16-1TB", "available", 8, null },
                    { 11, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, 10, "IP15P-128", "available", 20, null },
                    { 12, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, 10, "IP15P-256", "available", 15, null },
                    { 13, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 11, "S24-128", "available", 25, null },
                    { 14, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 11, "S24-256", "available", 20, null },
                    { 15, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, 12, "PIX9-128", "available", 18, null },
                    { 16, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 12, "PIX9-256", "available", 12, null },
                    { 17, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 20, "IPADPRO-128", "available", 20, null },
                    { 18, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, 20, "IPADPRO-256", "available", 15, null },
                    { 19, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, 21, "TABS9-128", "available", 25, null },
                    { 20, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 21, "TABS9-256", "available", 15, null }
                });

            migrationBuilder.InsertData(
                table: "attribute_value",
                columns: new[] { "Id", "AttributeId", "Value" },
                values: new object[,]
                {
                    { 70, 70, "Red" },
                    { 71, 70, "Blue" },
                    { 72, 70, "Brown" },
                    { 82, 73, "Wired" },
                    { 83, 81, "Bluetooth" },
                    { 84, 91, "3.5mm" },
                    { 85, 91, "USB-C" },
                    { 90, 100, "USB-C" },
                    { 91, 101, "65" },
                    { 100, 110, "Silicone" },
                    { 101, 110, "Leather" },
                    { 102, 110, "Plastic" }
                });

            migrationBuilder.InsertData(
                table: "product_detail",
                columns: new[] { "ProductId", "LongDescription", "Specs", "WarrantyInfo" },
                values: new object[,]
                {
                    { 30, null, "{\r\n    \"Display\": \"27-inch IPS 4K UHD\",\r\n    \"Resolution\": \"3840x2160\",\r\n    \"RefreshRate\": \"60Hz\",\r\n    \"Ports\": \"HDMI, DisplayPort, USB-C\",\r\n    \"ColorGamut\": \"99% sRGB\",\r\n    \"Warranty\": \"24 months\"\r\n}", null },
                    { 31, null, "{\r\n    \"Display\": \"32-inch VA QHD\",\r\n    \"Resolution\": \"2560x1440\",\r\n    \"RefreshRate\": \"165Hz\",\r\n    \"Ports\": \"HDMI, DisplayPort\",\r\n    \"Sync\": \"G-Sync Compatible\"\r\n}", null },
                    { 40, null, "{ \"Type\": \"Wireless\", \"Layout\": \"Full-size\", \"Backlight\": \"Yes\", \"Battery\": \"USB-C rechargeable\" }", null },
                    { 41, null, "{ \"Type\": \"Mechanical\", \"Switch\": \"Razer Green\", \"Backlight\": \"RGB\", \"Connection\": \"Wired\" }", null },
                    { 50, null, "{ \"Sensor\": \"Logitech Darkfield\", \"Connection\": \"Bluetooth / USB\", \"Battery\": \"70 days\", \"Buttons\": 7 }", null },
                    { 51, null, "{ \"Sensor\": \"Focus Pro 30K\", \"Weight\": \"58g\", \"Connection\": \"Wireless\", \"Battery\": \"80h\" }", null },
                    { 60, null, "{ \"Type\": \"Over-ear\", \"ANC\": \"Yes\", \"Battery\": \"30h\", \"Charging\": \"USB-C\", \"Microphone\": \"Yes\" }", null },
                    { 61, null, "{ \"Type\": \"In-ear\", \"ANC\": \"Yes\", \"Battery\": \"6h + 24h\", \"Wireless\": \"Bluetooth 5.3\" }", null },
                    { 70, null, "{ \"Power\": \"65W\", \"Ports\": \"2x USB-C, 1x USB-A\", \"Material\": \"GaN\", \"Input\": \"100–240V\" }", null },
                    { 71, null, "{ \"Length\": \"1.5m\", \"Connector\": \"USB-C to USB-C\", \"Material\": \"Nylon braided\", \"MaxPower\": \"100W\" }", null },
                    { 80, null, "{ \"Material\": \"TPU\", \"ShockResistant\": \"Yes\", \"CompatibleDevices\": \"iPhone 15\" }", null },
                    { 81, null, "{ \"Material\": \"Ballistic Nylon\", \"Fits\": \"13–15 inch laptops\", \"WaterResistant\": \"Yes\" }", null }
                });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "CreatedAt", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[,]
                {
                    { 21, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 499.00m, 30, "DELL-U2720Q", "available", 12, null },
                    { 22, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 599.00m, 31, "LG-UG32", "available", 10, null },
                    { 23, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 119.00m, 40, "MX-KEYS-GRAY", "available", 30, null },
                    { 24, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 149.00m, 41, "RZR-BW-V4", "available", 25, null },
                    { 25, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 99.00m, 50, "MXM3S-GRAPHITE", "available", 35, null },
                    { 26, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 129.00m, 51, "RZR-V2PRO", "available", 28, null },
                    { 27, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 349.00m, 60, "SONY-XM5-BLK", "available", 18, null },
                    { 28, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 249.00m, 61, "AIRPODS-PRO2", "available", 25, null },
                    { 29, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 59.00m, 70, "ANKER-65W", "available", 40, null },
                    { 30, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 19.00m, 71, "BASEUS-CABLE-15", "available", 100, null },
                    { 31, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 29.00m, 80, "SPIGEN-RUGGED", "available", 50, null }
                });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[] { 32, 49.00m, 81, "UAG-SLEEVE", "available", 35, null });

            migrationBuilder.InsertData(
                table: "product_variant_attribute",
                columns: new[] { "AttributeId", "VariantId", "ValueId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 12, 1, 21 },
                    { 13, 1, 31 },
                    { 1, 2, 2 },
                    { 12, 2, 22 },
                    { 13, 2, 32 },
                    { 1, 3, 3 },
                    { 12, 3, 21 },
                    { 13, 3, 30 },
                    { 1, 4, 1 },
                    { 12, 4, 22 },
                    { 13, 4, 31 },
                    { 1, 11, 1 },
                    { 24, 11, 30 },
                    { 1, 12, 2 },
                    { 24, 12, 31 },
                    { 1, 13, 3 },
                    { 24, 13, 30 },
                    { 1, 14, 4 },
                    { 24, 14, 31 },
                    { 1, 17, 1 },
                    { 33, 17, 30 },
                    { 1, 18, 2 },
                    { 33, 18, 31 },
                    { 50, 21, 40 },
                    { 52, 21, 42 },
                    { 70, 23, 50 },
                    { 73, 23, 60 },
                    { 80, 25, 70 },
                    { 81, 25, 60 },
                    { 90, 27, 80 },
                    { 91, 27, 60 },
                    { 92, 27, 81 },
                    { 100, 29, 90 },
                    { 101, 29, 91 },
                    { 110, 31, 100 },
                    { 111, 31, 101 },
                    { 112, 31, 102 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_attribute_value_AttributeId_Value",
                table: "attribute_value",
                columns: new[] { "AttributeId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_brand_Slug",
                table: "brand",
                column: "Slug",
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
                name: "IX_product_attribute_CategoryId",
                table: "product_attribute",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_image_ProductId",
                table: "product_image",
                column: "ProductId");

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
                name: "IX_user_RoleId",
                table: "user",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_detail");

            migrationBuilder.DropTable(
                name: "product_image");

            migrationBuilder.DropTable(
                name: "product_variant_attribute");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "attribute_value");

            migrationBuilder.DropTable(
                name: "product_variant");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "product_attribute");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
