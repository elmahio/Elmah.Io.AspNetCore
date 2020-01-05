using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public class ElmahIoPublisher : IHealthCheckPublisher
    {
        internal static string _assemblyVersion = typeof(ElmahIoPublisher).Assembly.GetName().Version.ToString();

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
                    api = (ElmahioAPI)ElmahioAPI.Create(options.ApiKey, new ElmahIoOptions());
                    // Override the default 5 seconds. Publishing health check results doesn't impact any HTTP request on the users website, why it is fine to wait.
                    api.HttpClient.Timeout = new TimeSpan(0, 0, 0, 30);
                    api.HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Elmah.Io.AspNetCore.HealthChecks", _assemblyVersion)));
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
                    throw;
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
