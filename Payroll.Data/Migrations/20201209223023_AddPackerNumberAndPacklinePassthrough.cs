using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddPackerNumberAndPacklinePassthrough : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackerNumber",
                table: "PlantPayLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Packline",
                table: "PlantPayLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackerNumber",
                table: "PlantPayLines");

            migrationBuilder.DropColumn(
                name: "Packline",
                table: "PlantPayLines");
        }
    }
}
