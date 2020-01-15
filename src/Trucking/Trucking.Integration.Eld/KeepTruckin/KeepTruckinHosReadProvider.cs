using System.Threading.Tasks;
using Trucking.Integration.Core.Capabilities;
using Trucking.Integration.Core.Events;

namespace Trucking.Integration.Eld.KeepTruckin
{
    public class KeepTruckinHosReadProvider : IHosReadProvider
    {
        public KeepTruckinHosReadProvider()
        {
        }

        public async Task ReadHos(object request)
        {
            if (!(request is KeepTruckinReadSyncRequested requestedEvent))
                throw new System.ArgumentException(nameof(request), "Invalid KeepTruckin Sync Reqeust");

            //var logs = 
                
            throw new System.NotImplementedException();
        }
    }
}
