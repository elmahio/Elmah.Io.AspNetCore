using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Elmah.Io.Extensions.Logging
{
    public class ElmahIoLoggerProvider : ILoggerProvider
    {
        private readonly string _apiKey;
        private readonly Guid _logId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ElmahIoLoggerProvider(string apiKey, Guid logId, IHttpContextAccessor httpContextAccessor)
        {
            _apiKey = apiKey;
            _logId = logId;
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string name)
        {
            return new ElmahIoLogger(_apiKey, _logId, _httpContextAccessor);
        }

        public void Dispose()
        {
        }
    }
}
