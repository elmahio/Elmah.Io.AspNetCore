using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    public static class ExceptionExtensions
    {
        [Obsolete("Configure apiKey and logId through the new AddElmahIo method and call .ShipAsync(context) instead.")]
        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            await ShipAsync(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        public static async Task ShipAsync(this Exception exception, HttpContext context)
        {
            var config = (ElmahIoConfiguration)context.RequestServices.GetService(typeof(ElmahIoConfiguration));
            await MessageShipper.ShipAsync(config.ApiKey, config.LogId, exception.Message, context, config.Settings, exception);
        }

        [Obsolete("Configure apiKey and logId through the new AddElmahIo method and call .Ship(context) instead.")]
        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            Ship(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        public static void Ship(this Exception exception, HttpContext context)
        {
            var config = (ElmahIoConfiguration)context.RequestServices.GetService(typeof(ElmahIoConfiguration));
            MessageShipper.Ship(config.ApiKey, config.LogId, exception.Message, context, config.Settings, exception);
        }

        [Obsolete("Configure apiKey, logId and settings through the new AddElmahIo method and call .ShipAsync(context) instead.")]
        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            await MessageShipper.ShipAsync(apiKey, logId, exception.Message, context, settings, exception);
        }

        [Obsolete("Configure apiKey, logId and settings through the new AddElmahIo method and call .Ship(context) instead.")]
        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            MessageShipper.Ship(apiKey, logId, exception.Message, context, settings, exception);
        }
    }
}