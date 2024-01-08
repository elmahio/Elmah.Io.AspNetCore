using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Elmah.Io.AspNetCore.Breadcrumbs
{
    [ProviderAlias("ElmahIoBreadcrumbs")]
    internal sealed class ElmahIoBreadcrumbProvider : ILoggerProvider
    {
        private readonly ElmahIoOptions _options;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ElmahIoBreadcrumbProvider(IOptions<ElmahIoOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options.Value;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ElmahIoBreadcrumbLogger(_options, httpContextAccessor);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
