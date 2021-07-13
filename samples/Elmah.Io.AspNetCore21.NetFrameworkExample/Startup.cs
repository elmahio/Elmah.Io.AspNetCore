using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elmah.Io.AspNetCore21.NetFrameworkExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // IMPORTANT: this is where the magic happens. Insert your api key found on the profile as well as the log id of the log to log to.
            services.AddElmahIo(options =>
            {
                options.ApiKey = "API_KEY";
                options.LogId = new Guid("LOG_ID");

                // Optional application name
                //options.Application = "ASP.NET Core 2.1 on .NET Framework";

                // Add event handlers etc. like this:
                //options.OnMessage = msg =>
                //{
                //    msg.Version = "2.1.0";
                //};

                // Remove comment on the following line to log through a proxy (in this case Fiddler).
                //options.WebProxy = new WebProxy("localhost", 8888);
            });

            // ApiKey and LogId can be configured in appsettings.json as well, by calling the Configure-method instead of AddElmahIo.
            //services.Configure<Elmah.Io.AspNetCore.ElmahIoOptions>(Configuration.GetSection("ElmahIo"));

            // If you configure ApiKey and LogId through appsettings.json, you can still add event handlers, configure handled status codes, etc.
            //services.Configure<Elmah.Io.AspNetCore.ElmahIoOptions>(o =>
            //{
            //    o.OnMessage = msg =>
            //    {
            //        msg.Version = "2.1.0";
            //    };
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // IMPORTANT: registers the elmah.io middleware (after registering other exception-aware middleware.
            app.UseElmahIo();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
