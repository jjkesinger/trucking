using Newtonsoft.Json;

namespace Trucking.Integration.Core.Dto.KeepTruckin
{
    internal class Driver
    {
        [JsonProperty("id")]
        internal int Id { get; set; }
        [JsonProperty("first_name")]
        internal string FirstName { get; set; }
        [JsonProperty("last_name")]
        internal string LastName { get; set; }
        [JsonProperty("username")]
        internal string Username { get; set; }
    }
}
