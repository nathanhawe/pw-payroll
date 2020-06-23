using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class UpdateBatchToIncludeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Batches");

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                table: "Batches",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusMessage",
                table: "Batches",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "StatusMessage",
                table: "Batches");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
