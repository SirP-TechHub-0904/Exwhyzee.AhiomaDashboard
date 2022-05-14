using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class majorupdateRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RequestPhoneEmailChanges",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserProfileId",
                table: "RequestPhoneEmailChanges",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RequestPhoneEmailChanges_UserProfileId",
                table: "RequestPhoneEmailChanges",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestPhoneEmailChanges_UserProfiles_UserProfileId",
                table: "RequestPhoneEmailChanges",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestPhoneEmailChanges_UserProfiles_UserProfileId",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.DropIndex(
                name: "IX_RequestPhoneEmailChanges_UserProfileId",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "RequestPhoneEmailChanges");
        }
    }
}
