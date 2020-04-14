using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class UpdateBatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Batches",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LayoffId",
                table: "Batches",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WeekEndDate",
                table: "Batches",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "LayoffId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "WeekEndDate",
                table: "Batches");
        }
    }
}
