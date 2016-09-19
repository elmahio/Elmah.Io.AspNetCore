using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;
        private readonly Guid _logId;
        private readonly ElmahIoSettings _settings;

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId) : this(next, apiKey, logId, new ElmahIoSettings())
        {
        }

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("Input an API key", nameof(apiKey));
            if (logId == Guid.Empty) throw new ArgumentException("Input a valid guid as log ID", nameof(logId));
            if (settings == null) throw new ArgumentNullException(nameof(settings), "Input settings for elmah.io");

            _next = next;
            _apiKey = apiKey;
            _logId = logId;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exception)
            {
                var elmahioApi = ElmahioAPI.Create(_apiKey);
                var createMessage = new CreateMessage
                {
                    DateTime = DateTime.UtcNow,
                    Detail = _settings.ExceptionFormatter != null ? _settings.ExceptionFormatter.Format(exception) : exception.ToString(),
                    Type = exception.GetType().Name,
                    Title = exception.Message,
                    Data = exception.ToDataList(),
                    Cookies = Cookies(context),
                    Form = Form(context),
                    Hostname = context.Request?.Host.Host,
                    ServerVariables = ServerVariables(context),
                    StatusCode = context.Response?.StatusCode,
                    Url = context.Request?.Path.Value,
                    QueryString = QueryString(context),
                    Method = context.Request?.Method,
                };

                elmahioApi.Messages.OnMessage += (sender, args) => {
                    _settings.OnMessage?.Invoke(args.Message);
                };
                elmahioApi.Messages.OnMessageFail += (sender, args) => {
                    _settings.OnError?.Invoke(args.Message, args.Error);
                };

                try
                {
                    elmahioApi.Messages.CreateAndNotify(_logId.ToString(), createMessage);
                }
                catch (Exception e)
                {
                    _settings.OnError?.Invoke(createMessage, e);
                    // If there's a Exception while generating the error page, re-throw the original exception.
                }

                throw;
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