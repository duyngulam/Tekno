using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerNote",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedAt",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCarrier",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNote",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippedAt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCarrier",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "orders");
        }
    }
}
