using Elmah.Io.Client;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elmah.Io.AspNetCore.Tests
{
    public class MessageShipperTest
    {
        [Test]
        public void CanFilterMessage()
        {
            // Arrange
            var backgroundTaskQueueMock = Substitute.For<IBackgroundTaskQueue>();
            var options = new ElmahIoOptions
            {
                OnFilter = msg => true,
            };

            // Act
            MessageShipper.Ship(new Exception(), "test", new DefaultHttpContext(), options, backgroundTaskQueueMock);

            // Assert
            backgroundTaskQueueMock
                .DidNotReceive()
                .QueueBackgroundWorkItem(Arg.Any<Func<CancellationToken, Task>>());
        }

        [Test]
        public void CanShipMessage()
        {
            // Arrange
            var innerException = new ArgumentException("an argument exception");
            innerException.Data.Add("innerkey", "innervalue");
            innerException.Source = "innersource";
            var outerException = new ApplicationException("an application exception", innerException);
            outerException.Data.Add("outerkey", "outervalue");
            var backgroundTaskQueueMock = Substitute.For<IBackgroundTaskQueue>();
            var options = new ElmahIoOptions
            {
                Application = "MyApp",
            };
            CreateMessage message = null;
            // Maybe a bit ugly to use the OnFilter action to validate the message, but it's just a lot
            // easier than having to write a check on the Func sent to the queue.
            options.OnFilter = msg =>
            {
                message = msg;
                return false;
            };

            // Act
            MessageShipper.Ship(outerException, "test", new DefaultHttpContext(), options, backgroundTaskQueueMock);

            // Assert
            backgroundTaskQueueMock
                .Received()
                .QueueBackgroundWorkItem(Arg.Any<Func<CancellationToken, Task>>());
            Assert.That(message != null);
            Assert.That(message.Application, Is.EqualTo("MyApp"));
            Assert.That(message.DateTime.HasValue && (DateTime.UtcNow - message.DateTime.Value).TotalMinutes <= 1);
            Assert.That(message.Type, Is.EqualTo("System.ArgumentException"));
            Assert.That(message.Title, Is.EqualTo("test"));
            Assert.That(message.Data != null && message.Data.Count == 4);
            Assert.That(message.Data.Any(d => d.Key == "ApplicationException.outerkey" && d.Value == "outervalue"));
            Assert.That(message.Data.Any(d => d.Key == "ArgumentException.innerkey" && d.Value == "innervalue"));
            Assert.That(message.Data.Any(d => d.Key == "X-ELMAHIO-EXCEPTIONINSPECTOR"));
            Assert.That(message.Data.Any(d => d.Key == "X-ELMAHIO-FRAMEWORKDESCRIPTION"));
            Assert.That(message.Hostname, Is.EqualTo(Environment.MachineName));
            Assert.That(message.StatusCode, Is.EqualTo(500));
            Assert.That(message.Severity, Is.EqualTo("Error"));
            Assert.That(message.Source, Is.EqualTo("innersource"));
        }
    }
}
