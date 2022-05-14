using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class cartid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCarts_UserProfiles_UserProfileId",
                table: "ProductCarts");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "ProductCarts",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ProductCarts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCarts_UserProfiles_UserProfileId",
                table: "ProductCarts",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCarts_UserProfiles_UserProfileId",
                table: "ProductCarts");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ProductCarts");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "ProductCarts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCarts_UserProfiles_UserProfileId",
                table: "ProductCarts",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
