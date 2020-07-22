using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddCrewLaborRateForMinimumAssuranceFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseCrewLaborRateForMinimumAssurance",
                table: "PlantPayLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseCrewLaborRateForMinimumAssurance",
                table: "PlantAdjustmentLines",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCrewLaborRateForMinimumAssurance",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "UseCrewLaborRateForMinimumAssurance",
                table: "PlantAdjustmentLines");
        }
    }
}
