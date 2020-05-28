using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimaCompany.IDP.Migrations.IdentityDb
{
    public partial class AddEmailAndSecurityCodeToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("579171f0-e637-4f4d-8fca-ed9fb7c7dfc8"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("852b8fa2-8b0d-4c68-82de-9b5526c7060b"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("8ce67e9b-b61e-4000-b0ab-ff90af616f83"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("9f5ff0a9-c1d2-41ad-a788-c0fa45d48e82"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityCode",
                table: "Users",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SecurityCodeExpirationDate",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("d5d7c829-f7c1-4d66-9e0c-d120eb5dc889"), "c50dfe95-7744-43b6-9146-b8718bef47e9", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Frank" },
                    { new Guid("7c49fefc-153d-46f7-a162-1b893bd41d43"), "3c32db66-3038-4858-a858-c59c33e37359", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Underwood" },
                    { new Guid("777c0ef3-cc0e-47c1-88b9-e790cddcb17d"), "6f11f62d-15ff-4207-acaa-423ea4a73a7c", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Claire" },
                    { new Guid("afaea2d8-b49b-4eac-bf5d-cc4cddbbe253"), "0753b7b3-2aeb-4d9e-80c1-d1b9cb84d823", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Underwood" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "b897d5d2-8427-45a1-9075-6abd2c57f438");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "39eeb0fe-d425-48bf-a804-31412611d3b0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("777c0ef3-cc0e-47c1-88b9-e790cddcb17d"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("7c49fefc-153d-46f7-a162-1b893bd41d43"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("afaea2d8-b49b-4eac-bf5d-cc4cddbbe253"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("d5d7c829-f7c1-4d66-9e0c-d120eb5dc889"));

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityCodeExpirationDate",
                table: "Users");

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("579171f0-e637-4f4d-8fca-ed9fb7c7dfc8"), "b56d7c95-a762-4474-a9f3-46d932fe944b", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Frank" },
                    { new Guid("9f5ff0a9-c1d2-41ad-a788-c0fa45d48e82"), "fb8ffa6a-a0ea-496e-a14d-34c08298dc83", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Underwood" },
                    { new Guid("852b8fa2-8b0d-4c68-82de-9b5526c7060b"), "615c8036-e008-4eb9-be8c-057e6669c49b", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Claire" },
                    { new Guid("8ce67e9b-b61e-4000-b0ab-ff90af616f83"), "c3efee89-d382-4da6-8d39-1e272c81397f", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Underwood" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "2a302f8f-c763-419e-8d2f-b93692be89c7");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "85b103ed-2f29-4663-b728-04ba401f62aa");
        }
    }
}
