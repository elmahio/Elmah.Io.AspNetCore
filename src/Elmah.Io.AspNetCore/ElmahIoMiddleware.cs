using System;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore.Breadcrumbs;
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
        private readonly RequestDelegate next;
        private readonly IBackgroundTaskQueue queue;
        private readonly ElmahIoOptions options;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ElmahIoMiddleware(RequestDelegate next, IBackgroundTaskQueue queue, IOptions<ElmahIoOptions> options)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            this.next = next;
            this.queue = queue;
            this.options = options.Value;
            this.options.ApiKey.AssertApiKey();
            this.options.LogId.AssertLogId();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public async Task Invoke(HttpContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                context.Features.Set(new ElmahIoBreadcrumbFeature(options));

                await next.Invoke(context);
                if (ShoudLogStatusCode(context))
                {
                    MessageShipper.Ship(null, "Unsuccessful status code in response", context, options, queue);
                }
            }
            catch (Exception exception)
            {
                MessageShipper.Ship(exception, exception.GetBaseException().Message, context, options, queue);
                throw;
            }
        }

        private bool ShoudLogStatusCode(HttpContext context)
        {
            return context.Response != null && options.HandledStatusCodesToLog != null &&
                   options.HandledStatusCodesToLog.Contains(context.Response.StatusCode);
        }
    }
}