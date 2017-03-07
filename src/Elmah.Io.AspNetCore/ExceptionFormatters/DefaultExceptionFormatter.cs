using System;

namespace Elmah.Io.AspNetCore.ExceptionFormatters
{
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public string Format(Exception exception)
        {
            return exception?.ToString();
        }
    }
}