using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            try
            {
                if (report.Status == HealthStatus.Healthy) return Task.CompletedTask;

                if (api == null)
                {
                    api = new ElmahioAPI(new ApiKeyCredentials(options.Value.ApiKey), HttpClientHandlerFactory.GetHttpClientHandler(new ElmahIoOptions()));
                }

                var firstErrorWithException = report
                    .Entries
                    .Values
                    .OrderBy(v => v.Status)
                    .Cast<HealthReportEntry?>()
                    .FirstOrDefault(v => v.HasValue && v.Value.Exception != null);

                var baseException = firstErrorWithException?.Exception.GetBaseException();
                var msg = new CreateMessage
                {
                    Title = Title(report),
                    Severity = Severity(report.Status),
                    Detail = Detail(report),
                    DateTime = DateTime.UtcNow,
                    Type = baseException?.GetType().FullName,
                    Source = baseException?.Source,
                    Hostname = Environment.MachineName,
                    Data = Data(report),
                    Application = options.Value.Application,
                };

                var result = api.Messages.CreateAndNotify(options.Value.LogId, msg);
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Error during publishing health check status to elmah.io.");
                throw;
            }

            return Task.CompletedTask;
        }

        private IList<Item> Data(HealthReport report)
        {
            var result = new List<Item>();
            foreach (var check in report.Entries)
            {
                var checkData = check.Value.Data?.Select(d => new Item($"{check.Key}.{d.Key}", d.Value?.ToString()));
                if (checkData != null) {
                    result.AddRange(checkData);
                }
                var exceptionData = check.Value.Exception?.ToDataList();
                if (exceptionData != null)
                {
                    foreach (var keyItem in exceptionData)
                    {
                        keyItem.Key = $"{check.Key}.{keyItem.Key}";
                    }

                    result.AddRange(exceptionData);
                }
            }

            return result;
        }

        private string Title(HealthReport report)
        {
            return $"{(string.IsNullOrWhiteSpace(options.Value.Application) ? "Application" : options.Value.Application)} in {report.Status} state";
        }

        private string Detail(HealthReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Title(report)} (took: {report.TotalDuration})");
            sb.AppendLine();
            sb.AppendLine("Health Checks:");
            foreach (var s in report.Entries)
            {
                sb.AppendLine($"- {s.Key}: {s.Value.Status}. Took: {s.Value.Duration}. Description: {s.Value.Description}");
            }

            return sb.ToString();
        }

        private string Severity(HealthStatus status)
        {
            return status == HealthStatus.Degraded ? "Warning" : "Error";
        }
    }
}
