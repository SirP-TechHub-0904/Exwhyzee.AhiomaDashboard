using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class adminpagemanage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminReferrals",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainReferalId = table.Column<string>(nullable: true),
                    SubReferalId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminReferrals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminShopReferrals",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainReferalId = table.Column<string>(nullable: true),
                    SubReferalId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminShopReferrals", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminReferrals");

            migrationBuilder.DropTable(
                name: "AdminShopReferrals");
        }
    }
}
