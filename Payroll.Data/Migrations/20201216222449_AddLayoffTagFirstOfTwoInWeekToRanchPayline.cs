using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddLayoffTagFirstOfTwoInWeekToRanchPayline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLayoffTagFirstOfTwoInWeek",
                table: "RanchPayLines",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLayoffTagFirstOfTwoInWeek",
                table: "RanchPayLines");
        }
    }
}
