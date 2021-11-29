var builder = WebApplication.CreateBuilder(args);

// IMPORTANT: this is where the magic happens. Insert your api key found on the profile as well as the log id of the log to log to.
builder.Services.AddElmahIo(options =>
{
    options.ApiKey = "API_KEY";
    options.LogId = new Guid("LOG_ID");

    // Use log messages logged through Microsoft.Extensions.Logging as breadcrumbs
    //options.TreatLoggingAsBreadcrumbs = true;

    // Filter out breadcrumbs you don't want (like some messages logged through Microsoft.Extensions.Logging)
    //options.OnFilterBreadcrumb = breadcrumb => breadcrumb.Message == "A message we don't want as a breadcrumb";

    // Optional application name
    //options.Application = "ASP.NET Core 6.0 Application";

    // Add event handlers etc. like this:
    //options.OnMessage = msg =>
    //{
    //    msg.Version = "6.0.0";
    //};

    // Remove comment on the following line to log through a proxy (in this case Fiddler).
    //options.WebProxy = new WebProxy("localhost", 8888);
});

// ApiKey and LogId can be configured in appsettings.json as well, by calling the Configure-method instead of AddElmahIo.
//builder.Services.Configure<Elmah.Io.AspNetCore.ElmahIoOptions>(builder.Configuration.GetSection("ElmahIo"));
// Still need to call this to register all dependencies
//builder.Services.AddElmahIo();

// If you configure ApiKey and LogId through appsettings.json, you can still add event handlers, configure handled status codes, etc.
//builder.Services.Configure<Elmah.Io.AspNetCore.ElmahIoOptions>(o =>
//{
//    o.OnMessage = msg =>
//    {
//        msg.Version = "6.0.0";
//    };
//});

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

// IMPORTANT: registers the elmah.io middleware (after registering other exception-aware middleware.
app.UseElmahIo();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
