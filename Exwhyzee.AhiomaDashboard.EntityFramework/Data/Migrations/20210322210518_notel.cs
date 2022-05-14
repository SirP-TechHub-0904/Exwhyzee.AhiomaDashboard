using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class notel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 50000000,
                oldDefaultValue: "Full Description");

            migrationBuilder.AddColumn<long>(
                name: "ProfileId",
                table: "ProductChecks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "ProductChecks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserProfileId",
                table: "ProductChecks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductChecks_TenantId",
                table: "ProductChecks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductChecks_UserProfileId",
                table: "ProductChecks",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChecks_Tenants_TenantId",
                table: "ProductChecks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChecks_Tenants_TenantId",
                table: "ProductChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductChecks_UserProfiles_UserProfileId",
                table: "ProductChecks");

            migrationBuilder.DropIndex(
                name: "IX_ProductChecks_TenantId",
                table: "ProductChecks");

            migrationBuilder.DropIndex(
                name: "IX_ProductChecks_UserProfileId",
                table: "ProductChecks");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "ProductChecks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductChecks");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "ProductChecks");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 50000000,
                nullable: false,
                defaultValue: "Full Description",
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
