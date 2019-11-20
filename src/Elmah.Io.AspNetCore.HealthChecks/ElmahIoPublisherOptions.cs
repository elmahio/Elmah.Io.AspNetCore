using Elmah.Io.Client.Models;
using System;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public class ElmahIoPublisherOptions
    {
        public string ApiKey { get; set; }

        public Guid LogId { get; set; }

        public string HeartbeatId { get; set; }

        public string Application { get; set; }

        public Action<CreateHeartbeat> OnHeartbeat { get; set; }

        public Action<CreateHeartbeat, Exception> OnError { get; set; }

        public Func<CreateHeartbeat, bool> OnFilter { get; set; }
    }
}
