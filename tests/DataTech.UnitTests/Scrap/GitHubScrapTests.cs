using DataTech.Domain.Common;
using DataTech.Domain.Models;
using DataTech.Infrastructure.Scrap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DataTech.UnitTests.Scrap
{
    [Trait("Unit Tests", "GitHubScrap - Scrap")]
    public class GitHubScrapTests
    {
        private readonly TestHelper _testHelper;

        public GitHubScrapTests()
        {
            _testHelper = new TestHelper();
        }

        [Fact(DisplayName = "scrap when html is valid returns OK")]
        public async Task Scrap_WhenHtmlIsValid_ReturnsOk()
        {
            // Arrange
            var htmlReadScrapValid = File.ReadAllText(@"..\..\..\..\FilesToTests\GitHub\ReadScrap_Valid.html");

            var mockHttp = _testHelper.CreateMockHttp(HttpMethod.Get, htmlReadScrapValid, System.Net.HttpStatusCode.OK);
            var githubScrap = new GitHubScrap(mockHttp);

            var param = new GitHubInItem
            {
                Location = new List<string>
                {
                    "São Paulo",
                },
                Language = new List<string>
                {
                    "C#",
                }
            };

            // Act
            var result = await githubScrap.Scrap(param, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.OK, result.Status);
            Assert.True(result.ValueAsSuccess.User.Count == 10);
            Assert.Equal("https://github.com/paulosalgado", result.ValueAsSuccess.User.Last().Url);
        }

        [Fact(DisplayName = "scrap when Location is null or empty returns badrequest")]
        public async Task Scrap_WhenLocationIsNullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            var githubScrap = new GitHubScrap(null);

            // Act
            var result = await githubScrap.Scrap(new GitHubInItem { }, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.BadRequest, result.Status);
            Assert.Equal("location can not be null", result.Value.ToString());
        }

        [Theory(DisplayName = "scrap when language list have null or empty item returns badrequest")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Scrap_WhenLanguageListHaveNullOrEmptyItem_ReturnsBadRequest(string language)
        {
            // Arrange
            var githubScrap = new GitHubScrap(null);

            var param = new GitHubInItem
            {
                Location = new List<string>
                {
                    "São Paulo",
                },
                Language = new List<string>
                {
                    "C#",
                    language
                }
            };

            // Act
            var result = await githubScrap.Scrap(param, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.BadRequest, result.Status);
            Assert.Equal("language list can not have null item", result.Value.ToString());
        }

        [Theory(DisplayName = "scrap when request returns some error returns it")]
        [InlineData(HttpStatusCode.BadRequest, ResultStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError, ResultStatusCode.Error)]
        [InlineData(HttpStatusCode.RequestTimeout, ResultStatusCode.TimedOut)]
        [InlineData(HttpStatusCode.NotFound, ResultStatusCode.NotFound)]
        public async Task Scrap_WhenRequestReturnSomeError_ReturnsIt(HttpStatusCode status, ResultStatusCode expected)
        {
            // Arrange
            var message = "some error message";
            var mockHttp = _testHelper.CreateMockHttp(HttpMethod.Get, message, status);
            var githubScrap = new GitHubScrap(mockHttp);

            var param = new GitHubInItem
            {
                Location = new List<string>
                {
                    "São Paulo",
                },
                Language = new List<string>
                {
                    "C#",
                }
            };

            // Act
            var result = await githubScrap.Scrap(param, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Status);
            Assert.Equal(message, result.Value.ToString());
        }

        [Fact(DisplayName = "scrap when html is empty returns error")]
        public async Task Scrap_WhenHtmlIsEmpty_ReturnsError()
        {
            // Arrange

            var mockHttp = _testHelper.CreateMockHttp(HttpMethod.Get, "", HttpStatusCode.OK);
            var githubScrap = new GitHubScrap(mockHttp);

            var param = new GitHubInItem
            {
                Location = new List<string>
                {
                    "São Paulo",
                },
                Language = new List<string>
                {
                    "C#",
                }
            };

            // Act
            var result = await githubScrap.Scrap(param, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.Error, result.Status);
            Assert.Equal("html is null or empty", result.Value.ToString());
        }

        [Fact(DisplayName = "scrap when not found any user returns notfound")]
        public async Task Scrap_WhenNotFoundAnyUser_ReturnsNotFound()
        {
            // Arrange
            var htmlReadScrapValid = File.ReadAllText(@"..\..\..\..\FilesToTests\GitHub\ReadScrap_NotFoundAnyUsers.html");

            var mockHttp = _testHelper.CreateMockHttp(HttpMethod.Get, htmlReadScrapValid, System.Net.HttpStatusCode.OK);
            var githubScrap = new GitHubScrap(mockHttp);

            var param = new GitHubInItem
            {
                Location = new List<string>
                {
                    "São Paulo",
                },
                Language = new List<string>
                {
                    "C#",
                }
            };

            // Act
            var result = await githubScrap.Scrap(param, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.NotFound, result.Status);
            Assert.Equal("not found any users", result.Value.ToString());
        }
    }
}