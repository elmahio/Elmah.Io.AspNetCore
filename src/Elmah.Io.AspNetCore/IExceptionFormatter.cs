using System;

namespace Elmah.Io.AspNetCore
{
    /// <summary>
    /// Interface for formatting an exception before sending it to elmah.io.
    /// You can create your own exception formatters by creating a class that implements
    /// this interface with a <code>Format</code> method.
    /// </summary>
    public interface IExceptionFormatter
    {
        string Format(Exception exception);
    }
}