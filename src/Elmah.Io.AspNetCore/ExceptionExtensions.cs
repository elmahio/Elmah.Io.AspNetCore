using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    public static class ExceptionExtensions
    {
        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            await ShipAsync(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        public static async Task ShipAsync(this Exception exception, HttpContext context)
        {
            await ShipAsync(exception, context, new ElmahIoSettings());
        }

        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            Ship(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        public static void Ship(this Exception exception, HttpContext context)
        {
            Ship(exception, context, new ElmahIoSettings());
        }

        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            await MessageShipper.ShipAsync(apiKey, logId, exception.Message, context, settings, exception);
        }

        public static async Task ShipAsync(this Exception exception, HttpContext context, ElmahIoSettings settings)
        {
            await MessageShipper.ShipAsync(exception.Message, context, settings, exception);
        }

        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            MessageShipper.Ship(apiKey, logId, exception.Message, context, settings, exception);
        }

        public static void Ship(this Exception exception, HttpContext context, ElmahIoSettings settings)
        {
            MessageShipper.Ship(exception.Message, context, settings, exception);
        }
    }
}