using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

        public static IServiceCollection AddElmahIo(this IServiceCollection services, string apiKey, Guid logId)
        {
            return services.AddElmahIo(apiKey, logId, new ElmahIoSettings());
        }

        public static IServiceCollection AddElmahIo(this IServiceCollection services, string apiKey, Guid logId, ElmahIoSettings settings)
        {
            var config = new ElmahIoConfiguration(apiKey, logId, settings);
            services.AddSingleton(config);
            return services;
        }
    }
}