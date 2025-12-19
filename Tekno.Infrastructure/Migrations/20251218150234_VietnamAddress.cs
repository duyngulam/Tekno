using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VietnamAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DivisionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneCode = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provinces", x => x.Id);
                    table.UniqueConstraint("AK_provinces_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DivisionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProvinceCode = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.Id);
                    table.UniqueConstraint("AK_districts_Code", x => x.Code);
                    table.ForeignKey(
                        name: "FK_districts_provinces_ProvinceCode",
                        column: x => x.ProvinceCode,
                        principalTable: "provinces",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DivisionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DistrictCode = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wards_districts_DistrictCode",
                        column: x => x.DistrictCode,
                        principalTable: "districts",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_Code",
                table: "districts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_districts_ProvinceCode",
                table: "districts",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_provinces_Code",
                table: "provinces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wards_Code",
                table: "wards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wards_DistrictCode",
                table: "wards",
                column: "DistrictCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wards");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "provinces");
        }
    }
}
