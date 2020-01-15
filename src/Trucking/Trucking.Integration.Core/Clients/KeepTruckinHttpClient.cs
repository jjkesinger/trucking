using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Trucking.Integration.Core.Dto.KeepTruckin;

[assembly: InternalsVisibleTo("Trucking.Integration.Tests")]
namespace Trucking.Integration.Core.Clients
{
    public class KeepTruckinHttpClient
    {
        private readonly HttpClient _httpClient;
        internal KeepTruckinHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<List<Log>> GetLogs(int[] driverIds = null, DateTime? startDate = null, 
            DateTime? endDate = null, DateTime? updatedAfter = null)
        {
            var logs = new List<Log>();
            int pg = 0, pgTotal = 0;

            do
            {
                var param = new Dictionary<string, string>();

                if (driverIds != null && driverIds.Length > 0)
                    param.Add("driverIds", string.Join(",", driverIds));

                if (startDate.HasValue)
                    param.Add("start_date", startDate.Value.ToString("YYYY-mm-dd"));

                if (endDate.HasValue)
                    param.Add("end_date", endDate.Value.ToString("YYYY-mm-dd"));

                if (updatedAfter.HasValue)
                    param.Add("updated_after", updatedAfter.Value.ToString("YYYY-mm-dd"));

                pg++;
                param.Add("page_no", pg.ToString());

                var uri = QueryHelpers.AddQueryString("logs", param);
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                var logResponse = await SendRequest<LogRequest>(request);
                logs.AddRange(logResponse.Logs);
                
                pgTotal = logResponse.Pagination.PageTotal;
            }
            while (pg < pgTotal);

            return logs;
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request) where T : class
        { 
            var response = await _httpClient.SendAsync(request);
            var serilaized = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(serilaized);
        }
    }
}
