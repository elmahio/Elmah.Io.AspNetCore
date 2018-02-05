using System;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;
        private readonly Guid _logId;
        private readonly ElmahIoSettings _settings;

        [Obsolete]
        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId) : this(next, apiKey, logId, new ElmahIoSettings())
        {
        }

        [Obsolete]
        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            _next = next;
            _apiKey = apiKey.AssertApiKey();
            _logId = logId.AssertLogId();
            _settings = settings.AssertSettings();
        }

        public ElmahIoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var config = (ElmahIoConfiguration)context.RequestServices.GetService(typeof(ElmahIoConfiguration));
            var apiKey = config != null ? config.ApiKey : _apiKey;
            var logId = config != null ? config.LogId : _logId;
            var settings = config != null ? config.Settings : _settings;
            try
            {
                await _next.Invoke(context);
                if (ShoudLogStatusCode(context, settings))
                {
                    await MessageShipper.ShipAsync(apiKey, logId, "Unsuccessful status code in response", context, settings);
                }
            }
            catch (Exception exception)
            {
                await MessageShipper.ShipAsync(apiKey, logId, exception.Message, context, settings, exception);
                throw;
            }
        }

        private bool ShoudLogStatusCode(HttpContext context, ElmahIoSettings settings)
        {
            return context.Response != null && settings.HandledStatusCodesToLog != null &&
                   settings.HandledStatusCodesToLog.Contains(context.Response.StatusCode);
        }
    }
}