using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Blog : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_post_products");

            migrationBuilder.DropTable(
                name: "blog_post_tags");

            migrationBuilder.DropTable(
                name: "blog_posts");
        }
    }
}
