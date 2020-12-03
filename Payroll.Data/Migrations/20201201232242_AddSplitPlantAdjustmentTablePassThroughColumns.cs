using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddSplitPlantAdjustmentTablePassThroughColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoxStyle",
                table: "PlantAdjustmentLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BoxStyleDescription",
                table: "PlantAdjustmentLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "PlantAdjustmentLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "H2AHoursOffered",
                table: "PlantAdjustmentLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncentiveDisqualified",
                table: "PlantAdjustmentLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "PlantAdjustmentLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxStyle",
                table: "PlantAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "BoxStyleDescription",
                table: "PlantAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PlantAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "H2AHoursOffered",
                table: "PlantAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "IsIncentiveDisqualified",
                table: "PlantAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PlantAdjustmentLines");
        }
    }
}
