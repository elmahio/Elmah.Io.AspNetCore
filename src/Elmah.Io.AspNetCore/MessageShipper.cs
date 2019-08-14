using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Http;

namespace Elmah.Io.AspNetCore
{
    internal class MessageShipper
    {
        internal static string _assemblyVersion = typeof(MessageShipper).Assembly.GetName().Version.ToString();

        public static void Ship(Exception exception, string title, HttpContext context, ElmahIoOptions options, IBackgroundTaskQueue queue)
        {
            var baseException = exception?.GetBaseException();
            var createMessage = new CreateMessage
            {
                DateTime = DateTime.UtcNow,
                Detail = Detail(exception, options),
                Type = baseException?.GetType().FullName,
                Title = title,
                Data = exception?.ToDataList(),
                Cookies = Cookies(context),
                Form = Form(context),
                Hostname = context.Request?.Host.Host,
                ServerVariables = ServerVariables(context),
                StatusCode = StatusCode(exception, context),
                Url = context.Request?.Path.Value,
                QueryString = QueryString(context),
                Method = context.Request?.Method,
                Severity = Severity(exception, context),
                Source = Source(baseException),
                Application = options.Application,
            };

            TrySetUser(context, createMessage);

            if (options.OnFilter != null && options.OnFilter(createMessage))
            {
                return;
            }

            queue.QueueBackgroundWorkItem(async token =>
            {
                var elmahioApi = new ElmahioAPI(new ApiKeyCredentials(options.ApiKey), HttpClientHandlerFactory.GetHttpClientHandler(new Client.ElmahIoOptions
                {
                    WebProxy = options.WebProxy
                }));
                elmahioApi.HttpClient.Timeout = new TimeSpan(0, 0, 5);
                elmahioApi.HttpClient.DefaultRequestHeaders.UserAgent.Clear();                
                elmahioApi.HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Elmah.Io.AspNetCore", _assemblyVersion)));

                elmahioApi.Messages.OnMessage += (sender, args) =>
                {
                    options.OnMessage?.Invoke(args.Message);
                };
                elmahioApi.Messages.OnMessageFail += (sender, args) =>
                {
                    options.OnError?.Invoke(args.Message, args.Error);
                };

                try
                {
                    await elmahioApi.Messages.CreateAndNotifyAsync(options.LogId, createMessage);
                }
                catch (Exception e)
                {
                    options.OnError?.Invoke(createMessage, e);
                    // If there's a Exception while generating the error page, re-throw the original exception.
                }
            });
        }

        private static string Source(Exception baseException)
        {
            return baseException?.Source;
        }

        private static void TrySetUser(HttpContext context, CreateMessage createMessage)
        {
            try
            {
                createMessage.User = context?.User?.Identity?.Name;
            }
            catch
            {
                // ASP.NET Core < 2.0 is broken. When creating a new ASP.NET Core 1.x project targeting .NET Framework
                // .NET throws a runtime error complaining about missing System.Security.Claims. For this reason,
                // we don't support setting the User property for 1.x projects targeting .NET Framework.
                // Check out the following GitHub issues for details:
                // - https://github.com/dotnet/standard/issues/410
                // - https://github.com/dotnet/sdk/issues/901
            }
        }

        private static string Severity(Exception exception, HttpContext context)
        {
            var statusCode = StatusCode(exception, context);

            if (statusCode.HasValue && statusCode >= 400 && statusCode < 500) return Client.Severity.Warning.ToString();
            if (statusCode.HasValue && statusCode >= 500) return Client.Severity.Error.ToString();
            if (exception != null) return Client.Severity.Error.ToString();

            return null; // Let elmah.io decide when receiving the message
        }

        private static int? StatusCode(Exception exception, HttpContext context)
        {
            if (exception != null)
            {
                // If an exception is thrown, but the response has a successful status code,
                // it is because the elmah.io middleware are running before the correct
                // status code is assigned the response. Override it with 500.
                return context.Response?.StatusCode < 400 ? 500 : context.Response?.StatusCode;
            }

            return context.Response?.StatusCode;
        }

        private static string Detail(Exception exception, ElmahIoOptions options)
        {
            if (exception == null) return null;
            return options.ExceptionFormatter != null
                ? options.ExceptionFormatter.Format(exception)
                : exception.ToString();
        }

        private static List<Item> Cookies(HttpContext httpContext)
        {
            return httpContext.Request?.Cookies?.Keys.Select(k => new Item(k, httpContext.Request.Cookies[k])).ToList();
        }

        private static List<Item> Form(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request?.Form?.Keys.Select(k => new Item(k, httpContext.Request.Form[k])).ToList();
            }
            catch (InvalidOperationException)
            {
                // Request not a form POST or similar
            }

            return null;
        }

        private static List<Item> ServerVariables(HttpContext httpContext)
        {
            return httpContext.Request?.Headers?.Keys.Select(k => new Item(k, httpContext.Request.Headers[k])).ToList();
        }

        private static List<Item> QueryString(HttpContext httpContext)
        {
            return httpContext.Request?.Query?.Keys.Select(k => new Item(k, httpContext.Request.Query[k])).ToList();
        }
    }
}