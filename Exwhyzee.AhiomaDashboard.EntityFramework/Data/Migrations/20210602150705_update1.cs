using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "UserProfiles",
                newName: "LastUserUpdated");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAdminUpdated",
                table: "UserProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "response",
                table: "TransferQueues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateProcessedByUser",
                table: "SoaRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSentToSoa",
                table: "SoaRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IdNumber",
                table: "SoaRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProcessedByUser",
                table: "SoaRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SentToSoaBy",
                table: "SoaRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoaRequestStatus",
                table: "SoaRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "UserProfileId",
                table: "SoaRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoaRequests_UserProfileId",
                table: "SoaRequests",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_SoaRequests_UserProfiles_UserProfileId",
                table: "SoaRequests",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoaRequests_UserProfiles_UserProfileId",
                table: "SoaRequests");

            migrationBuilder.DropIndex(
                name: "IX_SoaRequests_UserProfileId",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LastAdminUpdated",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "response",
                table: "TransferQueues");

            migrationBuilder.DropColumn(
                name: "DateProcessedByUser",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "DateSentToSoa",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedByUser",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "SentToSoaBy",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "SoaRequestStatus",
                table: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "SoaRequests");

            migrationBuilder.RenameColumn(
                name: "LastUserUpdated",
                table: "UserProfiles",
                newName: "LastUpdated");
        }
    }
}
