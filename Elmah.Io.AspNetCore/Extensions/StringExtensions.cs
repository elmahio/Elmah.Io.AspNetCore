using System;

namespace Elmah.Io.AspNetCore.Extensions
{
    public static class StringExtensions
    {
        public static string AssertApiKey(this string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Input an API key", nameof(apiKey));

            return apiKey;
        }
         
    }
}