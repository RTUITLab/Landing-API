using Landing.API.Configure;
using Landing.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class ScrubProjectsService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IOptions<ScrubOptions> scrubOptions;
        private readonly ILogger<ScrubProjectsService> logger;

        public ScrubProjectsService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ScrubOptions> scrubOptions,
            ILogger<ScrubProjectsService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.scrubOptions = scrubOptions;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    using var scope = serviceScopeFactory.CreateScope();
                    var client = scope.ServiceProvider.GetRequiredService<GitHubClient>();
                    await ScrubProjects(client, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Service stopping");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled error on github scrab");
                }
                logger.LogInformation($"Wait to new scrub: {scrubOptions.Value.Delay}");
                await Task.Delay(scrubOptions.Value.Delay, stoppingToken);
            }
        }

        private async Task ScrubProjects(GitHubClient client, CancellationToken cancellationToken)
        {
            int i = 0;

            var reps = await client.Repository.GetAllForOrg("rtuitlab");
            foreach (var rep in reps)
            {
                cancellationToken.ThrowIfCancellationRequested();
                logger.LogInformation($"{i++}/{reps.Count}: {rep.FullName}");
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var projectsService = scope.ServiceProvider.GetRequiredService<ProjectInfoService>();
                    var repoHandlerService = scope.ServiceProvider.GetRequiredService<ScrubProjectInfoService>();

                    if (!rep.PushedAt.HasValue)
                    {
                        logger.LogInformation($"Skip {rep.FullName} no PushetAt property");
                        continue;
                    }

                    if (!await projectsService.CommitIsActual(rep.FullName, rep.PushedAt ?? DateTime.MinValue))
                    {
                        logger.LogInformation($"Skip {rep.FullName} too old commit");
                        continue;
                    }

                    var projectInfo = await repoHandlerService.ExtractInfoFromRepo(rep);

                    if (projectInfo != null)
                    {
                        await projectsService.AddProjectInfo(rep.FullName, projectInfo.CommitSha, rep.PushedAt.Value, projectInfo);
                    }
                }
                catch (ApiException ex) when (ex.Message?.Contains("is empty") == true)
                {
                    logger.LogWarning($"repository {rep.FullName} is empty");
                }
            }
        }
    }
}
