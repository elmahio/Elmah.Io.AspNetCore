using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OtherQueuedHostedService : BackgroundService
    {
        private readonly IOtherBackgroundTaskQueue _taskQueue;
        private Task _backgroundRunner;
        public OtherQueuedHostedService(IOtherBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _backgroundRunner = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var t = _taskQueue.DequeueAsync(cancellationToken);
                        await t;
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}