using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Landing.API.Database.Migrations
{
    public partial class ContactUsDoneTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DoneTime",
                table: "ContactUsMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoneTime",
                table: "ContactUsMessages");
        }
    }
}
