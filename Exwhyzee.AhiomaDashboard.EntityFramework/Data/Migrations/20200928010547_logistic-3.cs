using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class logistic3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VehicleImage",
                table: "LogisticVehicle",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleWeight",
                table: "LogisticVehicle",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleImage",
                table: "LogisticVehicle");

            migrationBuilder.DropColumn(
                name: "VehicleWeight",
                table: "LogisticVehicle");
        }
    }
}
