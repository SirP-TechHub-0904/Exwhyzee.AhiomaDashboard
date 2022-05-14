using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class queue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferQueues",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(nullable: false),
                    uid = table.Column<string>(nullable: true),
                    account_bank = table.Column<string>(nullable: true),
                    account_number = table.Column<string>(nullable: true),
                    fullname = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    IDNUmber = table.Column<string>(nullable: true),
                    amount = table.Column<int>(nullable: false),
                    narration = table.Column<string>(nullable: true),
                    currency = table.Column<string>(nullable: true),
                    reference = table.Column<string>(nullable: true),
                    callback_url = table.Column<string>(nullable: true),
                    debit_currency = table.Column<string>(nullable: true),
                    QueueStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferQueues", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferQueues");
        }
    }
}
