using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class updateproduct2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductColor_Products_ProductId",
                table: "ProductColor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSize_Products_ProductId",
                table: "ProductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSize",
                table: "ProductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColor",
                table: "ProductColor");

            migrationBuilder.RenameTable(
                name: "ProductSize",
                newName: "ProductSizes");

            migrationBuilder.RenameTable(
                name: "ProductColor",
                newName: "ProductColors");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSize_ProductId",
                table: "ProductSizes",
                newName: "IX_ProductSizes_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColor_ProductId",
                table: "ProductColors",
                newName: "IX_ProductColors_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColors",
                table: "ProductColors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColors_Products_ProductId",
                table: "ProductColors",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_Products_ProductId",
                table: "ProductSizes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductColors_Products_ProductId",
                table: "ProductColors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_Products_ProductId",
                table: "ProductSizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColors",
                table: "ProductColors");

            migrationBuilder.RenameTable(
                name: "ProductSizes",
                newName: "ProductSize");

            migrationBuilder.RenameTable(
                name: "ProductColors",
                newName: "ProductColor");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizes_ProductId",
                table: "ProductSize",
                newName: "IX_ProductSize_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColors_ProductId",
                table: "ProductColor",
                newName: "IX_ProductColor_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSize",
                table: "ProductSize",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColor",
                table: "ProductColor",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColor_Products_ProductId",
                table: "ProductColor",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSize_Products_ProductId",
                table: "ProductSize",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
