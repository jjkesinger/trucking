using System;

namespace Trucking.Gps.Parsing
{
    public sealed class GeometrisPingParser : BasePingParser
    {
        public override Predicate<string> Filter { get => new Predicate<string>(IsValid); }
        public GeometrisPingParser() : base(Parse)
        { }

        private static Ping Parse(string msg)
        {
            var rnd = new Random();
            return new Ping(msg)
            {
                Timestamp = DateTime.Now.Ticks,
                DeviceId = msg, //msg.Substring(3, msg.Length - 3),
                Latitude = rnd.Next(-180, 180),
                Longitude = rnd.Next(-180, 180)
            };
        }

        private static bool IsValid(string msg)
        {
            if (msg.Length < 3)
                return false;

            return msg.Substring(0, 3).Equals("GEO", StringComparison.OrdinalIgnoreCase);
        }
    }
}
