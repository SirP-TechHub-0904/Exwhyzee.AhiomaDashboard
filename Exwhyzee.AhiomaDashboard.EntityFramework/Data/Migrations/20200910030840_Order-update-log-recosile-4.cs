using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class Orderupdatelogrecosile4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackCartId",
                table: "ProductCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackCartId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackCartId",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "TrackCartId",
                table: "Orders");
        }
    }
}
