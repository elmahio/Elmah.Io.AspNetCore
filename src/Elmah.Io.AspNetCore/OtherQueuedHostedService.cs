using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OtherQueuedHostedService : BackgroundService
    {
        private readonly IOtherBackgroundTaskQueue _taskQueue;
        private readonly ILogger<OtherQueuedHostedService> _logger;

        public OtherQueuedHostedService(IOtherBackgroundTaskQueue taskQueue, ILogger<OtherQueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var task = _taskQueue.DequeueAsync(cancellationToken);
                        await task;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while dequeue and execute task");
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}