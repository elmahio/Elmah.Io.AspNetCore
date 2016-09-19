using System;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elmah.Io.AspNetCore.Example
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            var logger = loggerFactory.CreateLogger("MyLog");

            // IMPORTANT: this is where the magic happens. Insert your api key found on the profile as well as the log id of the log to log to.
            // To execute some code every time a message is logged and/or fails, comment out the two event handlers.
            app.UseElmahIo("API_KEY", new Guid("LOG_ID")/*, OnMessage(), OnError(logger)*/);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static Action<CreateMessage, Exception> OnError(ILogger logger)
        {
            return (msg, ex) =>
            {
                logger.LogError(1, ex, "Error during logging of message to elmah.io");
            };
        }

        private static Action<CreateMessage> OnMessage()
        {
            return msg =>
            {
                msg.Version = "1.0.0";
            };
        }
    }
}
