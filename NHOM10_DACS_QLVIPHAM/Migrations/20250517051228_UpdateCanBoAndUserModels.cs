using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHOM10_DACS_QLVIPHAM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCanBoAndUserModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CanBos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanBos_UserId",
                table: "CanBos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanBos_AspNetUsers_UserId",
                table: "CanBos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanBos_AspNetUsers_UserId",
                table: "CanBos");

            migrationBuilder.DropIndex(
                name: "IX_CanBos_UserId",
                table: "CanBos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CanBos");
        }
    }
}
