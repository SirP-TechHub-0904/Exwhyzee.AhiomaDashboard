using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class Logistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products");

            migrationBuilder.AlterColumn<long>(
                name: "TenantId",
                table: "Products",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredDate",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryRangeEnd",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryRangeStart",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LogisticProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CompanyDocument = table.Column<string>(nullable: true),
                    Referee = table.Column<string>(nullable: true),
                    CustomerCareNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticProfiles_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogisticVehicle",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleName = table.Column<string>(nullable: true),
                    VehiclePlateNumber = table.Column<string>(nullable: true),
                    VehiclePhoneNumber = table.Column<string>(nullable: true),
                    VehicleSize = table.Column<string>(nullable: true),
                    VehicleType = table.Column<string>(nullable: true),
                    LeastAmount = table.Column<decimal>(nullable: false),
                    LogisticProfileId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticVehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticVehicle_LogisticProfiles_LogisticProfileId",
                        column: x => x.LogisticProfileId,
                        principalTable: "LogisticProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogisticProfiles_UserId",
                table: "LogisticProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticProfiles_UserProfileId",
                table: "LogisticProfiles",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticVehicle_LogisticProfileId",
                table: "LogisticVehicle",
                column: "LogisticProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "LogisticVehicle");

            migrationBuilder.DropTable(
                name: "LogisticProfiles");

            migrationBuilder.DropColumn(
                name: "DeliveredDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryRangeEnd",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryRangeStart",
                table: "Orders");

            migrationBuilder.AlterColumn<long>(
                name: "TenantId",
                table: "Products",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
