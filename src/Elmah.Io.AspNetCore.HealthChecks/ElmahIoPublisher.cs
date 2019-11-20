using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public class ElmahIoPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<ElmahIoPublisher> logger;
        private readonly ElmahIoPublisherOptions options;
        private ElmahioAPI api;

        public ElmahIoPublisher(ILogger<ElmahIoPublisher> logger, IOptions<ElmahIoPublisherOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            try
            {
                if (api == null)
                {
                    api = new ElmahioAPI(new ApiKeyCredentials(options.ApiKey), HttpClientHandlerFactory.GetHttpClientHandler(new ElmahIoOptions()));
                }

                var createHeartbeat = new CreateHeartbeat
                {
                    Result = Result(report),
                    Reason = Reason(report),
                    Application = options.Application,
                };

                if (options.OnFilter != null && options.OnFilter(createHeartbeat))
                {
                    return;
                }

                options.OnHeartbeat?.Invoke(createHeartbeat);

                try
                {
                    await api.Heartbeats.CreateAsync(options.HeartbeatId, options.LogId.ToString(), createHeartbeat);
                }
                catch (Exception inner)
                {
                    options.OnError?.Invoke(createHeartbeat, inner);
                    throw inner;
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Error during publishing health check status to elmah.io.");
                throw;
            }
        }

        private string Reason(HealthReport report)
        {
            var sb = new StringBuilder();
            sb.Append(options.Application ?? "Application");
            sb.AppendLine($" in {report.Status} state (took: {report.TotalDuration})");
            sb.AppendLine();
            sb.AppendLine("Health Checks:");
            foreach (var s in report.Entries)
            {
                sb.AppendLine($"- {s.Key}: {s.Value.Status}. Took: {s.Value.Duration}. Description: {s.Value.Description}");
            }

            return sb.ToString();
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
