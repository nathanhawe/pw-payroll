using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class UpdateAndSeedCrewBossWage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "CrewBossWages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "CrewBossWages",
                columns: new[] { "Id", "DateCreated", "DateModified", "EffectiveDate", "IsDeleted", "Wage", "WorkerCountThreshold" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 2, 12, 14, 38, 9, 772, DateTimeKind.Local).AddTicks(8416), new DateTime(2020, 2, 12, 14, 38, 9, 772, DateTimeKind.Local).AddTicks(8448), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 24.5m, 36 },
                    { 23, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1507), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1510), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 16m, 0 },
                    { 22, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1502), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1504), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 16.25m, 15 },
                    { 20, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1490), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1493), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 16.75m, 17 },
                    { 19, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1485), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1487), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 17m, 18 },
                    { 18, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1479), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1482), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 17.25m, 19 },
                    { 17, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1474), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1476), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 17.5m, 20 },
                    { 16, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1469), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1471), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 17.75m, 21 },
                    { 15, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1463), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1465), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 18m, 22 },
                    { 14, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1458), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1460), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 18.25m, 23 },
                    { 13, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1452), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1455), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 18.5m, 24 },
                    { 21, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1497), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1499), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 16.5m, 16 },
                    { 11, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1440), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1443), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 19.5m, 26 },
                    { 10, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1435), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1437), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 20m, 27 },
                    { 9, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1430), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1432), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 20.5m, 28 },
                    { 8, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1424), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1427), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 21m, 29 },
                    { 7, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1419), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1421), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 21.5m, 30 },
                    { 6, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1413), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1416), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 22m, 31 },
                    { 5, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1407), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1409), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 22.5m, 32 },
                    { 4, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1400), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1402), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 23m, 33 },
                    { 12, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1446), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1448), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 19m, 25 },
                    { 3, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1393), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1396), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 23.5m, 34 },
                    { 2, new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1244), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1261), new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 24m, 35 }
                });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 765, DateTimeKind.Local).AddTicks(7704), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(3283) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(5409), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(5430) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(5460), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(5463) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9177), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9197) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9203), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9205) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9208), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9211) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9214), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9216) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9219), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9221) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9225), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9227) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9230), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9233) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9236), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9238) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9241), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9244) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9247), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9249) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9252), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9255) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9258), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9260) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9263), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9265) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9268), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9271) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9274), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9276) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "CrewBossWages");

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 937, DateTimeKind.Local).AddTicks(6431), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(319) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2719), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2751) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2800), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2806) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2906), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2923) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2930), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2932) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2935), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2938) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2941), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2944) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2947), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2949) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2953), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2955) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2959), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2961) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2964), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2967) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2970), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2973) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2976), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2979) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2982), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2984) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2988), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2990) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2993), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2996) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2999), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3002) });

            migrationBuilder.UpdateData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3005), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3007) });
        }
    }
}
