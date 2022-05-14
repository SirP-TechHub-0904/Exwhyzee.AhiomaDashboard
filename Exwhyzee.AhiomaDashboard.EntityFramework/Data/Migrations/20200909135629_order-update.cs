using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class orderupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerStatus",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

          

            migrationBuilder.AddColumn<long>(
                name: "LogisticId",
                table: "Orders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "LogisticStatus",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShopStatus",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CustomerStatus",
                table: "Orders");

           

            migrationBuilder.DropColumn(
                name: "LogisticId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopStatus",
                table: "Orders");
        }
    }
}
