using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rollback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                column: "Specs",
                value: "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}\r\n]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                column: "Specs",
                value: "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}");
        }
    }
}
