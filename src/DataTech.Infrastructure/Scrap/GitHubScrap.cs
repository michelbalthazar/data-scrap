using DataTech.Domain.Common;
using DataTech.Domain.Interfaces;
using DataTech.Domain.Models;
using System.Threading.Tasks;

namespace DataTech.Infrastructure.Scrap
{
    public class GitHubScrap : IScrap<GitHubInItem, GitHubOutItem>
    {
        public async Task<Result<GitHubOutItem>> Scrap(GitHubInItem param)
        {
            return ResultStatusCode.OK;
        }

        public Result<GitHubOutItem> ReadScrap(string html)
        {
            return ResultStatusCode.OK;
        }
    }
}