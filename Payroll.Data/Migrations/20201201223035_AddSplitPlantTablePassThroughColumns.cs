using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddSplitPlantTablePassThroughColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoxStyle",
                table: "PlantPayLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BoxStyleDescription",
                table: "PlantPayLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "PlantPayLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "H2AHoursOffered",
                table: "PlantPayLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "PlantPayLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxStyle",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "BoxStyleDescription",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "H2AHoursOffered",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PlantPayLines");
        }
    }
}
