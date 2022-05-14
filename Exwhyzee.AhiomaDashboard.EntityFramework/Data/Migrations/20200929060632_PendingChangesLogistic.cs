using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class PendingChangesLogistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders");
        }
    }
}
