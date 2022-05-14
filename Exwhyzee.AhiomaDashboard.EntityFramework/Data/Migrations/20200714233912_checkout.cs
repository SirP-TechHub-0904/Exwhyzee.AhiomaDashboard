using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class checkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemSize",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Itemcolor",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemSize",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Itemcolor",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");
        }
    }
}
