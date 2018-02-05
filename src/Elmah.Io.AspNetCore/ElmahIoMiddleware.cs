using System;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ElmahIoOptions _options;

        //[Obsolete]
        //public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId) : this(next, apiKey, logId, new ElmahIoSettings())
        //{
        //}

        //[Obsolete]
        //public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId, ElmahIoSettings settings)
        //{
        //    _next = next;
        //    _options = new ElmahIoOptions
        //    {
        //        ApiKey = apiKey,
        //        LogId = logId,
        //        ExceptionFormatter = settings.ExceptionFormatter,
        //        HandledStatusCodesToLog = settings.HandledStatusCodesToLog,
        //        OnError = settings.OnError,
        //        OnFilter = settings.OnFilter,
        //        OnMessage = settings.OnMessage,
        //    };
        //}

        public ElmahIoMiddleware(RequestDelegate next, IOptions<ElmahIoOptions> options)
        {
            _next = next;
            _options = options.Value;
            _options.ApiKey.AssertApiKey();
            _options.LogId.AssertLogId();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                if (ShoudLogStatusCode(context))
                {
                    await MessageShipper.ShipAsync(null, "Unsuccessful status code in response", context, _options);
                }
            }
            catch (Exception exception)
            {
                await MessageShipper.ShipAsync(exception, exception.Message, context, _options);
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