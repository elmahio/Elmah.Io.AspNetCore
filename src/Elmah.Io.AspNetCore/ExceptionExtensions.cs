using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    /// <summary>
    /// Extension methods for logging to elmah.io manually.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Ship an exception to elmah.io manually:
        /// <code>ex.Ship(HttpContext);</code>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public static void Ship(this Exception exception, HttpContext context)
        {
            var options = (IOptions<ElmahIoOptions>)context.RequestServices.GetService(typeof(IOptions<ElmahIoOptions>));
            var queue = (IBackgroundTaskQueue)context.RequestServices.GetService(typeof(IBackgroundTaskQueue));
            MessageShipper.Ship(exception, exception.GetBaseException().Message, context, options.Value, queue);
        }
    }
}