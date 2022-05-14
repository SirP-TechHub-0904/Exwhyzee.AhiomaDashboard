using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class majorupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityAnswer",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityQuestion",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "UserAddresses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequestPhoneEmailChanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldMail = table.Column<string>(nullable: true),
                    NewMail = table.Column<string>(nullable: true),
                    OldPhone = table.Column<string>(nullable: true),
                    NewPhone = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    SecurityAnswer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestPhoneEmailChanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    WalletId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<string>(nullable: true),
                    ProfileId = table.Column<long>(nullable: true),
                    LedgerBalance = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Destination = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletHistories_UserProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletHistories_ProfileId",
                table: "WalletHistories",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestPhoneEmailChanges");

            migrationBuilder.DropTable(
                name: "WalletHistories");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SecurityAnswer",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SecurityQuestion",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Default",
                table: "UserAddresses");
        }
    }
}
