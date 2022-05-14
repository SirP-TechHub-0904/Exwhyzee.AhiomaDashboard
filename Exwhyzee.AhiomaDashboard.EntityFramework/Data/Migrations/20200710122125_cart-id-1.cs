using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class cartid1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartTempId",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemSize",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Itemcolor",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductCarts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartTempId",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "ItemSize",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "Itemcolor",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductCarts");
        }
    }
}
