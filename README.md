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
    app.UseExceptionHandler("/Home/Error");
    ...
    app.UseElmahIo("API_KEY", new Guid("LOG_ID"));
    ...
}
```

In the example we tell ASP.NET Core to use elmah.io with the specified `API_KEY` and `LOG_ID`. You will need to replace `API_KEY` with your elmah.io API key (found on your profile) as well as the log ID of the elmah.io log you want to log to. It is important to configure the elmah.io middleware after adding other exception handling middleware (like `UseExceptionHandler`).

As default, uncaught exceptions (500's) and 404's are logged automatically. Let's say you have a controller returning a Bad Request and want to log that as well. Since returning a 400 from a controller doesn't trigger an exception, you will need to configure this status code:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    var settings = new ElmahIoSettings();
    settings.HandledStatusCodesToLog.Add(400);
    app.UseElmahIo("API_KEY", new Guid("LOG_ID"), settings);
    ...
}
```

By adding the status code `400`, the elmah.io client will log any responses with this status code as well. Notice that you will not need to configure this, if your controller action set a status code of 400 and throw an exception.

To decorate errors or handle elmah.io downtime, you can use two events found on the `ElmahIoSettings` class:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    var settings = new ElmahIoSettings
    {
        OnMessage = msg =>
        {
            msg.Version = "1.0.0";
        },
        OnError = (msg, ex) =>
        {
            // Do something about the failing message
        }
    };
    ...
}
```

## Usage
The logger automatically logs all errors happening in your web application.

## Logging messages manually
If you need to log messages and errors to elmah.io manually, you have a range of options. We integrate with the logging framework bundled with ASP.NET Core named Microsoft.Extensions.Logging. Check out the [Elmah.Io.Extensions.Logging](https://github.com/elmahio/Elmah.Io.Extensions.Logging) for details on how to set that up. An alternative is to use one of our integrations for [Serilog](http://docs.elmah.io/logging-to-elmah-io-from-serilog/), [log4net](http://docs.elmah.io/logging-to-elmah-io-from-log4net/), [NLog](http://docs.elmah.io/logging-to-elmah-io-from-nlog/) or [Logary](http://docs.elmah.io/logging-to-elmah-io-from-logary/). Finally, logging directly through the elmah.io client is supported like this:

```csharp
var elmahioApi = ElmahioAPI.Create("API_KEY");
elmahioApi.Messages.Error(new Guid("LOG_ID"), exception, "An error happened");
```

or using the extension methods on `Exception`:

```csharp
try
{
    var i = 0;
    var result = 42/i;
}
catch (DivideByZeroException e)
{
    e.Ship("API_KEY", new Guid("LOG_ID"), HttpContext);
}

```
