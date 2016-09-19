using System;

namespace Elmah.Io.AspNetCore
{
    public interface IExceptionFormatter
    {
        string Format(Exception exception);
    }
}