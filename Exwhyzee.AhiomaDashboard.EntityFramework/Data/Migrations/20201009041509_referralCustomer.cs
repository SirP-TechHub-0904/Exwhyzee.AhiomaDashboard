using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class referralCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerRef",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerRef",
                table: "OrderItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CustomerRef",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerRef",
                table: "OrderItems");
        }
    }
}
