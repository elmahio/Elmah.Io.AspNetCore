using System;
using System.Net.Http.Headers;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.Breadcrumbs;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to help install Elmah.Io.AspNetCore.
    /// </summary>
    public static class ElmahIoExtensions
    {
        internal static string _assemblyVersion = typeof(MessageShipper).Assembly.GetName().Version.ToString();
        internal static string _aspNetCoreAssemblyVersion = typeof(HttpContext).Assembly.GetName().Version.ToString();

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
            services.AddHttpContextAccessor();
            services.AddSingleton<ILoggerProvider, ElmahIoBreadcrumbProvider>();
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ElmahIoOptions>>().Value;
                options.ApiKey.AssertApiKey();
                options.LogId.AssertLogId();
                var elmahioApi = Elmah.Io.Client.ElmahioAPI.Create(options.ApiKey, new Elmah.Io.Client.ElmahIoOptions
                {
                    WebProxy = options.WebProxy
                });
                // Storing the message is behind a queue why the default timeout of 5 seconds isn't needed here.
                elmahioApi.HttpClient.Timeout = new TimeSpan(0, 0, 30);
                elmahioApi.HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Elmah.Io.AspNetCore", _assemblyVersion)));
                elmahioApi.HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Microsoft.AspNetCore.Http", _aspNetCoreAssemblyVersion)));

                elmahioApi.Messages.OnMessage += (sender, args) =>
                {
                    options.OnMessage?.Invoke(args.Message);
                };
                elmahioApi.Messages.OnMessageFail += (sender, args) =>
                {
                    options.OnError?.Invoke(args.Message, args.Error);
                };
                return elmahioApi;
            });
            return services;
        }
    }
}