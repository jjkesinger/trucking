using Newtonsoft.Json;

namespace Trucking.Integration.Core.Dto.KeepTruckin
{
    internal class Pagination
    {
        [JsonProperty("per_page")]
        internal int PerPage { get; set; }
        [JsonProperty("page_no")]
        internal int PageNumber { get; set; }
        [JsonProperty("total")]
        internal int PageTotal { get; set; }
    }
}
