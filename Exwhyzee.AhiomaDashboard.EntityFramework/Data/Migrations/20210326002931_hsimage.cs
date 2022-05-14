using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class hsimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageSize",
                table: "ProductPictures",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImageSize",
                table: "ProductPictures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSize",
                table: "ProductPictures");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageSize",
                table: "ProductPictures");
        }
    }
}
