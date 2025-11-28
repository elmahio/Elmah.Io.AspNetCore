#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
    public interface IOtherBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Task workItem);

        Task DequeueAsync(CancellationToken cancellationToken);
    }

    public class OtherBackgroundTaskQueue : IOtherBackgroundTaskQueue
    {
        private readonly BlockingCollection<Task> initiatedQueue = new BlockingCollection<Task>();

        public void QueueBackgroundWorkItem(Task workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);

            initiatedQueue.Add(workItem);
        }

        public Task DequeueAsync(CancellationToken cancellationToken)
        {
            return initiatedQueue.Take();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member