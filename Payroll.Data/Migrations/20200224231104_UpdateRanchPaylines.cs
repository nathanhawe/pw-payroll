using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class UpdateRanchPaylines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EmployeeHourlyRate",
                table: "RanchPayLines",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRateOverride",
                table: "RanchPayLines",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGross",
                table: "RanchPayLines",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeHourlyRate",
                table: "RanchPayLines");

            migrationBuilder.DropColumn(
                name: "HourlyRateOverride",
                table: "RanchPayLines");

            migrationBuilder.DropColumn(
                name: "TotalGross",
                table: "RanchPayLines");
        }
    }
}
