using System;
using System.Net.Http;

namespace Elmah.Io.AspNetCore
{
    internal static class HttpClientHandlerFactory
    {
        private static HttpClientHandler _instance = null;
        private static DateTime _initTime = DateTime.MinValue;
        private static TimeSpan _lifeTime = TimeSpan.FromHours(24);

        public static HttpClientHandler GetHttpClientHandler(ElmahIoOptions options)
        {            
            if (DateTime.Now.Subtract(_initTime) > _lifeTime || _instance == null)
            {
                _instance = new HttpClientHandler
                {
                    UseProxy = options.WebProxy != null,
                    Proxy = options.WebProxy,
                };
                _initTime = DateTime.Now;
            }
            return _instance;
        }
    }
}