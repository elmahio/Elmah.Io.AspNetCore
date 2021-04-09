using Elmah.Io.AspNetCore.Extensions;
using NUnit.Framework;
using System;

namespace Elmah.Io.AspNetCore.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Test] public void StringIsValid() => Assert.IsTrue("Hello".IsValidForItems());
        [Test] public void IntIsValid() => Assert.IsTrue(42.IsValidForItems());
        [Test] public void VersionIsValid() => Assert.IsTrue(new Version("1.0.0").IsValidForItems());
        [Test] public void ObjectNotValid() => Assert.IsFalse(new ArgumentException().IsValidForItems());
    }
}
