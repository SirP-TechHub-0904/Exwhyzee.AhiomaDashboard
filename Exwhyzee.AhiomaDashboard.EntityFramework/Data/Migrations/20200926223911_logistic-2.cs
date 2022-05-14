using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class logistic2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LGA",
                table: "LogisticVehicle",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LogisticVehicle",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleStatus",
                table: "LogisticVehicle",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LogisticStatus",
                table: "LogisticProfiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LGA",
                table: "LogisticVehicle");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LogisticVehicle");

            migrationBuilder.DropColumn(
                name: "VehicleStatus",
                table: "LogisticVehicle");

            migrationBuilder.DropColumn(
                name: "LogisticStatus",
                table: "LogisticProfiles");
        }
    }
}
