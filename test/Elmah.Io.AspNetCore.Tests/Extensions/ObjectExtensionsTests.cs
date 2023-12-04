using Elmah.Io.AspNetCore.Extensions;
using NUnit.Framework;
using System;
using System.Net;

namespace Elmah.Io.AspNetCore.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Test] public void StringIsValid() => Assert.That("Hello".IsValidForItems(), Is.True);
        [Test] public void IntIsValid() => Assert.That(42.IsValidForItems(), Is.True);
        [Test] public void VersionIsValid() => Assert.That(new Version("1.0.0").IsValidForItems(), Is.True);
        [Test] public void IPAddressIsValid() => Assert.That(IPAddress.Parse("127.0.0.1").IsValidForItems(), Is.True);
        [Test] public void ObjectNotValid() => Assert.That(new ArgumentException().IsValidForItems(), Is.False);
    }
}
