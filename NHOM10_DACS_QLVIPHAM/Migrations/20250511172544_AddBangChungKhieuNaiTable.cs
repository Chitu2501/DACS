using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddBangChungKhieuNaiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTraLoi",
                table: "KhieuNaiVPhams",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "MaBienBan",
                table: "KhieuNaiVPhams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BangChungKhieuNais",
                columns: table => new
                {
                    MaBangChung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhieuNai = table.Column<int>(type: "int", nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangChungKhieuNais", x => x.MaBangChung);
                    table.ForeignKey(
                        name: "FK_BangChungKhieuNais_KhieuNaiVPhams_MaKhieuNai",
                        column: x => x.MaKhieuNai,
                        principalTable: "KhieuNaiVPhams",
                        principalColumn: "MaKhieuNai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KhieuNaiVPhams_MaBienBan",
                table: "KhieuNaiVPhams",
                column: "MaBienBan");

            migrationBuilder.CreateIndex(
                name: "IX_BangChungKhieuNais_MaKhieuNai",
                table: "BangChungKhieuNais",
                column: "MaKhieuNai");

            migrationBuilder.AddForeignKey(
                name: "FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan",
                table: "KhieuNaiVPhams",
                column: "MaBienBan",
                principalTable: "BienBanVPhams",
                principalColumn: "MaBienBan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropTable(
                name: "BangChungKhieuNais");

            migrationBuilder.DropIndex(
                name: "IX_KhieuNaiVPhams_MaBienBan",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropColumn(
                name: "MaBienBan",
                table: "KhieuNaiVPhams");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTraLoi",
                table: "KhieuNaiVPhams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
