using System;

namespace Elmah.Io.AspNetCore.Extensions
{
    internal static class GuidExtensions
    {
        internal static Guid AssertLogId(this Guid logId)
        {
            if (logId == Guid.Empty)
                throw new ArgumentException("Input a valid guid as log ID", nameof(logId));

            return logId;
        }
    }
}