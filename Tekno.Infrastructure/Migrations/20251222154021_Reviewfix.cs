using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Reviewfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "product_reviews");

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "payment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_OrderId1",
                table: "payment",
                column: "OrderId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_orders_OrderId1",
                table: "payment",
                column: "OrderId1",
                principalTable: "orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_orders_OrderId1",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_OrderId1",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "payment");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "product_reviews",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
