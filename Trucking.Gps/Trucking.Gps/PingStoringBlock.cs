using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Trucking.Gps
{
    public class PingStoringBlock : ITargetBlock<string>
    {
        private readonly ITargetBlock<string> _target;
        private readonly ISourceBlock<string[]> _source;

        private readonly IList<Task> _pendingTasks = new List<Task>();

        public PingStoringBlock(Func<string[], Task> storeFunc)
        {
            _source = new BatchBlock<string>(25);
            for (int i = 0; i < 100; i++)
            {
                var storageBlock = new ActionBlock<string[]>(storeFunc, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
                _source.LinkTo(storageBlock, new DataflowLinkOptions { PropagateCompletion = true });

                _pendingTasks.Add(storageBlock.Completion);
            }

            _target = new ActionBlock<string>((message) =>
            {
                ((BatchBlock<string>)_source).Post(message);
            });

            _target.Completion.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    _source.Fault(t.Exception);
                else
                    _source.Complete();
            });

            _pendingTasks.Add(_source.Completion);
            _pendingTasks.Add(_target.Completion);
        }

        public Task Completion => Task.WhenAll(_pendingTasks);

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
