using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodify_DoAn.Migrations
{
    /// <inheritdoc />
    public partial class dbContext_modified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTCongThuc_CongThuc_CongThucMaCT",
                table: "CTCongThuc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTCongThuc_NguyenLieu_NguyenLieuMaNL",
                table: "CTCongThuc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaLuu_CongThuc_CongThucMaCT",
                table: "CTDaLuu");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaLuu_NguoiDung_NguoiDungMaND",
                table: "CTDaLuu");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaThich_CongThuc_CongThucMaCT",
                table: "CTDaThich");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaThich_NguoiDung_NguoiDungMaND",
                table: "CTDaThich");

            migrationBuilder.DropForeignKey(
                name: "FK_TheoDoi_NguoiDung_FollowedMaND",
                table: "TheoDoi");

            migrationBuilder.DropForeignKey(
                name: "FK_TheoDoi_NguoiDung_FollowerMaND",
                table: "TheoDoi");

            migrationBuilder.DropIndex(
                name: "IX_TheoDoi_FollowedMaND",
                table: "TheoDoi");

            migrationBuilder.DropIndex(
                name: "IX_TheoDoi_FollowerMaND",
                table: "TheoDoi");

            migrationBuilder.DropIndex(
                name: "IX_CTDaThich_CongThucMaCT",
                table: "CTDaThich");

            migrationBuilder.DropIndex(
                name: "IX_CTDaThich_NguoiDungMaND",
                table: "CTDaThich");

            migrationBuilder.DropIndex(
                name: "IX_CTDaLuu_CongThucMaCT",
                table: "CTDaLuu");

            migrationBuilder.DropIndex(
                name: "IX_CTDaLuu_NguoiDungMaND",
                table: "CTDaLuu");

            migrationBuilder.DropIndex(
                name: "IX_CTCongThuc_CongThucMaCT",
                table: "CTCongThuc");

            migrationBuilder.DropIndex(
                name: "IX_CTCongThuc_NguyenLieuMaNL",
                table: "CTCongThuc");

            migrationBuilder.DropColumn(
                name: "FollowedMaND",
                table: "TheoDoi");

            migrationBuilder.DropColumn(
                name: "FollowerMaND",
                table: "TheoDoi");

            migrationBuilder.DropColumn(
                name: "CongThucMaCT",
                table: "CTDaThich");

            migrationBuilder.DropColumn(
                name: "NguoiDungMaND",
                table: "CTDaThich");

            migrationBuilder.DropColumn(
                name: "CongThucMaCT",
                table: "CTDaLuu");

            migrationBuilder.DropColumn(
                name: "NguoiDungMaND",
                table: "CTDaLuu");

            migrationBuilder.DropColumn(
                name: "CongThucMaCT",
                table: "CTCongThuc");

            migrationBuilder.DropColumn(
                name: "NguyenLieuMaNL",
                table: "CTCongThuc");

            migrationBuilder.CreateIndex(
                name: "IX_TheoDoi_Followed_ID",
                table: "TheoDoi",
                column: "Followed_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaThich_MaCT",
                table: "CTDaThich",
                column: "MaCT");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaLuu_MaCT",
                table: "CTDaLuu",
                column: "MaCT");

            migrationBuilder.CreateIndex(
                name: "IX_CTCongThuc_MaNL",
                table: "CTCongThuc",
                column: "MaNL");

            migrationBuilder.AddForeignKey(
                name: "FK_CTCongThuc_CongThuc_MaCT",
                table: "CTCongThuc",
                column: "MaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTCongThuc_NguyenLieu_MaNL",
                table: "CTCongThuc",
                column: "MaNL",
                principalTable: "NguyenLieu",
                principalColumn: "MaNL",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaLuu_CongThuc_MaCT",
                table: "CTDaLuu",
                column: "MaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaLuu_NguoiDung_MaND",
                table: "CTDaLuu",
                column: "MaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaThich_CongThuc_MaCT",
                table: "CTDaThich",
                column: "MaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaThich_NguoiDung_MaND",
                table: "CTDaThich",
                column: "MaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TheoDoi_NguoiDung_Followed_ID",
                table: "TheoDoi",
                column: "Followed_ID",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TheoDoi_NguoiDung_Following_ID",
                table: "TheoDoi",
                column: "Following_ID",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTCongThuc_CongThuc_MaCT",
                table: "CTCongThuc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTCongThuc_NguyenLieu_MaNL",
                table: "CTCongThuc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaLuu_CongThuc_MaCT",
                table: "CTDaLuu");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaLuu_NguoiDung_MaND",
                table: "CTDaLuu");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaThich_CongThuc_MaCT",
                table: "CTDaThich");

            migrationBuilder.DropForeignKey(
                name: "FK_CTDaThich_NguoiDung_MaND",
                table: "CTDaThich");

            migrationBuilder.DropForeignKey(
                name: "FK_TheoDoi_NguoiDung_Followed_ID",
                table: "TheoDoi");

            migrationBuilder.DropForeignKey(
                name: "FK_TheoDoi_NguoiDung_Following_ID",
                table: "TheoDoi");

            migrationBuilder.DropIndex(
                name: "IX_TheoDoi_Followed_ID",
                table: "TheoDoi");

            migrationBuilder.DropIndex(
                name: "IX_CTDaThich_MaCT",
                table: "CTDaThich");

            migrationBuilder.DropIndex(
                name: "IX_CTDaLuu_MaCT",
                table: "CTDaLuu");

            migrationBuilder.DropIndex(
                name: "IX_CTCongThuc_MaNL",
                table: "CTCongThuc");

            migrationBuilder.AddColumn<int>(
                name: "FollowedMaND",
                table: "TheoDoi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FollowerMaND",
                table: "TheoDoi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CongThucMaCT",
                table: "CTDaThich",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NguoiDungMaND",
                table: "CTDaThich",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CongThucMaCT",
                table: "CTDaLuu",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NguoiDungMaND",
                table: "CTDaLuu",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CongThucMaCT",
                table: "CTCongThuc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NguyenLieuMaNL",
                table: "CTCongThuc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TheoDoi_FollowedMaND",
                table: "TheoDoi",
                column: "FollowedMaND");

            migrationBuilder.CreateIndex(
                name: "IX_TheoDoi_FollowerMaND",
                table: "TheoDoi",
                column: "FollowerMaND");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaThich_CongThucMaCT",
                table: "CTDaThich",
                column: "CongThucMaCT");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaThich_NguoiDungMaND",
                table: "CTDaThich",
                column: "NguoiDungMaND");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaLuu_CongThucMaCT",
                table: "CTDaLuu",
                column: "CongThucMaCT");

            migrationBuilder.CreateIndex(
                name: "IX_CTDaLuu_NguoiDungMaND",
                table: "CTDaLuu",
                column: "NguoiDungMaND");

            migrationBuilder.CreateIndex(
                name: "IX_CTCongThuc_CongThucMaCT",
                table: "CTCongThuc",
                column: "CongThucMaCT");

            migrationBuilder.CreateIndex(
                name: "IX_CTCongThuc_NguyenLieuMaNL",
                table: "CTCongThuc",
                column: "NguyenLieuMaNL");

            migrationBuilder.AddForeignKey(
                name: "FK_CTCongThuc_CongThuc_CongThucMaCT",
                table: "CTCongThuc",
                column: "CongThucMaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTCongThuc_NguyenLieu_NguyenLieuMaNL",
                table: "CTCongThuc",
                column: "NguyenLieuMaNL",
                principalTable: "NguyenLieu",
                principalColumn: "MaNL",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaLuu_CongThuc_CongThucMaCT",
                table: "CTDaLuu",
                column: "CongThucMaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaLuu_NguoiDung_NguoiDungMaND",
                table: "CTDaLuu",
                column: "NguoiDungMaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaThich_CongThuc_CongThucMaCT",
                table: "CTDaThich",
                column: "CongThucMaCT",
                principalTable: "CongThuc",
                principalColumn: "MaCT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTDaThich_NguoiDung_NguoiDungMaND",
                table: "CTDaThich",
                column: "NguoiDungMaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TheoDoi_NguoiDung_FollowedMaND",
                table: "TheoDoi",
                column: "FollowedMaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TheoDoi_NguoiDung_FollowerMaND",
                table: "TheoDoi",
                column: "FollowerMaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
