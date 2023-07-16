using Elmah.Io.AspNetCore.Breadcrumbs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Elmah.Io.AspNetCore
{
    /// <summary>
    /// Contain a number of methods for communicating with elmah.io through the currently configured client.
    /// </summary>
    public static class ElmahIoApi
    {
        /// <summary>
        /// Log an exception to elmah.io with the information provided from the exception and HTTP context.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public static void Log(Exception exception, HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var options = (IOptions<ElmahIoOptions>)context.RequestServices.GetService(typeof(IOptions<ElmahIoOptions>));
            var queue = (IBackgroundTaskQueue)context.RequestServices.GetService(typeof(IBackgroundTaskQueue));

            if (queue == null) throw new InvalidOperationException("No elmah.io dependencies have been registered. Make sure to call the AddElmahIo method in the ConfigureServices method in Startup.cs like this: services.AddElmahIo(...) or in the Program.cs file like this: builder.Services.AddElmahIo(...)");

            MessageShipper.Ship(exception, exception.GetBaseException().Message, context, options.Value, queue);
        }

        /// <summary>
        /// Add a breadcrumb to the current request.
        /// </summary>
        /// <param name="breadcrumb"></param>
        /// <param name="context"></param>
        public static void AddBreadcrumb(Client.Breadcrumb breadcrumb, HttpContext context)
        {
            var feature = context?.Features?.Get<ElmahIoBreadcrumbFeature>();
            feature?.Add(breadcrumb);
        }
    }
}
