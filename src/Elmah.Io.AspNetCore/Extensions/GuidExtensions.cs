using System;

namespace Elmah.Io.AspNetCore.Extensions
{
    internal static class GuidExtensions
    {
        public static Guid AssertLogId(this Guid logId)
        {
            if (logId == Guid.Empty)
                throw new ArgumentException("Input a valid guid as log ID", nameof(logId));

            return logId;
        }

        public static Guid AssertLogIdInMiddleware(this Guid logId)
        {
            if (logId == Guid.Empty)
                throw new ArgumentNullException(nameof(logId), "Log ID not initialized through elmah.io middleware");

            return logId;
        }
    }
}