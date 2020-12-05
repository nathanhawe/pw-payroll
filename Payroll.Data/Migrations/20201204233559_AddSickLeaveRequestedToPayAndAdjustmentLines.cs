using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddSickLeaveRequestedToPayAndAdjustmentLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SickLeaveRequested",
                table: "RanchPayLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SickLeaveRequested",
                table: "RanchAdjustmentLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SickLeaveRequested",
                table: "PlantPayLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SickLeaveRequested",
                table: "PlantAdjustmentLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SickLeaveRequested",
                table: "RanchPayLines");

            migrationBuilder.DropColumn(
                name: "SickLeaveRequested",
                table: "RanchAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "SickLeaveRequested",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "SickLeaveRequested",
                table: "PlantAdjustmentLines");
        }
    }
}
