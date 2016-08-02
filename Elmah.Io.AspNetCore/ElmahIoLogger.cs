using System;
using System.Collections.Generic;
using System.Linq;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ExtensionsLogger = Microsoft.Extensions.Logging.ILogger;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoLogger : ExtensionsLogger
    {
        private readonly IElmahioAPI _elmahioApi;
        private readonly Guid _logId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ElmahIoLogger(string apiKey, Guid logId, IHttpContextAccessor httpContextAccessor)
        {
            _logId = logId;
            _httpContextAccessor = httpContextAccessor;
            _elmahioApi = ElmahioAPI.Create(apiKey);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (!IsEnabled(logLevel)) return;

            var createMessage = new CreateMessage
            {
                DateTime = DateTime.UtcNow,
                Detail = exception?.ToString(),
                Type = exception?.GetType().Name,
                Severity = LogLevelToSeverity(logLevel),
                Title = formatter(state, exception),
                Data = exception.ToDataList(),
            };

            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                createMessage.Cookies = Cookies(httpContext);
                createMessage.Form = Form(httpContext);
                createMessage.Hostname = httpContext.Request?.Host.Host;
                createMessage.ServerVariables = ServerVariables(httpContext);
                createMessage.StatusCode = httpContext.Response?.StatusCode;
                createMessage.Url = httpContext.Request?.Path.Value;
                createMessage.QueryString = QueryString(httpContext);
                createMessage.Method = httpContext.Request?.Method;
            }

            _elmahioApi.Messages.Create(_logId.ToString(), createMessage);
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

        private string LogLevelToSeverity(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    return Severity.Warning.ToString();
                case LogLevel.Error:
                    return Severity.Error.ToString();
                case LogLevel.Critical:
                    return Severity.Fatal.ToString();
                default:
                    throw new ArgumentException("Log level not supported: " + logLevel);
            }
        }

        private List<Item> Cookies(HttpContext httpContext)
        {
            return httpContext.Request?.Cookies?.Keys.Select(k => new Item(k, httpContext.Request.Cookies[k])).ToList();
        }

        private List<Item> Form(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request?.Form?.Keys.Select(k => new Item(k, httpContext.Request.Form[k])).ToList();
            }
            catch (InvalidOperationException)
            {
                // Request not a form POST or similar
            }

            return null;
        }

        private static List<Item> ServerVariables(HttpContext httpContext)
        {
            return httpContext.Request?.Headers?.Keys.Select(k => new Item(k, httpContext.Request.Headers[k])).ToList();
        }

        private static List<Item> QueryString(HttpContext httpContext)
        {
            return httpContext.Request?.Query?.Keys.Select(k => new Item(k, httpContext.Request.Query[k])).ToList();
        }
    }
}