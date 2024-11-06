using Elmah.Io.AspNetCore.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Extension methods for installing the elmah.io health check publisher.
    /// </summary>
    public static class ElmahIoHealthCheckBuilderExtensions
    {
        /// <summary>
        /// Add a health check publisher elmah.io.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="options">Options used to configure the elmah.io health check publisher.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddElmahIoPublisher(this IHealthChecksBuilder builder, Action<ElmahIoPublisherOptions> options)
        {
            builder.AddElmahIoPublisher();
            builder.Services.Configure(options);
            return builder;
        }

        /// <summary>
        /// Add a health check publisher elmah.io.Calling this method requires you to configure elmah.io options manually like this:
        /// <code>services.Configure&lt;ElmahIoPublisherOptions&gt;(Configuration.GetSection("ElmahIo"));</code>
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddElmahIoPublisher(this IHealthChecksBuilder builder)
        {
            builder.Services.AddSingleton<IHealthCheckPublisher, ElmahIoPublisher>();
            return builder;
        }
    }
}
