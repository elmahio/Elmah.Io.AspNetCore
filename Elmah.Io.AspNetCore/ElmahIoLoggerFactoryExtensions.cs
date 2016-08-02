using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Elmah.Io.AspNetCore
{
    public static class ElmahIoLoggerFactoryExtensions
    {
        public static ILoggerFactory AddElmahIo(this ILoggerFactory factory, string apiKey, Guid logId, IHttpContextAccessor httpContextAccessor)
        {
            factory.AddProvider(new ElmahIoLoggerProvider(apiKey, logId, httpContextAccessor));
            return factory;
        }
    }
}
