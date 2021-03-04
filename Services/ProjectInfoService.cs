using Landing.API.Database;
using Landing.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class ProjectInfoService
    {
        private readonly LandingDbContext dbContext;

        public ProjectInfoService(LandingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<ProjectInfo>> GetPublicProjectInfos()
        {
            return await dbContext.ProjectInfos
                .Where(pi => pi.IsPublic)
                .Select(pi => pi.Info)
                .ToListAsync();
        }

        public async Task<bool> CommitIsActual(string repo, DateTimeOffset commitDate)
        {
            return !await dbContext.ProjectInfos
                .Where(pi => pi.Repo == repo)
                .Where(pi => pi.CommitDate >= commitDate)
                .AnyAsync();
        }

        public async Task AddProjectInfo(
            string repo,
            string commitSha,
            DateTimeOffset commitDate,
            ProjectInfo projectInfo)
        {
            var currentRepos = await dbContext.ProjectInfos
                .Where(pi => pi.Repo == repo)
                .AsNoTracking()
                .ToListAsync();
            var hidden = currentRepos.SingleOrDefault(i => !i.IsPublic);
            if (hidden != null)
            {
                hidden.Commit = commitSha;
                hidden.CommitDate = commitDate;
                dbContext.Update(hidden);
            } else
            {
                hidden = new ProjectInfoRecord
                {
                    Repo = repo,
                    Commit = commitSha,
                    CommitDate = commitDate,
                    IsPublic = false
                };
                dbContext.Add(hidden);
            }
            hidden.Info = projectInfo;
            await dbContext.SaveChangesAsync();
        }

        public async Task<(ProjectInfoRecord currentInfo, ProjectInfoRecord newInfo)> GetRecordsForRepo(string repo)
        {
            var records = await dbContext.ProjectInfos.AsNoTracking().Where(i => i.Repo == repo).ToListAsync();
            return (records.SingleOrDefault(r => r.IsPublic), records.SingleOrDefault(r => !r.IsPublic));
        }

        public async Task PublishInfo(string repo, int recordToPublishId)
        {
            var records = await dbContext.ProjectInfos.AsNoTracking().Where(i => i.Repo == repo).ToListAsync();
            var current = records.SingleOrDefault(r => r.IsPublic);
            if (current != null)
            {
                dbContext.Remove(current);
            }
            var newInfo = records.Single(r => r.Id == recordToPublishId);
            newInfo.IsPublic = true;
            dbContext.Update(newInfo);
            await dbContext.SaveChangesAsync();
        }
    }
}
