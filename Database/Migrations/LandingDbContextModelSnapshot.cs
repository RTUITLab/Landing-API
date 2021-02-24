﻿// <auto-generated />
using System;
using Landing.API.Database;
using Landing.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Landing.API.Database.Migrations
{
    [DbContext(typeof(LandingDbContext))]
    partial class LandingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Landing.API.Models.ContactUsMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SenderIp")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ContactUsMessages");
                });

            modelBuilder.Entity("Landing.API.Models.ProjectInfoRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Commit")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CommitDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<ProjectInfo>("Info")
                        .HasColumnType("jsonb");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<string>("Repo")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Repo", "IsPublic")
                        .IsUnique();

                    b.ToTable("ProjectInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
