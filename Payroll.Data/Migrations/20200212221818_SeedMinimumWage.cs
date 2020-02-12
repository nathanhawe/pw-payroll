using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Payroll.Data.Migrations
{
    public partial class SeedMinimumWage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MinimumWages",
                columns: new[] { "Id", "DateCreated", "DateModified", "EffectiveDate", "IsDeleted", "Wage" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 2, 12, 14, 18, 17, 937, DateTimeKind.Local).AddTicks(6431), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(319), new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 8m },
                    { 16, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2993), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2996), new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 13m },
                    { 15, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2988), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2990), new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 12m },
                    { 14, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2982), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2984), new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 11m },
                    { 13, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2976), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2979), new DateTime(2017, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10.5m },
                    { 12, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2970), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2973), new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10m },
                    { 11, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2964), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2967), new DateTime(2014, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 9m },
                    { 10, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2959), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2961), new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 8m },
                    { 9, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2953), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2955), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 15m },
                    { 8, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2947), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2949), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 14m },
                    { 7, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2941), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2944), new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 13m },
                    { 6, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2935), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2938), new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 12m },
                    { 5, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2930), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2932), new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 11m },
                    { 4, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2906), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2923), new DateTime(2017, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10.5m },
                    { 3, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2800), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2806), new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10m },
                    { 2, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2719), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2751), new DateTime(2014, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 9m },
                    { 17, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(2999), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3002), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 14m },
                    { 18, new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3005), new DateTime(2020, 2, 12, 14, 18, 17, 941, DateTimeKind.Local).AddTicks(3007), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 15m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MinimumWages",
                keyColumn: "Id",
                keyValue: 9);

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
        }
    }
}
