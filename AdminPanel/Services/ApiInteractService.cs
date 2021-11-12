using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Services
{
    public class ApiInteractService
    {
        private readonly HttpClient httpClient;

        public ApiInteractService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> UpdateRepoInfo(string fullRepoName)
        {
            var response = await httpClient.PostAsync("/api/projects", new StringContent($"\"{fullRepoName}\"", Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
