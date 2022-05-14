using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class mailin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisableAdsCrediting",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableAdsCrediting",
                table: "UserProfiles");
        }
    }
}
