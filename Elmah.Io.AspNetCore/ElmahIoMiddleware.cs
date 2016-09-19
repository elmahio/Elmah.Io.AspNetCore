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
        private readonly Action<CreateMessage> _onMessage;
        private readonly Action<CreateMessage, Exception> _onError;

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId) : this(next, apiKey, logId, msg => { }, (msg, ex) => { })
        {
        }

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId, Action<CreateMessage> onMessage, Action<CreateMessage, Exception> onError)
        {
            _next = next;
            _apiKey = apiKey;
            _logId = logId;
            _onMessage = onMessage;
            _onError = onError;
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
                    Detail = exception.ToString(),
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
                    _onMessage?.Invoke(args.Message);
                };
                elmahioApi.Messages.OnMessageFail += (sender, args) => {
                    _onError?.Invoke(args.Message, args.Error);
                };

                try
                {
                    elmahioApi.Messages.CreateAndNotify(_logId.ToString(), createMessage);
                }
                catch (Exception e)
                {
                    _onError?.Invoke(createMessage, e);
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