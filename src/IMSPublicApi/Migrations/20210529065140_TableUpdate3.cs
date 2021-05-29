using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagementSystem.PublicApi.Migrations
{
    public partial class TableUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Product_ProductId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_ProductId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "UserInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "UserInfo",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_ProductId",
                table: "UserInfo",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Product_ProductId",
                table: "UserInfo",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
