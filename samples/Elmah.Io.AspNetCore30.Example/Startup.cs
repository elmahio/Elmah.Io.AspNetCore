using System;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elmah.Io.AspNetCore30.Example
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

            services.AddElmahIo(options =>
            {
                options.ApiKey = "API_KEY";
                options.LogId = new Guid("LOG_ID");

                // Optional application name
                //options.Application = "ASP.NET Core 3.0 Application";

                // Add event handlers etc. like this:
                //options.OnMessage = msg =>
                //{
                //    msg.Version = "3.0.0";
                //};

                // Remove comment on the following line to log through a proxy (in this case Fiddler).
                //options.WebProxy = new WebProxy("localhost", 8888);
            });

            // ApiKey and LogId can be configured in appsettings.json as well, by calling the Configure-method instead of AddElmahIo.
            //services.Configure<ElmahIoOptions>(Configuration.GetSection("ElmahIo"));
            // Still need to call this to register all dependencies
            //services.AddElmahIo();

            // If you configure ApiKey and LogId through appsettings.json, you can still add event handlers, configure handled status codes, etc.
            //services.Configure<ElmahIoOptions>(o =>
            //{
            //    o.OnMessage = msg =>
            //    {
            //        msg.Version = "3.0.0";
            //    };
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // IMPORTANT: registers the elmah.io middleware (after registering other exception-aware middleware.
            app.UseElmahIo();

            app.UseHttpsRedirection();
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
