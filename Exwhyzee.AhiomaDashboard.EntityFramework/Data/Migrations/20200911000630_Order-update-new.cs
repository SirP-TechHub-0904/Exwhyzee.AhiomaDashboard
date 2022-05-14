using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class Orderupdatenew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackCartId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "TrackOrderId",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackOrderId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackOrderId",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "TrackOrderId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "TrackCartId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
