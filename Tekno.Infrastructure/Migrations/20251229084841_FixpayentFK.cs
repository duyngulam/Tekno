using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixpayentFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_orders_OrderId",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_orders_OrderId1",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_OrderId1",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "payment");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_orders_OrderId",
                table: "payment",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_orders_OrderId",
                table: "payment");

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
                name: "FK_payment_orders_OrderId",
                table: "payment",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_orders_OrderId1",
                table: "payment",
                column: "OrderId1",
                principalTable: "orders",
                principalColumn: "Id");
        }
    }
}
