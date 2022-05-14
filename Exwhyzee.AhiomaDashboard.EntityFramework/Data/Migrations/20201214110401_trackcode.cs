using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class trackcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackCode",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackCode",
                table: "OrderItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackCode",
                table: "OrderItems");
        }
    }
}
