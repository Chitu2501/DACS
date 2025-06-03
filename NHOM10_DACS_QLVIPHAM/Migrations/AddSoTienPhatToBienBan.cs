using Microsoft.EntityFrameworkCore.Migrations;

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    public partial class AddSoTienPhatToBienBan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SoTienPhat",
                table: "BienBanVPhams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoTienPhat",
                table: "BienBanVPhams");
        }
    }
} 