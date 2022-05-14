using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class ordertransactions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BankAccountApproved",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "UserProfiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BankAccountApproved",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "UserProfiles");
        }
    }
}
