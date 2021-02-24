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
        private readonly IMapper mapper;
        private readonly ProjectInfoService projectInfoService;
        private readonly IMemoryCache cache;
        public ProjectsController(
            ProjectInfoService projectInfoService,
            IMemoryCache cache,
            IMapper mapper)
        {
            this.projectInfoService = projectInfoService;
            this.cache = cache;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectInfoResponse>>> GetProjectsV11Async()
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
