using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class Orderupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateOfTransaction",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GroupOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemSize",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Itemcolor",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopStatus",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "Vat");

            migrationBuilder.AddColumn<string>(
                name: "LogisticType",
                table: "Products",
                nullable: true);

           

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOrder",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

          

            migrationBuilder.AddColumn<decimal>(
                name: "OrderAmount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    ShopStatus = table.Column<int>(nullable: false),
                    LogisticStatus = table.Column<int>(nullable: false),
                    CustomerStatus = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ItemSize = table.Column<string>(nullable: true),
                    Itemcolor = table.Column<string>(nullable: true),
                    LogisticType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

           
            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItem",
                column: "ProductId");

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "OrderItem");

           

            migrationBuilder.DropColumn(
                name: "LogisticType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateOfOrder",
                table: "Orders");

          

            migrationBuilder.DropColumn(
                name: "OrderAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Vat",
                table: "Orders",
                newName: "TotalAmount");

           

            migrationBuilder.AddColumn<int>(
                name: "CustomerStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfTransaction",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GroupOrderId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemSize",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Itemcolor",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LogisticId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "LogisticStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShopStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
