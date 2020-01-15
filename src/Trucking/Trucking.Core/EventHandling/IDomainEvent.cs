using System;

namespace Trucking.Core.EventHandling
{
    public interface IDomainEvent
    {
        DateTime EventDateTime { get; }
    }
}
