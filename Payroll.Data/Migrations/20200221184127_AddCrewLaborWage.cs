using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class AddCrewLaborWage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrewLaborWages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    Wage = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewLaborWages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "CrewLaborWages",
                columns: new[] { "Id", "DateCreated", "DateModified", "EffectiveDate", "IsDeleted", "Wage" },
                values: new object[,]
                {
                    { 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 14m },
                    { 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 13m }
                });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrewLaborWages");

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(7374), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(7396) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(8994), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9011) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9043), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9046) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9049), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9052) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9055), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9058) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9061), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9063) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9066), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9068) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9072), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9074) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9077), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9080) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9083), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9085) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9089), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9091) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9094), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9097) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9100), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9102) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9105), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9108) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9111), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9113) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9116), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9118) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9121), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9124) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9127), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9129) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9132), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9134) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9137), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9140) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9143), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9145) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9149), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9151) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9154), new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9156) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 140, DateTimeKind.Local).AddTicks(1757), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(3530) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5542), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5564) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5598), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5601) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5653), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5657) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5660), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5662) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5665), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5668) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5671), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5673) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5677), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5679) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5683), new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5686) });
        }
    }
}
