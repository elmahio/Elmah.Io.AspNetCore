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

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId) : this(next, apiKey, logId, new ElmahIoSettings())
        {
        }

        public ElmahIoMiddleware(RequestDelegate next, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            _next = next;
            _apiKey = apiKey.AssertApiKey();
            _logId = logId.AssertLogId();
            _settings = settings.AssertSettings();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                if (context.Response.StatusCode >= 400)
                {
                    // This is a catch all to also catch errors like 400, 404 and even 500 which not throws an exception.
                    await MessageShipper.ShipAsync(_apiKey, _logId, "Unsuccessful status code in response", context, _settings);
                }
            }
            catch (Exception exception)
            {
                await exception.ShipAsync(_apiKey, _logId, context, _settings);
                throw;
            }
        }
    }
}