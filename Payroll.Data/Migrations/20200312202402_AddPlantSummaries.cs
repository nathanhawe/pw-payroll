using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
	public partial class AddPlantSummaries : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "PlantSummaries",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					DateCreated = table.Column<DateTime>(nullable: false),
					DateModified = table.Column<DateTime>(nullable: false),
					IsDeleted = table.Column<bool>(nullable: false),
					BatchId = table.Column<int>(nullable: false),
					EmployeeId = table.Column<string>(nullable: true),
					WeekEndDate = table.Column<DateTime>(nullable: false),
					TotalHours = table.Column<decimal>(nullable: false),
					TotalGross = table.Column<decimal>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PlantSummaries", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "PlantSummaries");
		}
	}
}
