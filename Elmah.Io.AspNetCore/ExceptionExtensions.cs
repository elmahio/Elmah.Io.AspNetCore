using System;
using System.Collections.Generic;
using System.Linq;
using Elmah.Io.AspNetCore.Extensions;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    public static class ExceptionExtensions
    {
        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            Ship(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            MessageShipper.Ship(apiKey, logId, exception.Message, context, settings, exception);
        }
    }
}