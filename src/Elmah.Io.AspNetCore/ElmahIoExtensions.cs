using System;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoExtensions
    {
        [Obsolete("Configuration of apiKey and logId have been moved to the AddElmahIo method. When adding a call to that method in your ConfigureServices-method, call UseElmahIo (no parameters) in the Configure-method instead.")]
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId)
        {
            return app.UseElmahIo(apiKey, logId, new ElmahIoSettings());
        }

        [Obsolete("Configuration of apiKey, logId and settings have been moved to the AddElmahIo method. When adding a call to that method in your ConfigureServices-method, call UseElmahIo (no parameters) in the Configure-method instead.")]
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            return app.UseMiddleware<ElmahIoMiddleware>(apiKey, logId, settings);
        }

        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ElmahIoMiddleware>();
        }

        public static IServiceCollection AddElmahIo(this IServiceCollection services, ElmahIoOptions options)
        {
            services.Configure<ElmahIoOptions>(o =>
            {
                o.ApiKey = options.ApiKey;
                o.ExceptionFormatter = options.ExceptionFormatter;
                o.HandledStatusCodesToLog = options.HandledStatusCodesToLog;
                o.LogId = options.LogId;
                o.OnError = options.OnError;
                o.OnFilter = options.OnFilter;
                o.OnMessage = options.OnMessage;
            });
            return services;
        }
    }
}