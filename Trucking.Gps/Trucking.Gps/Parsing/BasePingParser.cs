using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Trucking.Gps.Core;

namespace Trucking.Gps.Parsing
{
    public abstract class BasePingParser : IPropagatorBlock<string, Ping>, ISourceBlock<Ping>, ITargetBlock<string>
    {
        protected readonly ISourceBlock<Ping> _source;
        protected readonly ITargetBlock<string> _target;
        public abstract Predicate<string> Filter { get; }
        public BasePingParser(Func<string, Ping> parse)
        {
            _source = new BufferBlock<Ping>();
            _target = new ActionBlock<string>(async msg => await ((BufferBlock<Ping>)_source).SendAsync(parse(msg)), 
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
                });

            _target.Completion.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                    _source.Fault(t.Exception);
                else
                    _source.Complete();
            });
        }

        public Task Completion => _target.Completion;

        public void Complete()
        {
            _target.Complete();
        }

        public Ping ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target, out bool messageConsumed)
        {
            return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public void Fault(Exception exception)
        {
            _source.Fault(exception);
        }

        public IDisposable LinkTo(ITargetBlock<Ping> target, DataflowLinkOptions linkOptions)
        {
            return _source.LinkTo(target, linkOptions);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, string messageValue, ISourceBlock<string> source, bool consumeToAccept)
        {
            return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target)
        {
            throw new NotImplementedException();
        }
    }
}
