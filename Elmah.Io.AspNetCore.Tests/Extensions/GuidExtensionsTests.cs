using System;
using Elmah.Io.AspNetCore.Extensions;
using NUnit.Framework;

namespace Elmah.Io.AspNetCore.Tests.Extensions
{
    public class GuidExtensionsTests
    {
        [Test]
        public void CanValidateEmpty()
        {
            var g = Guid.Empty;
            Assert.Throws<ArgumentException>(() => g.AssertLogId());
        }

        [Test]
        public void CanValidateGuid()
        {
            Assert.That(Guid.NewGuid().AssertLogId(), Is.Not.Null);
        }
    }
}