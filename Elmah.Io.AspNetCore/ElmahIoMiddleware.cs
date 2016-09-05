using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId)
        {
            _next = next;
            _apiKey = apiKey;
            _logId = logId;
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

                elmahioApi.Messages.Create(_logId.ToString(), createMessage);
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