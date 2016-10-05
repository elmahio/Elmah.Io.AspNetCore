using System;

namespace Elmah.Io.AspNetCore.Extensions
{
    public static class ElmahIoSettingsExtensions
    {
        public static ElmahIoSettings AssertSettings(this ElmahIoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "Input settings for elmah.io");

            return settings;
        }
         
    }
}