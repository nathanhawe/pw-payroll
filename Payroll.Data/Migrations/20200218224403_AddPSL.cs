using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
	public partial class AddPSL : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "PaidSickLeaves",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					DateCreated = table.Column<DateTime>(nullable: false),
					DateModified = table.Column<DateTime>(nullable: false),
					IsDeleted = table.Column<bool>(nullable: false),
					BatchId = table.Column<int>(nullable: false),
					EmployeeId = table.Column<string>(nullable: true),
					ShiftDate = table.Column<DateTime>(nullable: false),
					Company = table.Column<string>(nullable: true),
					Hours = table.Column<decimal>(nullable: false),
					Gross = table.Column<decimal>(nullable: false),
					NinetyDayHours = table.Column<decimal>(nullable: false),
					NinetyDayGross = table.Column<decimal>(nullable: false),
					HoursUsed = table.Column<decimal>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PaidSickLeaves", x => x.Id);
				});

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

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "PaidSickLeaves");

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 1,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(5307), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(5336) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 2,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(7936), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(7957) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 3,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8034), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8041) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 4,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8047), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8052) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 5,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8057), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8062) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 6,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8068), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8073) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 7,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8078), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8085) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 8,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8090), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8212) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 9,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8218), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8223) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 10,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8229), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8233) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 11,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8239), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8243) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 12,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8249), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8253) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 13,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8259), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8264) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 14,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8269), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8274) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 15,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8279), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8284) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 16,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8289), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8294) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 17,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8300), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8304) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 18,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8310), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8315) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 19,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8321), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8325) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 20,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8331), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8335) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 21,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8341), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8345) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 22,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8351), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8355) });

			migrationBuilder.UpdateData(
				table: "CrewBossWages",
				keyColumn: "Id",
				keyValue: 23,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8361), new DateTime(2020, 2, 12, 14, 41, 13, 855, DateTimeKind.Local).AddTicks(8365) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 1,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 848, DateTimeKind.Local).AddTicks(5885), new DateTime(2020, 2, 12, 14, 41, 13, 852, DateTimeKind.Local).AddTicks(9333) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 2,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2788), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2827) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 3,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2877), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2882) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 4,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2963), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2970) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 5,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2976), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2980) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 6,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2987), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2991) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 7,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(2998), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(3003) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 8,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(3009), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(3014) });

			migrationBuilder.UpdateData(
				table: "MinimumWages",
				keyColumn: "Id",
				keyValue: 9,
				columns: new[] { "DateCreated", "DateModified" },
				values: new object[] { new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(3021), new DateTime(2020, 2, 12, 14, 41, 13, 853, DateTimeKind.Local).AddTicks(3026) });
		}
	}
}
