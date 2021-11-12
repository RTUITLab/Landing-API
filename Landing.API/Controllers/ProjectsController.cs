using AutoMapper;
using Landing.API.Models;
using Landing.API.PublicAPI.Responses;
using Landing.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Landing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILogger<ProjectsController> logger;
        private readonly ProjectInfoService projectInfoService;
        private readonly IMemoryCache cache;
        public ProjectsController(
            ProjectInfoService projectInfoService,
            IMemoryCache cache,
            IMapper mapper,
            ILogger<ProjectsController> logger)
        {
            this.projectInfoService = projectInfoService;
            this.cache = cache;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectInfoResponse>>> GetProjectsV11Async()
        {
            var projectInfos = await cache.GetOrCreateAsync("PROJECTS", async (entry) =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return await projectInfoService.GetPublicProjectInfos();
            });
            return projectInfos.Select((p, i) =>
                {
                    var mapped = mapper.Map<ProjectInfoResponse>(p);
                    mapped.Id = i;
                    return mapped;
                })
                .ToList();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProjectInfo(
            [FromBody] string fullname,
            [FromServices] ScrubProjectInfoService projectInfoService,
            [FromServices] IOptions<LandingOptions> options,
            CancellationToken cancellationToken)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var values))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Require 'Authorization' header with correct secret token");
            }
            else if (values.ToString() != options.Value.AdminPanelAccessToken)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Require correct 'Authorization' header");
            }
            try
            {
                await projectInfoService.HandleRepo(fullname, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                logger.LogInformation(ex, "Incorrect argument");
                return BadRequest($"Incorrect {nameof(fullname)}");
            }
        }
    }
}
