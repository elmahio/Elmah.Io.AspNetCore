using System;
using Microsoft.AspNetCore.Http;

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
            ElmahIoApi.Log(exception, context);
        }
    }
}