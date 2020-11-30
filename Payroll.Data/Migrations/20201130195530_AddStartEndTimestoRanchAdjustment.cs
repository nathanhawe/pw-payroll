using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddStartEndTimestoRanchAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "RanchAdjustmentLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "RanchAdjustmentLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "RanchAdjustmentLines");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "RanchAdjustmentLines");
        }
    }
}
