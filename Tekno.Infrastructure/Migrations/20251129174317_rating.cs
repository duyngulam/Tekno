using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalSold",
                table: "product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Update product 51 Specs - using proper JSON format
            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                column: "Specs",
                value: "[{\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},{\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},{\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},{\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},{\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSold",
                table: "product");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                column: "Specs",
                value: "[{\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},{\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},{\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},{\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},{\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}]");
        }
    }
}
