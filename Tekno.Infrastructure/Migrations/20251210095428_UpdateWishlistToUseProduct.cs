using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWishlistToUseProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VariantId",
                table: "wishlists",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_wishlists_UserId_VariantId",
                table: "wishlists",
                newName: "IX_wishlists_UserId_ProductId");

            migrationBuilder.UpdateData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductId",
                value: 21);

            migrationBuilder.UpdateData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProductId",
                value: 30);

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_ProductId",
                table: "wishlists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_VariantId",
                table: "cart_items",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_product_variant_VariantId",
                table: "cart_items",
                column: "VariantId",
                principalTable: "product_variant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wishlists_product_ProductId",
                table: "wishlists",
                column: "ProductId",
                principalTable: "product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_product_variant_VariantId",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_wishlists_product_ProductId",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_wishlists_ProductId",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_cart_items_VariantId",
                table: "cart_items");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "wishlists",
                newName: "VariantId");

            migrationBuilder.RenameIndex(
                name: "IX_wishlists_UserId_ProductId",
                table: "wishlists",
                newName: "IX_wishlists_UserId_VariantId");

            migrationBuilder.UpdateData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 2,
                column: "VariantId",
                value: 27);

            migrationBuilder.UpdateData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 3,
                column: "VariantId",
                value: 31);
        }
    }
}
