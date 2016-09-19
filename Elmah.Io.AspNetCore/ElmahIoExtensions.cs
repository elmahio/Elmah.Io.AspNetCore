using System;
using Microsoft.AspNetCore.Builder;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoExtensions
    {
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId)
        {
            return app.UseMiddleware<ElmahIoMiddleware>(apiKey, logId, new ElmahIoSettings());
        }

        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            return app.UseMiddleware<ElmahIoMiddleware>(apiKey, logId, settings);
        }
    }
}