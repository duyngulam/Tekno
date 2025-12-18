using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlogPostWithProductIdsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_post_products");

            migrationBuilder.AddColumn<string>(
                name: "ProductIds",
                table: "blog_posts",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.UpdateData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductIds",
                value: "[10,61,80]");

            migrationBuilder.UpdateData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductIds",
                value: "[1,2,81]");

            migrationBuilder.UpdateData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProductIds",
                value: "[11,70,80]");

            migrationBuilder.UpdateData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProductIds",
                value: "[31,41,51,60,70]");

            migrationBuilder.UpdateData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProductIds",
                value: "[20,21,22]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "blog_posts");

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
                    table.ForeignKey(
                        name: "FK_blog_post_products_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
        }
    }
}
