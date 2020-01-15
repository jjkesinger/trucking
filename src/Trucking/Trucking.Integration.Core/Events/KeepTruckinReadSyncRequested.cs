using System;
using System.Collections.Generic;
using System.Linq;
using Trucking.Core.EventHandling;

namespace Trucking.Integration.Core.Events
{
    public class KeepTruckinReadSyncRequested : IDomainEvent
    {
        public DateTime EventDateTime { get; private set; }
        public string ApiKey { get; private set; }

        public bool IsCompanySyncRequested => !DriverIds.Any();
        public List<int> DriverIds { get; set; }
    }
}
