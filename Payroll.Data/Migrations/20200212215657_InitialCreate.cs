using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false),
                    Owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrewBossPayLines",
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
                    Crew = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: true),
                    PayMethod = table.Column<string>(nullable: true),
                    WorkerCount = table.Column<int>(nullable: false),
                    HoursWorked = table.Column<decimal>(nullable: false),
                    HourlyRate = table.Column<decimal>(nullable: false),
                    Gross = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewBossPayLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrewBossWages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Wage = table.Column<decimal>(nullable: false),
                    WorkerCountThreshold = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewBossWages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MinimumWages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Wage = table.Column<decimal>(nullable: false),
                    WageOrder = table.Column<int>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinimumWages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RanchPayLines",
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
                    Crew = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: true),
                    LaborCode = table.Column<int>(nullable: false),
                    BlockId = table.Column<int>(nullable: false),
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
                    AlternativeWorkWeek = table.Column<bool>(nullable: false),
                    FiveEight = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RanchPayLines", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "CrewBossPayLines");

            migrationBuilder.DropTable(
                name: "CrewBossWages");

            migrationBuilder.DropTable(
                name: "MinimumWages");

            migrationBuilder.DropTable(
                name: "RanchPayLines");
        }
    }
}
