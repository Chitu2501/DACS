using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddNgayThanhToanColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayThanhToan",
                table: "BienBanVPhams",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayThanhToan",
                table: "BienBanVPhams");
        }
    }
}
