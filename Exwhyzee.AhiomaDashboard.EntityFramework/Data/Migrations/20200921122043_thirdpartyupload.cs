using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class thirdpartyupload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyUserId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductUploadShops",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUploadShops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductUploadShops_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductUploadShops_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductUploadShops_TenantId",
                table: "ProductUploadShops",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUploadShops_UserProfileId",
                table: "ProductUploadShops",
                column: "UserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUploadShops");

            migrationBuilder.DropColumn(
                name: "ThirdPartyUserId",
                table: "Products");
        }
    }
}
