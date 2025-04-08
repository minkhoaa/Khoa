using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodify_DoAn.Migrations
{
    /// <inheritdoc />
    public partial class dbContext_modified2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoan_VaiTro_TaiKhoan_TaiKhoanId",
                table: "TaiKhoan_VaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoan_VaiTro_VaiTro_VaiTroId",
                table: "TaiKhoan_VaiTro");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoan_VaiTro_TaiKhoanId",
                table: "TaiKhoan_VaiTro");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoan_VaiTro_VaiTroId",
                table: "TaiKhoan_VaiTro");

            migrationBuilder.DropColumn(
                name: "TaiKhoanId",
                table: "TaiKhoan_VaiTro");

            migrationBuilder.DropColumn(
                name: "VaiTroId",
                table: "TaiKhoan_VaiTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaiKhoanId",
                table: "TaiKhoan_VaiTro",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VaiTroId",
                table: "TaiKhoan_VaiTro",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_VaiTro_TaiKhoanId",
                table: "TaiKhoan_VaiTro",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_VaiTro_VaiTroId",
                table: "TaiKhoan_VaiTro",
                column: "VaiTroId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoan_VaiTro_TaiKhoan_TaiKhoanId",
                table: "TaiKhoan_VaiTro",
                column: "TaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoan_VaiTro_VaiTro_VaiTroId",
                table: "TaiKhoan_VaiTro",
                column: "VaiTroId",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
