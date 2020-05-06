using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class FixEFCoreFixDecimalWarnings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "RanchPayLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "RanchAdjustmentLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrimaRate",
                table: "PlantPayLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NonPrimaRate",
                table: "PlantPayLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "IncreasedRate",
                table: "PlantPayLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "PlantAdjustmentLines",
                type: "decimal(18,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "RanchPayLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "RanchAdjustmentLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrimaRate",
                table: "PlantPayLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NonPrimaRate",
                table: "PlantPayLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");

            migrationBuilder.AlterColumn<decimal>(
                name: "IncreasedRate",
                table: "PlantPayLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PieceRate",
                table: "PlantAdjustmentLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,16)");
        }
    }
}
