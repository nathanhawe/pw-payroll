using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class IncentiveGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "RanchPayLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "RanchBonusPieceRates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "RanchPayLines");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "RanchBonusPieceRates");
        }
    }
}
