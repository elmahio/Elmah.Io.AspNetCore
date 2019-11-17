using System;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public class ElmahIoPublisherOptions
    {
        public string ApiKey { get; set; }

        public Guid LogId { get; set; }

        public string HeartbeatId { get; set; }

        public string Application { get; set; }
    }
}
