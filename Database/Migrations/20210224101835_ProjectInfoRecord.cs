using System;
using Landing.API.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Landing.API.Database.Migrations
{
    public partial class ProjectInfoRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Repo = table.Column<string>(nullable: true),
                    Commit = table.Column<string>(nullable: true),
                    CommitDate = table.Column<DateTimeOffset>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    Info = table.Column<ProjectInfo>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInfos_Repo_IsPublic",
                table: "ProjectInfos",
                columns: new[] { "Repo", "IsPublic" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectInfos");
        }
    }
}
