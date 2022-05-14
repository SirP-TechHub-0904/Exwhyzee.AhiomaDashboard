using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class majorupdateRequestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RequestPhoneEmailChanges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "UserAddressId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserAddressId",
                table: "Orders",
                column: "UserAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserAddresses_UserAddressId",
                table: "Orders",
                column: "UserAddressId",
                principalTable: "UserAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserAddresses_UserAddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RequestPhoneEmailChanges");

            migrationBuilder.DropColumn(
                name: "UserAddressId",
                table: "Orders");
        }
    }
}
