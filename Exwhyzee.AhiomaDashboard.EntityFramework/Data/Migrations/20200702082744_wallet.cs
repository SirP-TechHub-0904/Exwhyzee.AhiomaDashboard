using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class wallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    DateOfTransaction = table.Column<DateTime>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TransactionReference = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
