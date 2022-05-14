using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class emailmessaging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Messagings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailContents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldString = table.Column<string>(nullable: true),
                    NewString = table.Column<string>(nullable: true),
                    MessagingId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailContents_Messagings_MessagingId",
                        column: x => x.MessagingId,
                        principalTable: "Messagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailProducts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    MessagingId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailProducts_Messagings_MessagingId",
                        column: x => x.MessagingId,
                        principalTable: "Messagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailContents_MessagingId",
                table: "MailContents",
                column: "MessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_MailProducts_MessagingId",
                table: "MailProducts",
                column: "MessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_MailProducts_ProductId",
                table: "MailProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailContents");

            migrationBuilder.DropTable(
                name: "MailProducts");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Messagings");
        }
    }
}
