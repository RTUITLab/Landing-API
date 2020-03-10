using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Landing.API.Database.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SendTime",
                table: "ContactUsMessages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SenderIp",
                table: "ContactUsMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendTime",
                table: "ContactUsMessages");

            migrationBuilder.DropColumn(
                name: "SenderIp",
                table: "ContactUsMessages");
        }
    }
}
