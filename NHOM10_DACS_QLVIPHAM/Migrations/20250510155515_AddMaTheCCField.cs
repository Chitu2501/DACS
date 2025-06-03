using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddMaTheCCField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaTheCC",
                table: "BienBanVPhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienPhat",
                table: "BienBanVPhams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "YeuCauCapNhatThongTins",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongDan = table.Column<int>(type: "int", nullable: true),
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgayYeuCau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDungCapNhat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThongTinCapNhat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauCapNhatThongTins", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauCapNhatThongTins_AspNetUsers_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YeuCauCapNhatThongTins_CongDans_MaCongDan",
                        column: x => x.MaCongDan,
                        principalTable: "CongDans",
                        principalColumn: "MaCongDan");
                });

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauCapNhatThongTins_MaCongDan",
                table: "YeuCauCapNhatThongTins",
                column: "MaCongDan");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauCapNhatThongTins_MaNguoiDung",
                table: "YeuCauCapNhatThongTins",
                column: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YeuCauCapNhatThongTins");

            migrationBuilder.DropColumn(
                name: "MaTheCC",
                table: "BienBanVPhams");

            migrationBuilder.DropColumn(
                name: "SoTienPhat",
                table: "BienBanVPhams");
        }
    }
}
