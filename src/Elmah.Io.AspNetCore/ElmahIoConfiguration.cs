using System;
using System.Collections.Generic;
using System.Text;

namespace Elmah.Io.AspNetCore
{
    internal class ElmahIoConfiguration
    {
        public string ApiKey { get; set; }

        public Guid LogId { get; set; }

        public ElmahIoSettings Settings { get; set; }

        internal ElmahIoConfiguration(string apiKey, Guid logId, ElmahIoSettings settings)
        {
            ApiKey = apiKey;
            LogId = logId;
            Settings = settings;
        }
    }
}
