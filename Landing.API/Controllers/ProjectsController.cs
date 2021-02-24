using AutoMapper;
using Landing.API.Models;
using Landing.API.PublicAPI.Responses;
using Landing.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsInfoCache projectsCache;
        private readonly IMapper mapper;
        private IMemoryCache cache;
        public ProjectsController(
            ProjectsInfoCache projectsCache,
            IMemoryCache cache,
            IMapper mapper)
        {
            this.projectsCache = projectsCache;
            this.cache = cache;
            this.mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<ProjectInfoResponse>> GetProjects()
        {
            return projectsCache.GetAllProjects().Select((p, i) =>
                {
                    var mapped = mapper.Map<ProjectInfoResponse>(p);
                    mapped.Id = i;
                    return mapped;
                })
                .ToList();
        }
        [HttpGet("v1.1")]
        public async Task<ActionResult<IEnumerable<ProjectInfoResponse>>> GetProjectsV11Async([FromServices] ProjectInfoService projectInfoService)
        {
            var projectInfos = await cache.GetOrCreateAsync("PROJECTS", async (entry) =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                var projects = await projectInfoService.GetPublicProjectInfos();
                return projects;
            });
            return projectInfos.Select((p, i) =>
                {
                    var mapped = mapper.Map<ProjectInfoResponse>(p);
                    mapped.Id = i;
                    return mapped;
                })
                .ToList();
        }
    }
}
