using System;

namespace Elmah.Io.AspNetCore.ExceptionFormatters
{
    /// <summary>
    /// An exception formatter that calls <code>ToString</code> on the thrown exception.
    /// </summary>
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        /// <summary>
        /// Format an exception to a string using the ToString method.
        /// </summary>
        public string Format(Exception exception)
        {
            return exception?.ToString();
        }
    }
}