using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class RemoveExtraMinimumWageSeeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 18);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 772, DateTimeKind.Local).AddTicks(8416), new DateTime(2020, 2, 12, 14, 38, 9, 772, DateTimeKind.Local).AddTicks(8448) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1244), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1261) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1393), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1396) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1400), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1402) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1407), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1409) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1413), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1416) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1419), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1421) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1424), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1427) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1430), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1432) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1435), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1437) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1440), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1443) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1446), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1448) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1452), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1455) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1458), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1460) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1463), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1465) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1469), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1471) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1474), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1476) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1479), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1482) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1485), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1487) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1490), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1493) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1497), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1499) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1502), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1504) });

            migrationBuilder.UpdateData(
                table: "CrewBossWages",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1507), new DateTime(2020, 2, 12, 14, 38, 9, 773, DateTimeKind.Local).AddTicks(1510) });

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

            migrationBuilder.InsertData(
                table: "MinimumWages",
                columns: new[] { "Id", "DateCreated", "DateModified", "EffectiveDate", "IsDeleted", "Wage" },
                values: new object[,]
                {
                    { 11, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9236), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9238), new DateTime(2014, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 9m },
                    { 12, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9241), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9244), new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10m },
                    { 13, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9247), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9249), new DateTime(2017, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10.5m },
                    { 18, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9274), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9276), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 15m },
                    { 15, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9258), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9260), new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 12m },
                    { 16, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9263), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9265), new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 13m },
                    { 17, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9268), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9271), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 14m },
                    { 14, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9252), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9255), new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 11m },
                    { 10, new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9230), new DateTime(2020, 2, 12, 14, 38, 9, 770, DateTimeKind.Local).AddTicks(9233), new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 8m }
                });
        }
    }
}
