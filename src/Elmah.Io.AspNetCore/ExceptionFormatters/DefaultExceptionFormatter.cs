using System;

namespace Elmah.Io.AspNetCore.ExceptionFormatters
{
    /// <summary>
    /// An exception formatter that calls <code>ToString</code> on the thrown exception.
    /// </summary>
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public string Format(Exception exception)
        {
            return exception?.ToString();
        }
    }
}