using System;
using System.Threading.Tasks.Dataflow;

namespace Trucking.Gps.Core
{
    internal static class DataflowExtensions
    {
        internal static IDisposable Then<T>(this ISourceBlock<T> source, params ITargetBlock<T>[] targets) where T : class
        {
            IDisposable[] trash = new IDisposable[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                trash[i] = source.Then(targets[i], new DataflowLinkOptions { PropagateCompletion = true });
            }

            return new DisposeTrash(trash);
        }

        internal static IDisposable Then<T>(this ISourceBlock<T> source, ITargetBlock<T> target) where T : class
        {
            return source.Then<T>(target, new DataflowLinkOptions { PropagateCompletion = true });
        }

        private static IDisposable Then<T>(this ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options) where T : class
        {
            return source.Then<T>(target, options, null);
        }

        private static IDisposable Then<T>(this ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options, Predicate<T> filter) where T : class
        {
            if (!options.PropagateCompletion)
                options.PropagateCompletion = true;

            if (filter == null)
                return source.LinkTo(target, options);

            return source.LinkTo(target, options, filter);
        }

        private class DisposeTrash : IDisposable
        {
            private readonly IDisposable[] _trash;
            public DisposeTrash(IDisposable[] trash)
            {
                _trash = trash;
            }

            public void Dispose()
            {
                for (int i = 0; i < _trash.Length; i++)
                {
                    _trash[i].Dispose();
                }
            }
        }
    }
}
