using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Elmah.Io.AspNetCore.HealthChecks;

namespace Elmah.Io.AspNetCore.HealthChecks.Example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ApiKey and LogId can be configured in appsettings.json as well, by calling the Configure-method:
            //services.Configure<ElmahIoPublisherOptions>(Configuration.GetSection("ElmahIo"));

            services
                .AddHealthChecks()
                // Comment out the following to have health checks fail:
                //.AddCheck("Failing", () =>
                //{
                //    throw new ApplicationException("This is failing");
                //})
                .AddElmahIoPublisher(options =>
                {
                    options.ApiKey = "API_KEY";
                    options.LogId = new Guid("LOG_ID");
                    options.HeartbeatId = "HEARTBEAT_ID";
                    // Override the application name (defaults to "Heartbeats"):
                    //options.Application = "My app";
                    // Get a callback on every heartbeat:
                    //options.OnHeartbeat = msg =>
                    //{
                    //    msg.Version = "3.0.0";
                    //};
                });
            // Comment out the following if you want to configure the publisher from appsettings.json (remember to configure ElmahIoPublisherOptions manually as shown above):
            //.AddElmahIoPublisher();
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Period = TimeSpan.FromMinutes(5);
                // If setting Period in ASP.NET Core 2.2, you will need reflection:
                //var prop = options.GetType().GetField("_period", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                //prop.SetValue(options, TimeSpan.FromMinutes(5));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
