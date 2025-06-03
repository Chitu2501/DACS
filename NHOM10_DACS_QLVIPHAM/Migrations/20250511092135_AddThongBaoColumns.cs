using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddThongBaoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuThanhToans_BienBanVPhams_MaBienBan",
                table: "LichSuThanhToans");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "ThongBaos",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "NgayThongBao",
                table: "ThongBaos",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "MaLichSu",
                table: "LichSuThanhToans",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "ThongBaos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaCongDan",
                table: "ThongBaos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiThongBao",
                table: "ThongBaos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DaDoc",
                table: "ThongBaos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkChiTiet",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaBienBan",
                table: "ThongBaos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ThongBaos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SoTien",
                table: "LichSuThanhToans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayLadDuBienBan",
                table: "BienBanVPhams",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaBienBan",
                table: "ThongBaos",
                column: "MaBienBan");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaCongDan",
                table: "ThongBaos",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_UserId",
                table: "ThongBaos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuThanhToans_BienBanVPhams_MaBienBan",
                table: "LichSuThanhToans",
                column: "MaBienBan",
                principalTable: "BienBanVPhams",
                principalColumn: "MaBienBan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaos_AspNetUsers_UserId",
                table: "ThongBaos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaos_BienBanVPhams_MaBienBan",
                table: "ThongBaos",
                column: "MaBienBan",
                principalTable: "BienBanVPhams",
                principalColumn: "MaBienBan");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaos_CongDans_MaCongDan",
                table: "ThongBaos",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuThanhToans_BienBanVPhams_MaBienBan",
                table: "LichSuThanhToans");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaos_AspNetUsers_UserId",
                table: "ThongBaos");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaos_BienBanVPhams_MaBienBan",
                table: "ThongBaos");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaos_CongDans_MaCongDan",
                table: "ThongBaos");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaos_MaBienBan",
                table: "ThongBaos");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaos_MaCongDan",
                table: "ThongBaos");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaos_UserId",
                table: "ThongBaos");

            migrationBuilder.DropColumn(
                name: "DaDoc",
                table: "ThongBaos");

            migrationBuilder.DropColumn(
                name: "LinkChiTiet",
                table: "ThongBaos");

            migrationBuilder.DropColumn(
                name: "MaBienBan",
                table: "ThongBaos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ThongBaos");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "ThongBaos",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "ThongBaos",
                newName: "NgayThongBao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LichSuThanhToans",
                newName: "MaLichSu");

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MaCongDan",
                table: "ThongBaos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiThongBao",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "SoTien",
                table: "LichSuThanhToans",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayLadDuBienBan",
                table: "BienBanVPhams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuThanhToans_BienBanVPhams_MaBienBan",
                table: "LichSuThanhToans",
                column: "MaBienBan",
                principalTable: "BienBanVPhams",
                principalColumn: "MaBienBan",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
