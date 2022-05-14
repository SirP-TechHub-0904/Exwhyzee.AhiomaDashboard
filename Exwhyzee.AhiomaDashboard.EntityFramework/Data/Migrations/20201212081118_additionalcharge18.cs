﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class additionalcharge18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrderCostAfterProcessing",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCostAfterProcessing",
                table: "Orders");
        }
    }
}
