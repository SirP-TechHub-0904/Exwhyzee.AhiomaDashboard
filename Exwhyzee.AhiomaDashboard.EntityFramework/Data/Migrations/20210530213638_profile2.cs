using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class profile2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserProfileId",
                table: "Transactions",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserProfileId",
                table: "Transactions",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserProfileId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Transactions");
        }
    }
}
