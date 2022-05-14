using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class note : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Products",
                maxLength: 50000000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                defaultValue: "Full Description",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductChecks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductChecks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductChecks");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Full Description",
                oldClrType: typeof(string),
                oldMaxLength: 50000000);
        }
    }
}
