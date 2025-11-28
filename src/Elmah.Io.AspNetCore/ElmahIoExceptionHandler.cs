#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Exception handler for ASP.NET Core that logs exceptions to elmah.io. This can be used as an alternative to the elmah.io middleware.
    /// Logging using an exception handler works better in scanrios where you call app.UseExceptionHandler("/Error") or similar.
    ///
    /// This class is still experimental.
    /// </summary>
    public class ElmahIoExceptionHandler : IExceptionHandler
    {
        private readonly IBackgroundTaskQueue queue;
        private readonly ElmahIoOptions options;

        public ElmahIoExceptionHandler(IBackgroundTaskQueue queue, IOptions<ElmahIoOptions> options)
        {
            this.queue = queue;
            this.options = options.Value;
            this.options.ApiKey.AssertApiKey();
            this.options.LogId.AssertLogId();
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            MessageShipper.Ship(exception, exception?.GetBaseException().Message ?? "An error happened", httpContext, options, queue);
            return false;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member