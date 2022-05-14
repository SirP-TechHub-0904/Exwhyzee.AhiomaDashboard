using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class Orderupdatelogrecosile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LogisticAmount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LogisticVehicleId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_LogisticVehicleId",
                table: "Orders",
                column: "LogisticVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_LogisticVehicle_LogisticVehicleId",
                table: "Orders",
                column: "LogisticVehicleId",
                principalTable: "LogisticVehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Orders_UserProfiles_UserProfileId",
            //    table: "Orders",
            //    column: "UserProfileId",
            //    principalTable: "UserProfiles",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_LogisticVehicle_LogisticVehicleId",
                table: "Orders");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Orders_UserProfiles_UserProfileId",
            //    table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_LogisticVehicleId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticVehicleId",
                table: "Orders");
        }
    }
}
