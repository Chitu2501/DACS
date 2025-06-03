using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayPhatHanh",
                table: "TheCanCuocs");

            migrationBuilder.RenameColumn(
                name: "GIOTTING",
                table: "CongDans",
                newName: "GioiTinh");

            migrationBuilder.AlterColumn<string>(
                name: "TenViPham",
                table: "ViPhams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayHetHan",
                table: "TheCanCuocs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCap",
                table: "TheCanCuocs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiCap",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaNguoiDung",
                table: "ThanhToanViPhams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NguoiThucHien",
                table: "NhatKyThongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiDungHoatDong",
                table: "NhatKyThongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MucPhatToiDa",
                table: "LoaiViPhams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MucPhatToiThieu",
                table: "LoaiViPhams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "MaNguoiDung",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MaNguoiDung",
                table: "CongDans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoaDonMaHoaDon",
                table: "ChiTietHoaDons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauCapLaiThes_MaCongDan",
                table: "YeuCauCapLaiThes",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_KhieuNaiVPhams_MaCongDan",
                table: "KhieuNaiVPhams",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaCongDan",
                table: "HoaDons",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_CongDans_MaNguoiDung",
                table: "CongDans",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_HoaDonMaHoaDon",
                table: "ChiTietHoaDons",
                column: "HoaDonMaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_BienBanVPhams_MaCongDan",
                table: "BienBanVPhams",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_BienBanVPhams_MaViPham",
                table: "BienBanVPhams",
                column: "MaViPham");

            migrationBuilder.AddForeignKey(
                name: "FK_BienBanVPhams_CongDans_MaCongDan",
                table: "BienBanVPhams",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BienBanVPhams_ViPhams_MaViPham",
                table: "BienBanVPhams",
                column: "MaViPham",
                principalTable: "ViPhams",
                principalColumn: "MaViPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_HoaDonMaHoaDon",
                table: "ChiTietHoaDons",
                column: "HoaDonMaHoaDon",
                principalTable: "HoaDons",
                principalColumn: "MaHoaDon");

            migrationBuilder.AddForeignKey(
                name: "FK_CongDans_AspNetUsers_MaNguoiDung",
                table: "CongDans",
                column: "MaNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_CongDans_MaCongDan",
                table: "HoaDons",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KhieuNaiVPhams_CongDans_MaCongDan",
                table: "KhieuNaiVPhams",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YeuCauCapLaiThes_CongDans_MaCongDan",
                table: "YeuCauCapLaiThes",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BienBanVPhams_CongDans_MaCongDan",
                table: "BienBanVPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_BienBanVPhams_ViPhams_MaViPham",
                table: "BienBanVPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_HoaDonMaHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_CongDans_AspNetUsers_MaNguoiDung",
                table: "CongDans");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_CongDans_MaCongDan",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_KhieuNaiVPhams_CongDans_MaCongDan",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_YeuCauCapLaiThes_CongDans_MaCongDan",
                table: "YeuCauCapLaiThes");

            migrationBuilder.DropIndex(
                name: "IX_YeuCauCapLaiThes_MaCongDan",
                table: "YeuCauCapLaiThes");

            migrationBuilder.DropIndex(
                name: "IX_KhieuNaiVPhams_MaCongDan",
                table: "KhieuNaiVPhams");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_MaCongDan",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_CongDans_MaNguoiDung",
                table: "CongDans");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDons_HoaDonMaHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_BienBanVPhams_MaCongDan",
                table: "BienBanVPhams");

            migrationBuilder.DropIndex(
                name: "IX_BienBanVPhams_MaViPham",
                table: "BienBanVPhams");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "NgayCap",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "NoiCap",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "NguoiThucHien",
                table: "NhatKyThongs");

            migrationBuilder.DropColumn(
                name: "NoiDungHoatDong",
                table: "NhatKyThongs");

            migrationBuilder.DropColumn(
                name: "MucPhatToiDa",
                table: "LoaiViPhams");

            migrationBuilder.DropColumn(
                name: "MucPhatToiThieu",
                table: "LoaiViPhams");

            migrationBuilder.DropColumn(
                name: "MaNguoiDung",
                table: "CongDans");

            migrationBuilder.DropColumn(
                name: "HoaDonMaHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.RenameColumn(
                name: "GioiTinh",
                table: "CongDans",
                newName: "GIOTTING");

            migrationBuilder.AlterColumn<string>(
                name: "TenViPham",
                table: "ViPhams",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayHetHan",
                table: "TheCanCuocs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayPhatHanh",
                table: "TheCanCuocs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "MaNguoiDung",
                table: "ThanhToanViPhams",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MaNguoiDung",
                table: "HoaDons",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
