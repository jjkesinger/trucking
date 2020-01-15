using System.Threading.Tasks;
using Trucking.Core.EventHandling;
using Trucking.Integration.Core.Capabilities;
using Trucking.Integration.Core.Events;

namespace Trucking.Integration.Core.Providers
{
    public class KeepTruckin : IntegrationProvider, 
        IHandle<KeepTruckinReadSyncRequested>
    {
        private readonly IHosReadProvider _hosReadProvider;

        public KeepTruckin(IHosReadProvider hosReadProvider)
        {
            _hosReadProvider = hosReadProvider;
        }

        public async Task Handle(KeepTruckinReadSyncRequested domainEvent)
        {
            await _hosReadProvider.ReadHos(domainEvent);
        }
    }
}
