using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoExtensions
    {
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ElmahIoMiddleware>();
        }

        public static IServiceCollection AddElmahIo(this IServiceCollection services, Action<ElmahIoOptions> configureOptions)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.Configure(configureOptions);
            return services;
        }
    }
}