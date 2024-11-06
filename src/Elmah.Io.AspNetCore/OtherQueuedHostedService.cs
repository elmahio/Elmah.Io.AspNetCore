using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OtherQueuedHostedService(IOtherBackgroundTaskQueue taskQueue, ILogger<OtherQueuedHostedService> logger) : BackgroundService
    {
        private readonly IOtherBackgroundTaskQueue _taskQueue = taskQueue;
        private readonly ILogger<OtherQueuedHostedService> _logger = logger;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var task = _taskQueue.DequeueAsync(stoppingToken);
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Other Queued Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}