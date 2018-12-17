using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Elmah.Io.AspNetCore.HealthChecks
{
    public static class ElmahIoHealthCheckBuilderExtensions
    {
        /// <summary>
        /// Add a health check publisher elmah.io.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="apiKey">TODO</param>
        /// <param name="logId">TODO</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns></param>
        public static IHealthChecksBuilder AddElmahIoPublisher(this IHealthChecksBuilder builder, string apiKey, Guid logId, string application = null)
        {
            builder.Services
               .AddSingleton<IHealthCheckPublisher>(sp =>
               {
                   return new ElmahIoPublisher(apiKey, logId, application);
               });

            return builder;
        }
    }
}
