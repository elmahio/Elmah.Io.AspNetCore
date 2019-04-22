using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    public static class ExceptionExtensions
    {
        public static void Ship(this Exception exception, HttpContext context)
        {
            var options = (IOptions<ElmahIoOptions>)context.RequestServices.GetService(typeof(IOptions<ElmahIoOptions>));
            var queue = (IBackgroundTaskQueue)context.RequestServices.GetService(typeof(IBackgroundTaskQueue));
            MessageShipper.Ship(exception, exception.GetBaseException().Message, context, options.Value, queue);
        }
    }
}