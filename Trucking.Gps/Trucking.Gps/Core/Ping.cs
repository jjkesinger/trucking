namespace Trucking.Gps.Core
{
    public class Ping
    {
        public Ping(string message)
        {
            RawData = message;
        }

        public string DeviceId { get; set; }
        public long Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? FuelLevel { get; set; }
        public double? Speed { get; set; }
        public double? Direction { get; set; } //degrees
        public int? IgnitionStatus { get; set; }
        public string RawData { get; }
    }
}
