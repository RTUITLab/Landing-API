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
    }
}
