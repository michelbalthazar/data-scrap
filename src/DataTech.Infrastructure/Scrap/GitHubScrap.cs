using DataTech.Domain.Common;
using DataTech.Domain.Interfaces;
using DataTech.Domain.Models;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataTech.Infrastructure.Scrap
{
    public class GitHubScrap : IScrap<GitHubInItem, GitHubOutItem>
    {
        private readonly HttpClient _httpClient;

        public GitHubScrap(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new System.Uri("https://github.com/search");
        }

        public async Task<Result<GitHubOutItem>> Scrap(GitHubInItem param, CancellationToken ct)
        {
            var err = new ErrorBuilder();
            var location = string.Empty;
            var language = string.Empty;
            var url = string.Empty;
            try
            {
                // If location not contains any value or list have one or more null values = retuns error message
                err.Assert(param.Location != null && param.Location.Any() && param.Location.Select(e => string.IsNullOrWhiteSpace(e) == false).All(e => e == true), "location can not be null");
                // If Language is not null it can not be contain any null value in list
                err.Assert(param.Language == null || param.Language.Any() == false || param.Language.Select(e => string.IsNullOrWhiteSpace(e) == false).All(e => e == true), "language list can not have null item");

                // if assert is false hasErrors is true
                if (err.HasErrors)
                    return new Result<GitHubOutItem>(ResultStatusCode.BadRequest, err);

                foreach (var item in param.Location)
                    location += $"location%3A\"{item}\"+";

                // If language is null, can not send it
                if (param.Language.Any())
                {
                    foreach (var item in param.Language)
                        language += $"language%3A\"{WebUtility.UrlEncode(item)}\"+";

                    url = $"?q={location}{language}followers%3A>%3D{param.Followers}+repos%3A>%3D{param.Repository}";
                }
                else
                    url = $"?q={location}followers%3A>%3D{param.Followers}+repos%3A>%3D{param.Repository}";

                var response = await _httpClient.GetAsync(url, ct);

                var html = await HttpHelper<string>.ResponseReadAsStringAsync(response);

                if (html.Status != ResultStatusCode.OK)
                    return new Result<GitHubOutItem>(html.Status, html.Value);

                return ReadScrap(html.ValueAsSuccess);
            }
            catch (Exception ex)
            {
                return new Result<GitHubOutItem>(ResultStatusCode.Error, ex.Message);
            }
        }

        private Result<GitHubOutItem> ReadScrap(string html)
        {
            var err = new ErrorBuilder();
            var result = new GitHubOutItem();
            try
            {
                err.Assert(string.IsNullOrWhiteSpace(html) == false, "html is null or empty");
                if (err.HasErrors)
                    return new Result<GitHubOutItem>(ResultStatusCode.Error, err);

                err.Assert(html.Contains("We couldn’t find any users matching") == false, "not found any users");
                if (err.HasErrors)
                    return new Result<GitHubOutItem>(ResultStatusCode.NotFound, err);

                // need to log this html when returns this error
                err.Assert(html.Contains("user_search_results") && html.Contains("user-list") && html.Contains("user-list-item py-4 d-flex hx_hit-user"), "Invalid page");
                if (err.HasErrors)
                    return new Result<GitHubOutItem>(ResultStatusCode.InvalidPage, err);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='user-list-item py-4 d-flex hx_hit-user']");

                foreach (var node in nodes)
                {
                    // Add new user
                    result.User.Add(new User
                    {
                        Url = $"https://github.com/{node.SelectSingleNode("//a[@class='mr-1']/@href").Attributes.First(e => e.Name == "href").Value.Replace("/", "")}",
                        Location = node.SelectSingleNode("//div[@class='mr-3']").InnerText.Trim()
                    });

                    node.RemoveAll();
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<GitHubOutItem>(ResultStatusCode.Error, ex.Message);
            }
        }
    }
}