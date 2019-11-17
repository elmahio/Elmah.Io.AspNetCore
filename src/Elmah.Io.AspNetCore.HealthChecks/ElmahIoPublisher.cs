using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public class ElmahIoPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<ElmahIoPublisher> logger;
        private readonly IOptions<ElmahIoPublisherOptions> options;
        private ElmahioAPI api;

        public ElmahIoPublisher(ILogger<ElmahIoPublisher> logger, IOptions<ElmahIoPublisherOptions> options)
        {
            this.logger = logger;
            this.options = options;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            try
            {
                if (api == null)
                {
                    api = new ElmahioAPI(new ApiKeyCredentials(options.Value.ApiKey), HttpClientHandlerFactory.GetHttpClientHandler(new ElmahIoOptions()));
                }

                await api.Heartbeats.CreateAsync(options.Value.HeartbeatId, options.Value.LogId.ToString(), new CreateHeartbeat
                {
                    Result = Result(report),
                    Reason = Reason(report),
                });
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Error during publishing health check status to elmah.io.");
                throw;
            }
        }

        private string Reason(HealthReport report)
        {
            return report.ToString();
        }

        private string Result(HealthReport report)
        {
            switch (report.Status)
            {
                case HealthStatus.Degraded:
                    return "Degraded";
                case HealthStatus.Unhealthy:
                    return "Unhealthy";
                default:
                    return "Healthy";
            }
        }
    }
}
