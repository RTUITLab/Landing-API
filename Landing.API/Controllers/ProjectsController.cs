using AutoMapper;
using Landing.API.PublicAPI.Responses;
using Landing.API.Services;
using Microsoft.AspNetCore.Mvc;
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

        public ProjectsController(
            ProjectsInfoCache projectsCache,
            IMapper mapper)
        {
            this.projectsCache = projectsCache;
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
    }
}
