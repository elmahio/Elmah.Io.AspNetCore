using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Elmah.Io.AspNetCore
{
    /// <summary>
    /// Extension methods to help install Elmah.Io.AspNetCore.
    /// </summary>
    public static class ElmahIoExtensions
    {
        /// <summary>
        /// This method register the middleware with ASP.NET Core.
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseElmahIo(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // Verify if AddElmahIo was done before calling UseElmahIo
            if (app.ApplicationServices.GetService(typeof(IBackgroundTaskQueue)) == null)
            {
                throw new InvalidOperationException("No elmah.io dependencies have been registered. Make sure to call the AddElmahIo method in the ConfigureServices method in Startup.cs like this: services.AddElmahIo();");
            }

            return app.UseMiddleware<ElmahIoMiddleware>();
        }

        /// <summary>
        /// Add elmah.io with the specified options.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        public static IServiceCollection AddElmahIo(this IServiceCollection services, Action<ElmahIoOptions> configureOptions)
        {
            services.AddElmahIo();
            services.Configure(configureOptions);
            return services;
        }

        /// <summary>
        /// Add elmah.io without any options. Calling this method requires you to configure elmah.io options manually like this:
        /// <code>services.Configure&lt;ElmahIoOptions&gt;(Configuration.GetSection("ElmahIo"));</code>
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddElmahIo(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<OtherQueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IOtherBackgroundTaskQueue, OtherBackgroundTaskQueue>();
            return services;
        }
    }
}