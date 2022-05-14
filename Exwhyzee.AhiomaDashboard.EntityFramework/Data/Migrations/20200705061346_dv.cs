using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class dv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarnerLink",
                table: "Banners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SoaRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    ShopAddress = table.Column<string>(nullable: false),
                    BusinessName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoaRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoaRequests");

            migrationBuilder.DropColumn(
                name: "BarnerLink",
                table: "Banners");
        }
    }
}
