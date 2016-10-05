using System;

namespace Elmah.Io.AspNetCore.Extensions
{
    public static class GuidExtensions
    {
        public static Guid AssertLogId(this Guid logId)
        {
            if (logId == Guid.Empty)
                throw new ArgumentException("Input a valid guid as log ID", nameof(logId));

            return logId;
        }
         
    }
}