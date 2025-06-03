using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddYeuCauBangChungViPhamsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<int>(
                name: "MaCongDan",
                table: "ThongBaos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "YeuCauBangChungViPhams",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBienBan = table.Column<int>(type: "int", nullable: false),
                    MaCongDan = table.Column<int>(type: "int", nullable: false),
                    NgayYeuCau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanBangChung = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauBangChungViPhams", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauBangChungViPhams_BienBanVPhams_MaBienBan",
                        column: x => x.MaBienBan,
                        principalTable: "BienBanVPhams",
                        principalColumn: "MaBienBan",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_YeuCauBangChungViPhams_CongDans_MaCongDan",
                        column: x => x.MaCongDan,
                        principalTable: "CongDans",
                        principalColumn: "MaCongDan",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauBangChungViPhams_MaBienBan",
                table: "YeuCauBangChungViPhams",
                column: "MaBienBan");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauBangChungViPhams_MaCongDan",
                table: "YeuCauBangChungViPhams",
                column: "MaCongDan");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaos_CongDans_MaCongDan",
                table: "ThongBaos",
                column: "MaCongDan",
                principalTable: "CongDans",
                principalColumn: "MaCongDan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaos_CongDans_MaCongDan",
                table: "ThongBaos");

            migrationBuilder.DropTable(
                name: "YeuCauBangChungViPhams");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "ThongBaos",
                newName: "Role");

            migrationBuilder.AlterColumn<int>(
                name: "MaCongDan",
                table: "ThongBaos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaBienBan",
                table: "ThongBaos",
                column: "MaBienBan");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_UserId",
                table: "ThongBaos",
                column: "UserId");

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
    }
}
