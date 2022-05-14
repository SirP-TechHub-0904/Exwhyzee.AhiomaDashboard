using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class checkoutUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Transactions");

            migrationBuilder.AddColumn<long>(
                name: "OrderItemId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TransactionId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderItemId",
                table: "Transactions",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_OrderItems_OrderItemId",
                table: "Transactions",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_OrderItems_OrderItemId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OrderItemId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "Transactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
