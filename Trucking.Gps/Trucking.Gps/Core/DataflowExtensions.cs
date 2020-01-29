using System;
using System.Threading.Tasks.Dataflow;

namespace Trucking.Gps.Core
{
    internal static class DataflowExtensions
    {
        internal static IDisposable PropagateTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target) where T : class
        {
            return source.PropagateTo<T>(target, new DataflowLinkOptions { PropagateCompletion = true });
        }

        internal static IDisposable PropagateTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options) where T : class
        {
            return source.PropagateTo<T>(target, options, null);
        }

        internal static IDisposable PropagateTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options, Predicate<T> filter) where T : class
        {
            if (!options.PropagateCompletion)
                options.PropagateCompletion = true;

            if (filter == null)
                return source.LinkTo(target, options);

            return source.LinkTo(target, options, filter);
        }
    }
}
