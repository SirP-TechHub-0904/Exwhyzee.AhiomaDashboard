using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class cart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TenantAddresses_TenantAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Batches_BatchId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TenantAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "QuantityMoved",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "QuantitySold",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TenantAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            migrationBuilder.AlterColumn<long>(
                name: "BatchId",
                table: "Purchases",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Orders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCarts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    UserProfileId = table.Column<long>(nullable: false),
                    CartStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCarts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCarts_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    UserProfileId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedItems_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishLists",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    UserProfileId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishLists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishLists_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCarts_ProductId",
                table: "ProductCarts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCarts_UserProfileId",
                table: "ProductCarts",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedItems_ProductId",
                table: "SavedItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedItems_UserProfileId",
                table: "SavedItems",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_ProductId",
                table: "WishLists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_UserProfileId",
                table: "WishLists",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Batches_BatchId",
                table: "Purchases",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Batches_BatchId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "ProductCarts");

            migrationBuilder.DropTable(
                name: "SavedItems");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Orders");

            migrationBuilder.AlterColumn<long>(
                name: "BatchId",
                table: "Purchases",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityMoved",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantitySold",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TenantAddressId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TenantAddressId",
                table: "Orders",
                column: "TenantAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TenantAddresses_TenantAddressId",
                table: "Orders",
                column: "TenantAddressId",
                principalTable: "TenantAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Batches_BatchId",
                table: "Purchases",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
