using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class shipaddresstoorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                table: "orders",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "orders");
        }
    }
}
