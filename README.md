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

## Usage
The logger automatically logs all errors happening in your web application.

## Logging messages manually
If you need to log messages and errors to elmah.io manually, you have a range of options. We integrate with the logging framework bundled with ASP.NET Core named Microsoft.Extensions.Logging. Check out the [Elmah.Io.Extensions.Logging](https://github.com/elmahio/Elmah.Io.Extensions.Logging) for details on how to set that up. An alternative is to use one of our integrations for [Serilog](http://docs.elmah.io/logging-to-elmah-io-from-serilog/), [log4net](http://docs.elmah.io/logging-to-elmah-io-from-log4net/), [NLog](http://docs.elmah.io/logging-to-elmah-io-from-nlog/) or [Logary](http://docs.elmah.io/logging-to-elmah-io-from-logary/). Finally, logging directly through the elmah.io client is supported like this:

```csharp
var elmahioApi = ElmahioAPI.Create("API_KEY");
elmahioApi.Messages.Error(new Guid("LOG_ID"), exception, "An error happened");
```
