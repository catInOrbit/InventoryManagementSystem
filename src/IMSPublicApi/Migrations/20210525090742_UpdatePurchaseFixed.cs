using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagementSystem.PublicApi.Migrations
{
    public partial class UpdatePurchaseFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestTransactions");

            migrationBuilder.DropTable(
                name: "TransactionCommons");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrackingNumber = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SupplierId",
                table: "Transactions",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.CreateTable(
                name: "TransactionCommons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCommons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TrackingNumber = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionCommonId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestTransactions_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestTransactions_TransactionCommons_TransactionCommonId",
                        column: x => x.TransactionCommonId,
                        principalTable: "TransactionCommons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestTransactions_SupplierId",
                table: "PurchaseRequestTransactions",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestTransactions_TransactionCommonId",
                table: "PurchaseRequestTransactions",
                column: "TransactionCommonId",
                unique: true,
                filter: "[TransactionCommonId] IS NOT NULL");
        }
    }
}
