using Newtonsoft.Json;

namespace Trucking.Integration.Core.Dto.KeepTruckin
{
    internal class Vehicle
    {
        [JsonProperty("id")]
        internal int Id { get; set; }
        [JsonProperty("number")]
        internal string VehicleNumber { get; set; }
        [JsonProperty("year")]
        internal string Year { get; set; }
        [JsonProperty("make")]
        internal string Make { get; set; }
        [JsonProperty("model")]
        internal string Model { get; set; }
        [JsonProperty("vin")]
        internal string Vin { get; set; }
        [JsonProperty("metric_units")]
        internal bool MetricUnits { get; set; }
    }
}
