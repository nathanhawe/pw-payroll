using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
	public partial class AddPlantPayLine : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "PlantPayLines",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					DateCreated = table.Column<DateTime>(nullable: false),
					DateModified = table.Column<DateTime>(nullable: false),
					IsDeleted = table.Column<bool>(nullable: false),
					BatchId = table.Column<int>(nullable: false),
					LayoffId = table.Column<int>(nullable: false),
					QuickBaseRecordId = table.Column<int>(nullable: false),
					WeekEndDate = table.Column<DateTime>(nullable: false),
					ShiftDate = table.Column<DateTime>(nullable: false),
					Plant = table.Column<int>(nullable: false),
					EmployeeId = table.Column<string>(nullable: true),
					LaborCode = table.Column<int>(nullable: false),
					HoursWorked = table.Column<decimal>(nullable: false),
					PayType = table.Column<string>(nullable: true),
					Pieces = table.Column<decimal>(nullable: false),
					PieceRate = table.Column<decimal>(nullable: false),
					HourlyRate = table.Column<decimal>(nullable: false),
					OtDtWotRate = table.Column<decimal>(nullable: false),
					OtDtWotHours = table.Column<decimal>(nullable: false),
					GrossFromHours = table.Column<decimal>(nullable: false),
					GrossFromPieces = table.Column<decimal>(nullable: false),
					OtherGross = table.Column<decimal>(nullable: false),
					TotalGross = table.Column<decimal>(nullable: false),
					AlternativeWorkWeek = table.Column<bool>(nullable: false),
					HourlyRateOverride = table.Column<decimal>(nullable: false),
					EmployeeHourlyRate = table.Column<decimal>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PlantPayLines", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "PlantPayLines");
		}
	}
}
