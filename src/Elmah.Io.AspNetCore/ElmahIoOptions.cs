using Elmah.Io.AspNetCore.ExceptionFormatters;
using Elmah.Io.Client.Models;
using System;
using System.Collections.Generic;

namespace Elmah.Io.AspNetCore
{
    public class ElmahIoOptions
    {
        public ElmahIoOptions()
        {
            ExceptionFormatter = new DefaultExceptionFormatter();
            HandledStatusCodesToLog = new List<int> { 404 };
        }

        public string ApiKey { get; set; }

        public Guid LogId { get; set; }

        public Action<CreateMessage> OnMessage { get; set; }

        public Action<CreateMessage, Exception> OnError { get; set; }

        public Func<CreateMessage, bool> OnFilter { get; set; }

        public IExceptionFormatter ExceptionFormatter { get; set; }

        public List<int> HandledStatusCodesToLog { get; set; }

    }
}
