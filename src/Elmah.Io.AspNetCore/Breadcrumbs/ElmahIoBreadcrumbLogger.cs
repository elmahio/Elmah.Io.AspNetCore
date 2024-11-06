using Elmah.Io.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elmah.Io.AspNetCore.Breadcrumbs
{
    internal class ElmahIoBreadcrumbLogger(ElmahIoOptions options, IHttpContextAccessor httpContextAccessor) : ILogger
    {
        private readonly ElmahIoOptions options = options;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return options.TreatLoggingAsBreadcrumbs;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            var title = Title(state, formatter, exception);
            // It doesn't provide much value to show title-less breadcrumbs
            if (string.IsNullOrWhiteSpace(title)) return;

            var feature = httpContextAccessor.HttpContext?.Features?.Get<ElmahIoBreadcrumbFeature>();
            feature?.Add(new Client.Breadcrumb(DateTime.UtcNow, LogLevelToSeverity(logLevel), "Log", title));
        }

        private static string LogLevelToSeverity(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Critical => nameof(Severity.Fatal),
                LogLevel.Debug => nameof(Severity.Debug),
                LogLevel.Error => nameof(Severity.Error),
                LogLevel.Information => nameof(Severity.Information),
                LogLevel.Trace => nameof(Severity.Verbose),
                LogLevel.Warning => nameof(Severity.Warning),
                _ => nameof(Severity.Information),
            };
        }

        internal static string Title<TState>(TState state, Func<TState, Exception, string> formatter, Exception exception)
        {
            if (formatter != null)
            {
                var message = formatter(state, exception);

                // User logged a formatted message. Use this.
                if (!string.IsNullOrWhiteSpace(message)) return message;
            }

            // No formatted message provided. Use the base exception message if exceptions is logged as part of this message.
            if (exception != null) return exception.GetBaseException().Message;

            // No formatted message or exception provided. Build something from the state if key values pairs of string and object.
            if (state is IEnumerable<KeyValuePair<string, object>> enumerable) return string.Join(", ", enumerable.Take(5));

            // We tried everything else. Provide null to ignore this message
            return null;
        }
    }
}
