using System.Collections.Generic;

namespace Trucking.Integration.Core.Dto.KeepTruckin
{
    internal class LogRequest
    {
        internal LogRequest()
        {
            Pagination = new Pagination();
            Logs = new List<Log>();
        }

        public List<Log> Logs { get; set; }
        public Pagination Pagination { get; set; }
    }
}
