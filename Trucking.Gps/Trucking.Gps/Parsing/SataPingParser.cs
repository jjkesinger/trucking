using System;
using Trucking.Gps.Core;

namespace Trucking.Gps.Parsing
{
    public sealed class SataPingParser : BasePingParser
    {
        public override Predicate<string> Filter { get => new Predicate<string>(IsValid); }
        public SataPingParser() : base(Parse)
        { }

        private static Ping Parse(string msg)
        {
            var rnd = new Random();
            return new Ping(msg)
            {
                Timestamp = DateTime.Now.Ticks,
                DeviceId = msg, //msg.Substring(4, msg.Length - 4),
                Latitude = rnd.Next(-180, 180),
                Longitude = rnd.Next(-180, 180)
            };
        }

        private static bool IsValid(string msg)
        {
            if (msg.Length < 4)
                return false;

            return msg.Substring(0, 4).Equals("SATA", StringComparison.OrdinalIgnoreCase);
        }
    }
}
