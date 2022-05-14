using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class RequestStatusdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletHistories_UserProfiles_ProfileId",
                table: "WalletHistories");

            migrationBuilder.DropIndex(
                name: "IX_WalletHistories_ProfileId",
                table: "WalletHistories");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "WalletHistories");

            migrationBuilder.AlterColumn<long>(
                name: "WalletId",
                table: "WalletHistories",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "WalletHistories",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "WalletHistories",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "WalletHistories",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_WalletHistories_UserProfileId",
                table: "WalletHistories",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletHistories_UserProfiles_UserProfileId",
                table: "WalletHistories",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletHistories_UserProfiles_UserProfileId",
                table: "WalletHistories");

            migrationBuilder.DropIndex(
                name: "IX_WalletHistories_UserProfileId",
                table: "WalletHistories");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WalletHistories");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "WalletHistories");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderItems");

            migrationBuilder.AlterColumn<string>(
                name: "WalletId",
                table: "WalletHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "UserProfileId",
                table: "WalletHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "ProfileId",
                table: "WalletHistories",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletHistories_ProfileId",
                table: "WalletHistories",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletHistories_UserProfiles_ProfileId",
                table: "WalletHistories",
                column: "ProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
