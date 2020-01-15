using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Trucking.Integration.Tests.Mocks
{
    public sealed class HttpMessageHandlerMock : HttpMessageHandler, IDisposable
    {
        private HttpResponseMessage _response;
        public HttpMessageHandlerMock(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_response);
        }

        protected override void Dispose(bool disposing)
        {
            _response = null;

            base.Dispose(disposing);
        }
    }
}
