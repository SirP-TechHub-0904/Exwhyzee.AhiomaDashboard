using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class promo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "Transactions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "PromoCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Banner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    PromoCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoProducts_PromoCategories_PromoCategoryId",
                        column: x => x.PromoCategoryId,
                        principalTable: "PromoCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromoProducts_ProductId",
                table: "PromoProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoProducts_PromoCategoryId",
                table: "PromoProducts",
                column: "PromoCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PromoProducts");

            migrationBuilder.DropTable(
                name: "PromoCategories");

            migrationBuilder.AlterColumn<long>(
                name: "UserProfileId",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserProfiles_UserProfileId",
                table: "Transactions",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
