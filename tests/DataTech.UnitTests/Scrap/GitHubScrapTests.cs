using DataTech.Domain.Common;
using DataTech.Domain.Models;
using DataTech.Infrastructure.Scrap;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DataTech.UnitTests.Scrap
{
    public class GitHubScrapTests
    {
        private readonly TestHelper _testHelper;

        public GitHubScrapTests()
        {
            _testHelper = new TestHelper();
        }

        [Trait("Unit Tests", "GitHubScrap - Scrap")]
        [Fact(DisplayName = "scrap when html is valid returns OK")]
        public async Task Scrah_WhenHtmlIsValid_ReturnsOk()
        {
            // Arrange
            var htmlReadScrapValid = File.ReadAllText(@"..\..\..\..\FilesToTests\GitHub\ReadScrap_Valid.html");

            var mockHttp = _testHelper.CreateMockHttp(HttpMethod.Get, htmlReadScrapValid, System.Net.HttpStatusCode.OK);
            var githubScrap = new GitHubScrap(mockHttp);

            // Act
            var result = await githubScrap.Scrap(new GitHubInItem());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ResultStatusCode.OK, result.Status);
            Assert.True(result.ValueAsSuccess.User.Any());
        }
    }
}