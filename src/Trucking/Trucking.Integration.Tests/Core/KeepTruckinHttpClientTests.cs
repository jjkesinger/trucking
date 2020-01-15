using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trucking.Integration.Core.Dto.KeepTruckin;
using Trucking.Integration.Tests.Core.Builders;

namespace Trucking.Integration.Tests.Core
{
    [TestClass]
    public class KeepTruckinHttpClientTests
    {
        [TestMethod]
        public async Task GetLogsWithMultiplePagesShouldReturnAllLogs()
        {
            var expectedLogs = new List<Log>
            {
                new Log(),
                new Log(),
                new Log(),
                new Log(),
                new Log(),
                new Log()
            };

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(expectedLogs);
            var service = builder.Build();

            var actualLogs = await service.GetLogs();

            actualLogs.Should().BeEquivalentTo(expectedLogs);
            builder.HttpMessageHandler.AsMock().Protected().Verify("SendAsync", Times.Exactly(3), 
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task ShouldFilterOnDriverIds()
        {
            var logs = new List<Log>();
            var driverIds = new int[] { 3 };
            var sDriverIds = string.Join(",", driverIds);

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(logs);

            var service = builder.Build();

            await service.GetLogs(driverIds);

            builder.HttpMessageHandler.AsMock().Protected()
                .Verify("SendAsync", Times.Exactly(1), 
                    ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"driverIds={sDriverIds}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task ShouldFilterOnStartDate()
        {
            var logs = new List<Log>();
            var startDate = DateTime.Today;

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(logs);

            var service = builder.Build();

            await service.GetLogs(null, startDate);

            builder.HttpMessageHandler.AsMock().Protected()
                .Verify("SendAsync", Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"start_date={startDate.ToString("YYYY-mm-dd")}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task ShouldFilterOnEndDate()
        {
            var logs = new List<Log>();
            var endDate = DateTime.Today;

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(logs);

            var service = builder.Build();

            await service.GetLogs(null, null, endDate);

            builder.HttpMessageHandler.AsMock().Protected()
                .Verify("SendAsync", Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"end_date={endDate.ToString("YYYY-mm-dd")}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task ShouldFilterOnLastUpdatedDate()
        {
            var logs = new List<Log>();
            var updateDate = DateTime.Today;

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(logs);

            var service = builder.Build();

            await service.GetLogs(null, null, null, updateDate);

            builder.HttpMessageHandler.AsMock().Protected()
                .Verify("SendAsync", Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"updated_after={updateDate.ToString("YYYY-mm-dd")}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [DataTestMethod]
        [DataRow(2, 3)]
        [DataRow(6, 1)]
        [DataRow(3, 2)]
        public async Task ShouldPageByPageSize(int pageSize, int pages)
        {
            var logs = new List<Log>
            {
                new Log(),
                new Log(),
                new Log(),
                new Log(),
                new Log(),
                new Log()
            };

            var builder = new KeepTruckinHttpClientBuilder()
                .WithLogs(logs).WithLogPageSize(pageSize);

            var service = builder.Build();

            await service.GetLogs();

            for (int i = 1; i <= pages; i++)
            {
                builder.HttpMessageHandler.AsMock().Protected()
                   .Verify("SendAsync", Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(f => f.RequestUri.Query.Contains($"page_no={i}")),
                       ItExpr.IsAny<CancellationToken>());
            }
        }
    }
}
