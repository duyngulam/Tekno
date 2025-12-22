using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestoreVietnameseAddressSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "user_addresses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "user_addresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "user_addresses");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "user_addresses",
                newName: "ward_name");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "user_addresses",
                newName: "province_name");

            migrationBuilder.RenameColumn(
                name: "AddressLine1",
                table: "user_addresses",
                newName: "address_line");

            migrationBuilder.AddColumn<int>(
                name: "district_code",
                table: "user_addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "district_name",
                table: "user_addresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "province_code",
                table: "user_addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ward_code",
                table: "user_addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "address_line", "district_code", "district_name", "province_code", "province_name", "ward_code", "ward_name" },
                values: new object[] { "123 Nguyễn Huệ", 760, "Quận 1", 79, "Thành phố Hồ Chí Minh", 26734, "Phường Bến Nghé" });

            migrationBuilder.UpdateData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "address_line", "district_code", "district_name", "province_code", "province_name", "ward_code", "ward_name" },
                values: new object[] { "456 Võ Văn Tần", 769, "Quận 3", 79, "Thành phố Hồ Chí Minh", 27031, "Phường 6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "district_code",
                table: "user_addresses");

            migrationBuilder.DropColumn(
                name: "district_name",
                table: "user_addresses");

            migrationBuilder.DropColumn(
                name: "province_code",
                table: "user_addresses");

            migrationBuilder.DropColumn(
                name: "ward_code",
                table: "user_addresses");

            migrationBuilder.RenameColumn(
                name: "ward_name",
                table: "user_addresses",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "province_name",
                table: "user_addresses",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "address_line",
                table: "user_addresses",
                newName: "AddressLine1");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "user_addresses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "user_addresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Vietnam");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "user_addresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressLine1", "AddressLine2", "City", "Country", "PostalCode", "State" },
                values: new object[] { "123 Nguyen Hue Street", "Ben Nghe Ward", "District 1", "Vietnam", "700000", "Ho Chi Minh City" });

            migrationBuilder.UpdateData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AddressLine1", "AddressLine2", "City", "Country", "PostalCode", "State" },
                values: new object[] { "456 Le Loi Boulevard", "Ben Thanh Ward", "District 1", "Vietnam", "700000", "Ho Chi Minh City" });
        }
    }
}
