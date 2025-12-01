using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcategoryimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "category",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://www.svgrepo.com/show/525970/laptop.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/laptop_jchkjn.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/mobile_qk5kuf.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/iphone_air-3_2_hfq1wl.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/tablet_mhhzhn.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337716/xiaomi-pad-mini-4_adg1r9.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/mouse_enodsx.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/camera_xmozh9.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/may-anh-canon-eos-r100_8__havbm2.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/devices_kty5xc.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/pc_eoswm6.jpg" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/game_opdnni.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ps5_lfmig6.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/monitor_gfheqk.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/monitor_i9d6or.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/CPU_kfg2fy.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336079/cpu_b8usqu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/GPU_ltzw4j.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/ram_kssmbv.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/ram_luys0f.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337715/rom_sreazq.webp" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/banphim_iai2rn.jpg");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg", "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/mouse_enodsx.png" });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "IconPath", "ImageUrl" },
                values: new object[] { "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336342/headphone_pz0fkb.svg", null });

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "category");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 3,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 5,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 6,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 7,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 8,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 9,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 10,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 11,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 12,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 14,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 15,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");
        }
    }
}
