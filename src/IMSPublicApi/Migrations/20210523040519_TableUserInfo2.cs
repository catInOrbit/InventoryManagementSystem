using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagementSystem.PublicApi.Migrations
{
    public partial class TableUserInfo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IMSUser_Product_ProductId",
                table: "IMSUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IMSUser",
                table: "IMSUser");

            migrationBuilder.RenameTable(
                name: "IMSUser",
                newName: "UserInfo");

            migrationBuilder.RenameIndex(
                name: "IX_IMSUser_ProductId",
                table: "UserInfo",
                newName: "IX_UserInfo_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInfo",
                table: "UserInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Product_ProductId",
                table: "UserInfo",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Product_ProductId",
                table: "UserInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInfo",
                table: "UserInfo");

            migrationBuilder.RenameTable(
                name: "UserInfo",
                newName: "IMSUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserInfo_ProductId",
                table: "IMSUser",
                newName: "IX_IMSUser_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IMSUser",
                table: "IMSUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IMSUser_Product_ProductId",
                table: "IMSUser",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
