using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAdvertisement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_advertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "HomeTop"),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    StartDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_advertisements_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_IsActive",
                table: "product_advertisements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_IsActive_Position_Priority",
                table: "product_advertisements",
                columns: new[] { "IsActive", "Position", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_Position",
                table: "product_advertisements",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_product_advertisements_ProductId",
                table: "product_advertisements",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_advertisements");
        }
    }
}
