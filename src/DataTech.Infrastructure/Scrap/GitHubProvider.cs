using DataTech.Domain.Common;
using DataTech.Domain.Interfaces;
using DataTech.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DataTech.Infrastructure.Scrap
{
    public class GitHubProvider : IProvider<GitHubInItem, GitHubOutItem>
    {
        private readonly Settings _settings;

        public GitHubProvider(Settings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Method to scrap's management 
        /// </summary>
        /// <param name="param">Parameters to search user profile</param>
        /// <param name="ct">token to define request timeout</param>
        /// <returns></returns>
        public Task<Result<GitHubOutItem>> Flow(GitHubInItem param, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}