using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IOtherBackgroundTaskQueue _otherBackgroundTaskQueue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IOtherBackgroundTaskQueue otherBackgroundTaskQueue, ILogger<QueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _otherBackgroundTaskQueue = otherBackgroundTaskQueue;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                    var task = workItem(stoppingToken);
                    _otherBackgroundTaskQueue.QueueBackgroundWorkItem(task);
                }
                catch (OperationCanceledException oce)
                {
                    _logger.LogInformation(oce, "OperationCanceledException during dequeue or queue work item - the web app might be shutting down");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while dequeue or queue work item");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}