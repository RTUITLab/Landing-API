﻿using Landing.API.Configure;
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
        private readonly IOptions<GitHubOptinos> githubOptions;
        private readonly ILogger<ScrubProjectsService> logger;

        public ScrubProjectsService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ScrubOptions> scrubOptions,
            IOptions<GitHubOptinos> githubOptions,
            ILogger<ScrubProjectsService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.scrubOptions = scrubOptions;
            this.githubOptions = githubOptions;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    var client = new GitHubClient(new ProductHeaderValue(githubOptions.Value.ProductHeader));
                    if (string.IsNullOrEmpty(githubOptions.Value.OAuthToken))
                    {
                        logger.LogWarning("Using github unauthenticated client with limits, please provide OAuthToken token in options");
                    }
                    else
                    {
                        client.Credentials = new Credentials(githubOptions.Value.OAuthToken);
                    }
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

                    var projectInfo = await HandleRepository(client, rep);

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

        private async Task<ProjectInfo> HandleRepository(GitHubClient client, Repository repo)
        {


            var br = repo.DefaultBranch;

            var mainReference = await client.Git.Reference.Get(repo.Id, $"refs/heads/{br}");

            var tree = await client.Git.Tree.Get(repo.Id, mainReference.Ref);

            var targetFile = tree.Tree.SingleOrDefault(f => f.Path == "LANDING.md");
            if (targetFile is null)
            {
                logger.LogInformation($"Skip {repo.FullName} no LANDING file");
                return null;
            }
            var content = await client.Git.Blob.Get(repo.Id, targetFile.Sha);
            var fileContent = content.Content;

            if (content.Encoding == EncodingType.Base64)
            {
                fileContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(content.Content));
            }

            try
            {
                var info = await new LandingFileParser(repo.FullName, repo.DefaultBranch).ParseAsync(fileContent);
                info.Date = repo.UpdatedAt.ToString("dd/MM/yyyy");
                info.CommitSha = mainReference.Object.Sha;
                return info;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Can't parse project file in {repo.FullName} {repo.DefaultBranch} {mainReference.Ref}");
                return null;
            }
        }
    }
}
