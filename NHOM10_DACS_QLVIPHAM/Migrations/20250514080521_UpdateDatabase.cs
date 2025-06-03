using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LichSuThanhToans",
                newName: "MaLichSu");

            migrationBuilder.AddColumn<string>(
                name: "AnhMatSau",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnhMatTruoc",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuocTich",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "TheCanCuocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PhuongXas",
                columns: table => new
                {
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongXa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaQuanHuyen = table.Column<int>(type: "int", nullable: true),
                    LoaiDonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongXas", x => x.MaPhuongXa);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhuongXas");

            migrationBuilder.DropColumn(
                name: "AnhMatSau",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "AnhMatTruoc",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "QuocTich",
                table: "TheCanCuocs");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "TheCanCuocs");

            migrationBuilder.RenameColumn(
                name: "MaLichSu",
                table: "LichSuThanhToans",
                newName: "Id");
        }
    }
}
