using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddSouthCrewBossWage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SouthCrewBossWages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    Wage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkerCountThreshold = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SouthCrewBossWages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SouthCrewBossWages",
                columns: new[] { "Id", "DateCreated", "DateModified", "EffectiveDate", "IsDeleted", "Wage", "WorkerCountThreshold" },
                values: new object[,]
                {
                    { 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 24.75m, 19 },
                    { 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 24.25m, 18 },
                    { 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 23.75m, 17 },
                    { 4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 23.25m, 16 },
                    { 5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 22.75m, 15 },
                    { 6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 17.90m, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SouthCrewBossWages");
        }
    }
}
