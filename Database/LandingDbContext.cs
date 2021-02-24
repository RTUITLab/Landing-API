using Landing.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Landing.API.Database
{
    public class LandingDbContext : DbContext
    {
        public LandingDbContext(DbContextOptions<LandingDbContext> options): base(options)
        {
        }

        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public DbSet<ProjectInfoRecord> ProjectInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectInfoRecord>(pir =>
            {
                pir.Property(p => p.Info).HasColumnType("jsonb");

                // Only one public version of repo
                pir.HasIndex(p => new { p.Repo, p.IsPublic }).IsUnique();
            });
        }
    }
}
