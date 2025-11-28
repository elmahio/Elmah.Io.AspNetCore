#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new();
        private readonly SemaphoreSlim signal = new(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            workItems.Enqueue(workItem);
            signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member