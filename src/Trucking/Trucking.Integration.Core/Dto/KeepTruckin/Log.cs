using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Trucking.Integration.Core.Dto.KeepTruckin
{
    internal class Log
    {
        [JsonProperty("id")]
        internal int Id { get; set; }
        [JsonProperty("date")]
        internal string Date { get; set; }
        [JsonProperty("total_miles")]
        internal decimal TotalMiles { get; set; }
        [JsonProperty("metric_units")]
        internal bool IsMetric { get; set; }
        [JsonProperty("driver_signed_at")]
        internal DateTime? DriverSignedAt { get; set; }
        [JsonProperty("time_zone")]
        internal string TimeZone { get; set; }
        [JsonProperty("cycle")]
        internal string Ruleset { get; set; }
        [JsonProperty("shipping_docs")]
        internal string ShippingDoc { get; set; }

        [JsonProperty("driver")]
        internal Driver Driver { get; set; }

        [JsonProperty("vehicles")]
        internal List<Vehicle> Vehicles { get; set; }

        [JsonProperty("co_drivers")]
        internal List<Driver> CoDrivers { get; set; }
        
        [JsonProperty("pagination")]
        internal Pagination Pagination { get; set; }
    }
}
