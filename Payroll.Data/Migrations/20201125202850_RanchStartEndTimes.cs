using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class RanchStartEndTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "RanchPayLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "RanchPayLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "RanchPayLines");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "RanchPayLines");
        }
    }
}
