using Landing.API.Models;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class ScrubProjectInfoService
    {
        private readonly GitHubClient client;
        private readonly ILogger<ScrubProjectInfoService> logger;

        public ScrubProjectInfoService(GitHubClient client, ILogger<ScrubProjectInfoService> logger)
        {
            this.client = client;
            this.logger = logger;
        }
        // TODO: update to .NET 6 and use null safety
        /// <summary>
        /// Read info about repo and returns it. If info can't pe extracted - returns null
        /// </summary>
        /// <param name="repo">Target repo ti handle</param>
        /// <returns>Project info or null</returns>
        public async Task<ProjectInfo> ExtractInfoFromRepo(Repository repo)
        {

            var defaultBranch = repo.DefaultBranch;

            var mainReference = await client.Git.Reference.Get(repo.Id, $"refs/heads/{defaultBranch}");

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
