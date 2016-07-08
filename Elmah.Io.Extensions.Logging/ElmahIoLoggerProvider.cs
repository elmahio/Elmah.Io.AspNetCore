using System;
using Microsoft.Extensions.Logging;

namespace Elmah.Io.Extensions.Logging
{
    public class ElmahIoLoggerProvider : ILoggerProvider
    {
        private readonly string _apiKey;
        private readonly Guid _logId;

        public ElmahIoLoggerProvider(string apiKey, Guid logId)
        {
            _apiKey = apiKey;
            _logId = logId;
        }

        public ILogger CreateLogger(string name)
        {
            return new ElmahIoLogger(_apiKey, _logId);
        }

        public void Dispose()
        {
        }
    }
}
