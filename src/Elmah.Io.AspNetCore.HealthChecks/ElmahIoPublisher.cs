using Elmah.Io.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    /// <summary>
    /// Publishes ASP.NET Core health check results as elmah.io Heartbeats.
    /// </summary>
    public class ElmahIoPublisher : IHealthCheckPublisher
    {
        internal static string _assemblyVersion = typeof(ElmahIoPublisher).Assembly.GetName().Version.ToString();

        private readonly ILogger<ElmahIoPublisher> logger;
        private readonly ElmahIoPublisherOptions options;
        private IElmahioAPI api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ElmahIoPublisher(ILogger<ElmahIoPublisher> logger, IOptions<ElmahIoPublisherOptions> options)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            this.logger = logger;
            this.options = options.Value;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                if (api == null)
                {
                    api = ElmahioAPI.Create(options.ApiKey, new Client.ElmahIoOptions
                    {
                        Timeout = new TimeSpan(0, 0, 30), // Override the default 5 seconds. Publishing health check results doesn't impact any HTTP request on the users website, why it is fine to wait.
                        UserAgent = new ProductInfoHeaderValue(new ProductHeaderValue("Elmah.Io.AspNetCore.HealthChecks", _assemblyVersion)).ToString(),
                    });
                }

                var createHeartbeat = new CreateHeartbeat
                {
                    Result = Result(report),
                    Reason = Reason(report),
                    Application = options.Application,
                    Took = Took(report),
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

        private long? Took(HealthReport report)
        {
            return (long?)report?.TotalDuration.TotalMilliseconds;
        }

        private string Reason(HealthReport report)
        {
            var sb = new StringBuilder();
            sb.Append(options.Application ?? "Application");
            sb.AppendLine($" in {report.Status} state (took: {report.TotalDuration})");

            var remainingCount = report.Entries.Count;
            if (remainingCount > 0) sb.AppendLine();

            var unhealthy = new List<KeyValuePair<string, HealthReportEntry>>();
            var degraded = new List<KeyValuePair<string, HealthReportEntry>>();
            var healthy = new List<KeyValuePair<string, HealthReportEntry>>();

            foreach (var s in report.Entries)
            {
                if (s.Value.Status == HealthStatus.Unhealthy) unhealthy.Add(s);
                if (s.Value.Status == HealthStatus.Degraded) degraded.Add(s);
                if (s.Value.Status == HealthStatus.Healthy) healthy.Add(s);
            }

            remainingCount = degraded.Count + healthy.Count;
            Generate(sb, unhealthy, "Unhealthy", remainingCount);
            remainingCount = healthy.Count;
            Generate(sb, degraded, "Degraded", remainingCount);
            remainingCount = 0;
            Generate(sb, healthy, "Healthy", remainingCount);

            return sb.ToString();
        }

        private void Generate(StringBuilder sb, List<KeyValuePair<string, HealthReportEntry>> checks, string category, int remainingCount)
        {
            if (checks.Any())
            {
                sb.AppendLine($"{category} checks:");
                foreach (var s in checks.OrderBy(u => u.Key))
                {
                    sb.AppendLine($"- {s.Key}: {s.Value.Status}. Took: {s.Value.Duration}. Description: {s.Value.Description}");
                }

                if (remainingCount > 0) sb.AppendLine();
            }
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
