using DataTech.Domain.Common;
using DataTech.Domain.Interfaces;
using DataTech.Domain.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataTech.Infrastructure.Scrap
{
    public class GitHubScrap : IScrap<GitHubInItem, GitHubOutItem>
    {
        private readonly HttpClient _httpClient;

        public GitHubScrap(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<Result<GitHubOutItem>> Scrap(GitHubInItem param)
        {
            return ResultStatusCode.Error;
        }

        private Result<GitHubOutItem> ReadScrap(string html)
        {
            return ResultStatusCode.OK;
        }
    }
}