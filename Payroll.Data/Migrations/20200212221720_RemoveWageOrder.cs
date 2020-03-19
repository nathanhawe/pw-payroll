using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
	public partial class RemoveWageOrder : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "WageOrder",
				table: "MinimumWages");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "WageOrder",
				table: "MinimumWages",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}
	}
}
