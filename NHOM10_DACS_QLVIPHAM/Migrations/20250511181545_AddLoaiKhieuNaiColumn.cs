using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiKhieuNaiColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuongDanBangChung",
                table: "KhieuNaiVPhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "KhieuNaiVPhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiKhieuNai",
                table: "KhieuNaiVPhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LyDoYeuCau",
                table: "KhieuNaiVPhams",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuongDanBangChung",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropColumn(
                name: "LoaiKhieuNai",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropColumn(
                name: "LyDoYeuCau",
                table: "KhieuNaiVPhams");
        }
    }
}
