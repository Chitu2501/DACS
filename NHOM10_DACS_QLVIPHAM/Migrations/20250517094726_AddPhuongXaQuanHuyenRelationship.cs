using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class AddPhuongXaQuanHuyenRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaQuanHuyen",
                table: "PhuongXas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiDonVi",
                table: "PhuongXas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "QuanHuyens",
                columns: table => new
                {
                    MaQuanHuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuanHuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiDonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHuyens", x => x.MaQuanHuyen);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhuongXas_MaQuanHuyen",
                table: "PhuongXas",
                column: "MaQuanHuyen");

            migrationBuilder.AddForeignKey(
                name: "FK_PhuongXas_QuanHuyens_MaQuanHuyen",
                table: "PhuongXas",
                column: "MaQuanHuyen",
                principalTable: "QuanHuyens",
                principalColumn: "MaQuanHuyen",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhuongXas_QuanHuyens_MaQuanHuyen",
                table: "PhuongXas");

            migrationBuilder.DropTable(
                name: "QuanHuyens");

            migrationBuilder.DropIndex(
                name: "IX_PhuongXas_MaQuanHuyen",
                table: "PhuongXas");

            migrationBuilder.AlterColumn<int>(
                name: "MaQuanHuyen",
                table: "PhuongXas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiDonVi",
                table: "PhuongXas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
