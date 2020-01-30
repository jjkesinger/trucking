using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Trucking.Gps
{
    public class PingStoringBlock : ITargetBlock<string>
    {
        private readonly ITargetBlock<string> _target;
        private readonly ISourceBlock<string[]> _source;

        public PingStoringBlock(Func<string[], Task> storeFunc)
        {
            _source = new BatchBlock<string>(25);

             var storageBlock = new ActionBlock<string[]>(storeFunc, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });
            _source.LinkTo(storageBlock, new DataflowLinkOptions { PropagateCompletion = true });

            _target = new ActionBlock<string>(async (message) =>
            {
                await ((BatchBlock<string>)_source).SendAsync(message);
            });

            _target.Completion.ContinueWith(t =>
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

        public void Fault(Exception exception)
        {
            _target.Fault(exception);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, string messageValue, ISourceBlock<string> source, bool consumeToAccept)
        {
            return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }
    }
}
