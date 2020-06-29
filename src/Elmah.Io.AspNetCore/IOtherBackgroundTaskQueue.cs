using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IOtherBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Task workItem);

        Task DequeueAsync(CancellationToken cancellationToken);
    }

    public class OtherBackgroundTaskQueue : IOtherBackgroundTaskQueue
    {
        private readonly BlockingCollection<Task> _initiatedQueue = new BlockingCollection<Task>();

        public void QueueBackgroundWorkItem(Task workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _initiatedQueue.Add(workItem);
        }

        public Task DequeueAsync(CancellationToken cancellationToken)
        {
            return _initiatedQueue.Take();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}