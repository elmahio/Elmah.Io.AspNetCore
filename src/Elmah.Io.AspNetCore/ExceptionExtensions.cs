using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    public static class ExceptionExtensions
    {
        public static async Task ShipAsync(this Exception exception, HttpContext context)
        {
            var options = (IOptions<ElmahIoOptions>)context.RequestServices.GetService(typeof(IOptions<ElmahIoOptions>));
            await MessageShipper.ShipAsync(exception, exception.GetBaseException().Message, context, options.Value);
        }

        public static void Ship(this Exception exception, HttpContext context)
        {
            var options = (IOptions<ElmahIoOptions>)context.RequestServices.GetService(typeof(IOptions<ElmahIoOptions>));
            Task.Factory.StartNew(s => MessageShipper.ShipAsync(exception, exception.GetBaseException().Message, context, options.Value), null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        [Obsolete("Configure apiKey and logId through the new AddElmahIo method and call .ShipAsync(context) instead.")]
        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            await ShipAsync(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        [Obsolete("Configure apiKey and logId through the new AddElmahIo method and call .Ship(context) instead.")]
        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context)
        {
            Ship(exception, apiKey, logId, context, new ElmahIoSettings());
        }

        [Obsolete("Configure apiKey, logId and settings through the new AddElmahIo method and call .ShipAsync(context) instead.")]
        public static async Task ShipAsync(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            await MessageShipper.ShipAsync(exception, exception.GetBaseException().Message, context, new ElmahIoOptions
            {
                ApiKey = apiKey,
                LogId = logId,
                ExceptionFormatter = settings.ExceptionFormatter,
                HandledStatusCodesToLog = settings.HandledStatusCodesToLog,
                OnError = settings.OnError,
                OnFilter = settings.OnFilter,
                OnMessage = settings.OnMessage
            });
        }

        [Obsolete("Configure apiKey, logId and settings through the new AddElmahIo method and call .Ship(context) instead.")]
        public static void Ship(this Exception exception, string apiKey, Guid logId, HttpContext context, ElmahIoSettings settings)
        {
            Task.Factory.StartNew(s => ShipAsync(exception, apiKey, logId, context, settings), null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
    }
}