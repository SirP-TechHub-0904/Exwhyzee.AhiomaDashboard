using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class ordertransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackCode",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupOrderId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GroupOrderId",
                table: "Orders");
        }
    }
}
