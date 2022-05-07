using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class PlantChanges2022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PositionTitle",
                table: "PlantPayLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositionTitle",
                table: "PlantAdjustmentLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionTitle",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "PositionTitle",
                table: "PlantAdjustmentLines");
        }
    }
}
