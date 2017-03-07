using System;
using System.Collections.Generic;
using Elmah.Io.AspNetCore.ExceptionFormatters;
using Elmah.Io.Client.Models;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoSettings
    {
        public ElmahIoSettings()
        {
            ExceptionFormatter = new DefaultExceptionFormatter();
            HandledStatusCodesToLog = new List<int> {404};
        }

        public Action<CreateMessage> OnMessage { get; set; }
        public Action<CreateMessage, Exception> OnError { get; set; }
        public IExceptionFormatter ExceptionFormatter { get; set; }
        public List<int> HandledStatusCodesToLog { get; set; }
    }
}