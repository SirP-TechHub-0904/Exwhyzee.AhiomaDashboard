using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class RequestStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.AddColumn<int>(
                name: "EmailStatus",
                table: "RequestPhoneEmailChanges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhoneStatus",
                table: "RequestPhoneEmailChanges",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailStatus",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.DropColumn(
                name: "PhoneStatus",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RequestPhoneEmailChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
