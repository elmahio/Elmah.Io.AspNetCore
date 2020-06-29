using System;
using System.Collections.Generic;
using System.Text;

namespace Elmah.Io.AspNetCore.ExceptionFormatters
{
    /// <summary>
    /// An exception formatter that formats a thrown exception like the yellow screen of death known from ASP.NET.
    /// </summary>
    public class YellowScreenOfDeathExceptionFormatter : IExceptionFormatter
    {
        /// <summary>
        /// Format an exception to a Yellow Screen of Death like output.
        /// </summary>
        public string Format(Exception exception)
        {
            if (exception == null) return null;

            var sb = new StringBuilder();
            var exceptionStack = new List<Exception>();
            for (Exception e = exception; e != null; e = e.InnerException)
            {
                exceptionStack.Add(e);
            }

            for (var i = exceptionStack.Count - 1; i >= 0; i--)
            {
                if (i < exceptionStack.Count - 1)
                    sb.Append("\r\n");

                var e = exceptionStack[i];

                sb.Append(ExceptionToString(e));
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private string ExceptionToString(Exception ex)
        {
            var description = new StringBuilder();
            description.AppendFormat("{0}: {1}", ex.GetType().FullName, ex.Message);
            description.Append("\r\n");
            description.Append(ex.StackTrace);
            return description.ToString();
        }
    }
}