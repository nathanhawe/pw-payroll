using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
	public partial class UpdatePlantPayLineForGrossCalc : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "PieceRate",
				table: "PlantPayLines");

			migrationBuilder.AddColumn<decimal>(
				name: "GrossFromIncentive",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<bool>(
				name: "HasNonPrimaViolation",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<decimal>(
				name: "IncreasedRate",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<bool>(
				name: "IsH2A",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsIncentiveDisqualified",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<decimal>(
				name: "NonPrimaRate",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<decimal>(
				name: "PrimaRate",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<bool>(
				name: "UseIncreasedRate",
				table: "PlantPayLines",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "GrossFromIncentive",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "HasNonPrimaViolation",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "IncreasedRate",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "IsH2A",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "IsIncentiveDisqualified",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "NonPrimaRate",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "PrimaRate",
				table: "PlantPayLines");

			migrationBuilder.DropColumn(
				name: "UseIncreasedRate",
				table: "PlantPayLines");

			migrationBuilder.AddColumn<decimal>(
				name: "PieceRate",
				table: "PlantPayLines",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);
		}
	}
}
