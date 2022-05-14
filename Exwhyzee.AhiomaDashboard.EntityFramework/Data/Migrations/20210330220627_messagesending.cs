using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class messagesending : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisableAhiaPay",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableAhiaPayTransfer",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableBankTransfer",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableBuy",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableDeposit",
                table: "UserProfiles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableAhiaPay",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DisableAhiaPayTransfer",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DisableBankTransfer",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DisableBuy",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DisableDeposit",
                table: "UserProfiles");
        }
    }
}
