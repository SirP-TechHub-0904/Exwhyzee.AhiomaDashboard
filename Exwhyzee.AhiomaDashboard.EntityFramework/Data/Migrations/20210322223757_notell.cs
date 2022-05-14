using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class notell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "ProductChecks");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "ProductChecks",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "ProductChecks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "ProfileId",
                table: "ProductChecks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
