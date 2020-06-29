using System;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    /// <summary>
    /// The ASP.NET Core middleware responsible for catching uncaught exceptions and logging them to elmah.io.
    /// </summary>
    public class ElmahIoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBackgroundTaskQueue _queue;
        private readonly ElmahIoOptions _options;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ElmahIoMiddleware(RequestDelegate next, IBackgroundTaskQueue queue, IOptions<ElmahIoOptions> options)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            _next = next;
            _queue = queue;
            _options = options.Value;
            _options.ApiKey.AssertApiKey();
            _options.LogId.AssertLogId();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public async Task Invoke(HttpContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                await _next.Invoke(context);
                if (ShoudLogStatusCode(context))
                {
                    MessageShipper.Ship(null, "Unsuccessful status code in response", context, _options, _queue);
                }
            }
            catch (Exception exception)
            {
                MessageShipper.Ship(exception, exception.GetBaseException().Message, context, _options, _queue);
                throw;
            }
        }

        private bool ShoudLogStatusCode(HttpContext context)
        {
            return context.Response != null && _options.HandledStatusCodesToLog != null &&
                   _options.HandledStatusCodesToLog.Contains(context.Response.StatusCode);
        }
    }
}