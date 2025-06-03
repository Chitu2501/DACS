using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    public partial class AddYeuCauCapNhatThongTin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YeuCauCapNhatThongTins",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongDan = table.Column<int>(nullable: true),
                    MaNguoiDung = table.Column<string>(nullable: true),
                    NgayYeuCau = table.Column<DateTime>(nullable: false),
                    NoiDungCapNhat = table.Column<string>(nullable: false),
                    ThongTinCapNhat = table.Column<string>(nullable: false),
                    TrangThai = table.Column<string>(nullable: false),
                    NgayXuLy = table.Column<DateTime>(nullable: true),
                    NguoiXuLy = table.Column<string>(nullable: true),
                    GhiChu = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauCapNhatThongTins", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauCapNhatThongTins_CongDans_MaCongDan",
                        column: x => x.MaCongDan,
                        principalTable: "CongDans",
                        principalColumn: "MaCongDan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauCapNhatThongTins_AspNetUsers_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YeuCauCapNhatThongTins");
        }
    }
} 