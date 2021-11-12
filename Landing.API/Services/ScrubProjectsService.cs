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
                    var scrubProjectInfoService = scope.ServiceProvider.GetRequiredService<ScrubProjectInfoService>();
                    await ScrubProjects(client, scrubProjectInfoService, stoppingToken);
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

        private async Task ScrubProjects(GitHubClient client, ScrubProjectInfoService scrubProjectInfoService, CancellationToken cancellationToken)
        {
            int i = 0;

            var reps = await client.Repository.GetAllForOrg("rtuitlab");
            foreach (var rep in reps)
            {
                logger.LogInformation($"{i++}/{reps.Count}: {rep.FullName}");
                await scrubProjectInfoService.HandleRepo(rep, cancellationToken);
            }
        }
    }
}
