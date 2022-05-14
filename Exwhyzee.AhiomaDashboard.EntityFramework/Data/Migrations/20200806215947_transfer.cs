using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class transfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AhiaPayTransfers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DateOfTransfer = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TransferReference = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Log = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AhiaPayTransfers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AhiaPayTransfers");
        }
    }
}
