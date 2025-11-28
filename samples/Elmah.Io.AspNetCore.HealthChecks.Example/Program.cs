using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
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
        //    msg.Version = "10.0.0";
        //};
    });
    // Comment out the following if you want to configure the publisher from appsettings.json (remember to configure ElmahIoPublisherOptions manually as shown above):
    //.AddElmahIoPublisher();

builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Period = TimeSpan.FromMinutes(1);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();