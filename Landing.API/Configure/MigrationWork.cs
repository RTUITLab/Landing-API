using Landing.API.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Landing.API.Configure
{
    public class MigrationWork : IConfigureWork
    {
        private readonly LandingDbContext landingDbContext;

        public MigrationWork(LandingDbContext landingDbContext)
        {
            this.landingDbContext = landingDbContext;
        }
        public Task Configure(CancellationToken cancellationToken)
        {
            return landingDbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
