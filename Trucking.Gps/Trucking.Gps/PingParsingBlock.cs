using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Trucking.Gps.Core;
using Trucking.Gps.Parsing;

namespace Trucking.Gps
{
    public class PingParsingBlock : IPropagatorBlock<string, Ping>, ISourceBlock<string>
    {
        private readonly IPropagatorBlock<string, string> _deadLetter;
        private readonly IPropagatorBlock<Ping, Ping> _source;

        private readonly ITargetBlock<string> _target;
        private readonly ISourceBlock<string> _buffer;

        private readonly IList<Task> _pendingTasks = new List<Task>();
        public PingParsingBlock(params BasePingParser[] parsers)
        {
            _deadLetter = new BufferBlock<string>();
            _source = new BufferBlock<Ping>();
            _buffer = new BufferBlock<string>();

            _target = new ActionBlock<string>(async (s) => {
                await ((BufferBlock<string>)_buffer).SendAsync(s);
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
            });

            for (int i = 0; i < parsers.Length; i++)
            {
                var parser = parsers[i];
                _buffer.LinkTo(parser, new DataflowLinkOptions { PropagateCompletion = true }, parser.Filter);
                parser.LinkTo(_source, new DataflowLinkOptions { PropagateCompletion = true });

                _pendingTasks.Add(parser.Completion);
            }
            _buffer.LinkTo(_deadLetter, new DataflowLinkOptions { PropagateCompletion = true });

            _target.Completion.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    _buffer.Fault(t.Exception);
                }
                else
                {
                    _buffer.Complete();
                }
            });

            _pendingTasks.Add(_source.Completion);
            _pendingTasks.Add(_target.Completion);
            _pendingTasks.Add(_buffer.Completion);
        }

        public Task Completion => Task.WhenAll(_pendingTasks);

        public void Complete()
        {
            _target.Complete();
        }

        public Ping ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target, out bool messageConsumed)
        {
            return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public string ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<string> target, out bool messageConsumed)
        {
            return _deadLetter.ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public void Fault(Exception exception)
        {
            _target.Fault(exception);
        }

        /// <summary>
        /// Source for parsed messages
        /// </summary>
        /// <param name="target">Block to handle parsed pings</param>
        /// <param name="linkOptions">Link options</param>
        /// <returns></returns>
        public IDisposable LinkTo(ITargetBlock<Ping> target, DataflowLinkOptions linkOptions)
        {
            return _source.LinkTo(target, linkOptions);
        }

        /// <summary>
        /// Source for deadletter messages not able to be parsed
        /// </summary>
        /// <param name="target">Block to handle deadletter messages</param>
        /// <param name="linkOptions">Link options</param>
        /// <returns></returns>
        public IDisposable LinkTo(ITargetBlock<string> target, DataflowLinkOptions linkOptions)
        {
            return _deadLetter.LinkTo(target, linkOptions);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, string messageValue, ISourceBlock<string> source, bool consumeToAccept)
        {
            return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target)
        {
            _source.ReleaseReservation(messageHeader, target);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<string> target)
        {
            _deadLetter.ReleaseReservation(messageHeader, target);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<Ping> target)
        {
            return _source.ReserveMessage(messageHeader, target);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<string> target)
        {
            return _deadLetter.ReserveMessage(messageHeader, target);
        }
    }
}
