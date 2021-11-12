using Landing.API.Models;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class ScrubProjectInfoService
    {
        private readonly GitHubClient client;
        private readonly ProjectInfoService projectInfoService;
        private readonly ILogger<ScrubProjectInfoService> logger;

        public ScrubProjectInfoService(GitHubClient client,
            ProjectInfoService projectInfoService,
            ILogger<ScrubProjectInfoService> logger)
        {
            this.client = client;
            this.projectInfoService = projectInfoService;
            this.logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullRepoName"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentException">Argument incorrect</exception>
        /// <returns></returns>
        public async Task HandleRepo(string fullRepoName, CancellationToken cancellationToken)
        {
            var chunks = fullRepoName?.Split('/');
            if (chunks?.Length != 2)
            {
                throw new ArgumentException("Required full name in format Owner/Repo", fullRepoName);
            }
            var owner = chunks[0];
            var repoName = string.Join("", chunks.Skip(1));
            try
            {
                var targetRepo = await client.Repository.Get(owner, repoName);
                if (targetRepo == null)
                {
                    return;
                }
                await HandleRepo(targetRepo, cancellationToken);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException($"Not found target {fullRepoName}");
            }
        }

        public async Task HandleRepo(Repository repo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (!repo.PushedAt.HasValue)
                {
                    logger.LogInformation($"{repo.FullName} no PushetAt property");
                    return;
                }

                if (!await projectInfoService.CommitIsActual(repo.FullName, repo.PushedAt ?? DateTime.MinValue))
                {
                    logger.LogInformation($"{repo.FullName} too old commit");
                    return;
                }

                var projectInfo = await ExtractInfoFromRepo(repo);

                if (projectInfo != null)
                {
                    await projectInfoService.AddProjectInfo(repo.FullName, projectInfo.CommitSha, repo.PushedAt.Value, projectInfo);
                }
            }
            catch (ApiException ex) when (ex.Message?.Contains("is empty") == true)
            {
                logger.LogWarning($"Repository {repo.FullName} is empty");
            }
        }

        // TODO: update to .NET 6 and use null safety
        /// <summary>
        /// Read info about repo and returns it. If info can't pe extracted - returns null
        /// </summary>
        /// <param name="repo">Target repo ti handle</param>
        /// <returns>Project info or null</returns>
        private async Task<ProjectInfo> ExtractInfoFromRepo(Repository repo)
        {
            var defaultBranch = repo.DefaultBranch;

            var mainReference = await client.Git.Reference.Get(repo.Id, $"refs/heads/{defaultBranch}");

            var tree = await client.Git.Tree.Get(repo.Id, mainReference.Ref);

            var targetFile = tree.Tree.SingleOrDefault(f => f.Path == "LANDING.md");
            if (targetFile is null)
            {
                logger.LogInformation($"{repo.FullName} no LANDING file");
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
                var info = await new LandingFileParser(repo.FullName, defaultBranch).ParseAsync(fileContent);
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
