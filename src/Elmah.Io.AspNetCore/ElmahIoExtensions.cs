using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoExtensions
    {
        /// <summary>
        /// This method register the middleware with ASP.NET Core.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ElmahIoMiddleware>();
        }

        /// <summary>
        /// Add elmah.io with the specified options.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddElmahIo(this IServiceCollection services, Action<ElmahIoOptions> configureOptions)
        {
            services.AddElmahIo();
            services.Configure(configureOptions);
            return services;
        }

        /// <summary>
        /// Add elmah.io without any options. Calling this method requires you to configure elmah.io options manually like this:
        /// 
        /// <code>services.Configure<ElmahIoOptions>(Configuration.GetSection("ElmahIo"));</code>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddElmahIo(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<OtherQueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IOtherBackgroundTaskQueue, OtherBackgroundTaskQueue>();
            return services;
        }
    }
}