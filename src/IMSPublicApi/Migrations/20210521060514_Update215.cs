using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagementSystem.PublicApi.Migrations
{
    public partial class Update215 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "IMSUser",
                newName: "OwnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "IMSUser",
                newName: "OwnerId");
        }
    }
}
