using System;
using Elmah.Io.Client;
using Microsoft.Extensions.Logging;
using ExtensionsLogger = Microsoft.Extensions.Logging.ILogger;

namespace Elmah.Io.Extensions.Logging
{
    public class ElmahIoLogger : ExtensionsLogger
    {
        readonly IElmahioAPI _elmahioApi;
        private readonly Guid _logId;

        public ElmahIoLogger(string apiKey, Guid logId)
        {
            _logId = logId;
            _elmahioApi = ElmahioAPI.Create(apiKey);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (!IsEnabled(logLevel)) return;

            _elmahioApi.Messages.Log(_logId, exception, LogLevelToSeverity(logLevel), formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Critical || logLevel == LogLevel.Error || logLevel == LogLevel.Warning;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            //TODO not working with async
            return null;
        }

        private Severity LogLevelToSeverity(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    return Severity.Warning;
                case LogLevel.Error:
                    return Severity.Error;
                case LogLevel.Critical:
                    return Severity.Fatal;
                default:
                    throw new ArgumentException("Log level not supported: " + logLevel);
            }
        }
    }
}