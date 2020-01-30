using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Trucking.Gps.Core;

namespace Trucking.Gps
{
    public class PingFuelBlock : ITargetBlock<Ping>
    {
        private readonly ITargetBlock<Ping> _target;
        public PingFuelBlock(Func<Ping, Task> saveFuel)
        {
            _target = new ActionBlock<Ping>(saveFuel);
        }

        public Task Completion => _target.Completion;

        public void Complete()
        {
            _target.Complete();
        }

        public void Fault(Exception exception)
        {
            _target.Fault(exception);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, Ping messageValue, ISourceBlock<Ping> source, bool consumeToAccept)
        {
            return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }
    }
}
