using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class notellk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductChecks_ProductId",
                table: "ProductChecks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChecks_Products_ProductId",
                table: "ProductChecks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChecks_Products_ProductId",
                table: "ProductChecks");

            migrationBuilder.DropIndex(
                name: "IX_ProductChecks_ProductId",
                table: "ProductChecks");
        }
    }
}
