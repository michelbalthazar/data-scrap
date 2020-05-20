using Moq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataTech.UnitTests
{
    public class TestHelper
    {
        #region mocks
        public HttpClient CreateMockHttp(HttpMethod httpMethod, string response = "responseToTest", HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            var handler = new FakeHttpMessageHandler(response, httpMethod, httpStatusCode);

            //var mockHttpClient = new Mock<HttpClient>(handler);

            return new HttpClient(handler);
        }

        public HttpClient CreateMockHttp(HttpMethod httpMethod, byte[] response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            var handler = new FakeHttpMessageHandler(response, httpMethod, httpStatusCode);

            //var mockHttpClient = new Mock<HttpClient>(handler);

            return new HttpClient(handler);
        }
        #endregion mocks
    }
    #region class to helper mock httpClient

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage response;

        public FakeHttpMessageHandler(string response, HttpMethod httpMethod, HttpStatusCode httpStatusCode)
        {
            this.response = SendAsync(response, httpMethod, httpStatusCode);
        }

        public FakeHttpMessageHandler(byte[] response, HttpMethod httpMethod, HttpStatusCode httpStatusCode)
        {
            this.response = SendAsync(response, httpMethod, httpStatusCode);
        }

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseTask = new TaskCompletionSource<HttpResponseMessage>();
            responseTask.SetResult(response);

            return responseTask.Task;
        }

        public virtual HttpResponseMessage SendAsync(string response, HttpMethod httpMethod, HttpStatusCode httpStatusCode)
        {
            return new HttpResponseMessage(httpStatusCode) { RequestMessage = new HttpRequestMessage(httpMethod, "http://sample/test"), Content = new StringContent(response) };
        }

        public virtual HttpResponseMessage SendAsync(byte[] response, HttpMethod httpMethod, HttpStatusCode httpStatusCode)
        {
            return new HttpResponseMessage(httpStatusCode) { RequestMessage = new HttpRequestMessage(httpMethod, "http://sample/test"), Content = new ByteArrayContent(response) };
        }
    }

    public class FakeHttpContent : HttpContent
    {
        public string Content { get; set; }

        public FakeHttpContent(string content)
        {
            Content = content;
        }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(Content);
            await stream.WriteAsync(byteArray, 0, Content.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = Content?.Length ?? 0;
            return true;
        }
    }
    #endregion class to helper mock httpClient
}