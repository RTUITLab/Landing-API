using Landing.API.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class ProjectsInfoCache
    {
        private ConcurrentDictionary<string, ProjectInfo> projects = new ConcurrentDictionary<string, ProjectInfo>();
        public List<ProjectInfo> GetAllProjects()
        {
            return projects.Values.ToList();
        }

        public void UpdateCache(string fullName, ProjectInfo projectInfo)
        {
            if (projectInfo is null)
            {
                projects.TryRemove(fullName, out _);
            }
            else
            {
                projects.AddOrUpdate(fullName, projectInfo, (k, v) => projectInfo);
            }
        }
    }
}
