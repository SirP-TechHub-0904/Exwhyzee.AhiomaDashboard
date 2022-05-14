using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class updateproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductColorId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductSizeId",
                table: "Products",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "UseColor",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseSize",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProductColor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemColor = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColor_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSize",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemSize = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSize_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ProductId",
                table: "ProductColor",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSize_ProductId",
                table: "ProductSize",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductColor");

            migrationBuilder.DropTable(
                name: "ProductSize");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UseColor",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UseSize",
                table: "Products");
        }
    }
}
