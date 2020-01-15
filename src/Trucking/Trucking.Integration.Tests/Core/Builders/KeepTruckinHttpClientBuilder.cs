using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Trucking.Integration.Core.Clients;
using Trucking.Integration.Core.Dto.KeepTruckin;
using Trucking.Integration.Tests.Mocks;

namespace Trucking.Integration.Tests.Core.Builders
{
    internal class KeepTruckinHttpClientBuilder
    {
        private int _pageSize;
        internal HttpMessageHandler HttpMessageHandler;
        internal KeepTruckinHttpClientBuilder()
        {
            _pageSize = 2;
            HttpMessageHandler = new HttpMessageHandlerMock(new HttpResponseMessage());
        }

        internal KeepTruckinHttpClientBuilder WithLogPageSize(int pageSize)
        {
            _pageSize = pageSize;

            return this;
        }

        internal KeepTruckinHttpClientBuilder WithLogs(List<Log> logs)
        {
            var mock = new Mock<HttpMessageHandler>();
            var ret = new List<Log>();
            var cnt = 0;

            if (!logs.Any())
                mock.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(new LogRequest
                        {
                            Logs = ret,
                            Pagination = new Pagination
                            {
                                PageNumber = 1,
                                PageTotal = 1
                            }
                        }.AsHttpResponseMessage());
            else
                do
                {
                    ret = logs.Skip(_pageSize * cnt).Take(_pageSize).ToList();
                    cnt++;

                    var pg = cnt;
                    var pgTtl = (logs.Count / _pageSize) + logs.Count % _pageSize;

                    if (ret.Any())
                        mock.Protected()
                            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"page_no={pg}")),
                                ItExpr.IsAny<CancellationToken>())
                            .ReturnsAsync(new LogRequest
                            {
                                Logs = ret,
                                Pagination = new Pagination
                                {
                                    PageNumber = pg,
                                    PageTotal = pgTtl
                                }
                            }.AsHttpResponseMessage());
                } while (ret.Any());

            HttpMessageHandler = mock.Object;

            return this;
        }

        internal KeepTruckinHttpClient Build()
        {
            return new KeepTruckinHttpClient(
                new HttpClient(HttpMessageHandler, true)
                {
                    BaseAddress = new System.Uri("http://localhost.com")
                });
        }
    }
}
