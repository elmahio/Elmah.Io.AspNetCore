# Elmah.Io.AspNetCore

[![Build status](https://ci.appveyor.com/api/projects/status/j57ekc2k9eon3u9u?svg=true)](https://ci.appveyor.com/project/ThomasArdal/elmah-io-aspnetcore)
[![NuGet](https://img.shields.io/nuget/vpre/Elmah.Io.AspNetCore.svg)](https://www.nuget.org/packages/Elmah.Io.AspNetCore)

Log to [elmah.io](https://elmah.io/) from [ASP.NET Core](http://www.asp.net/core).

## Installation
Elmah.Io.AspNetCore installs through NuGet:

```
PS> Install-Package Elmah.Io.AspNetCore
```

Configure the elmah.io provider through code:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    app.UseElmahIo("API_KEY", new Guid("LOG_ID"));
    ...
}
```

In the example we tell ASP.NET Core to use elmah.io with the specified `API_KEY` and `LOG_ID`. You will need to replace `API_KEY` with your elmah.io API key (found on your profile) as well as the log ID of the elmah.io log you want to log to.

To decorate errors with information about the failing request, you will need to install the [Microsoft.AspNetCore.Http](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/) NuGet package and register `IHttpContextAccessor` in `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    ...
}
```

## Usage
The logger automatically logs all warnings and errors happening in your web application. You can log exceptions manually like this:

```csharp
logger.LogError(1, new Exception(), "Unexpected error");
```

where `logger` is an instance of `Microsoft.Extensions.Logging.ILogger`.
