using System;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Builder;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoExtensions
    {
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId)
        {
            return app.UseMiddleware<ElmahIoMiddleware>(apiKey, logId);
        }

        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId, Action<CreateMessage> onMessage, Action<CreateMessage, Exception> onError)
        {
            return app.UseMiddleware<ElmahIoMiddleware>(apiKey, logId, onMessage, onError);
        }
    }
}