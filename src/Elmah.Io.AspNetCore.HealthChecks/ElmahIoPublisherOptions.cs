using Elmah.Io.Client.Models;
using System;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    /// <summary>
    /// Contain properties for configuring the elmah.io health check publisher for ASP.NET Core.
    /// </summary>
    public class ElmahIoPublisherOptions
    {
        /// <summary>
        /// The API key from the elmah.io UI.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The id of the log containing the heartbeat.
        /// </summary>
        public Guid LogId { get; set; }

        /// <summary>
        /// The id of the heartbeat to send messages to.
        /// </summary>
        public string HeartbeatId { get; set; }

        /// <summary>
        /// An application name to put on all heartbeat results.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Register an action to be called before logging a heartbeat. Use the OnHeartbeat action to
        /// decorate heartbeats with additional information.
        /// </summary>
        public Action<CreateHeartbeat> OnHeartbeat { get; set; }

        /// <summary>
        /// Register an action to be called if communicating with the elmah.io API fails.
        /// You can use this callback to log the error through Microsoft.Extensions.Logging
        /// or what ever logging framework you may use.
        /// </summary>
        public Action<CreateHeartbeat, Exception> OnError { get; set; }

        /// <summary>
        /// Register an action to filter heartbeats. Use this to add client-side ignore
        /// of some heartbeats. If the filter action returns true, the heartbeat is ignored.
        /// </summary>
        public Func<CreateHeartbeat, bool> OnFilter { get; set; }
    }
}
