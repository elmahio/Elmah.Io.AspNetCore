using Elmah.Io.AspNetCore.Extensions;
using NUnit.Framework;
using System;

namespace Elmah.Io.AspNetCore.Tests.Extensions
{
    public class ElmahIoSettingsExtensionsTests
    {
        [Test]
        public void CanValidateNull()
        {
            ElmahIoSettings settings = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => settings.AssertSettings());
        }

        [Test]
        public void CanValidateObject()
        {
            Assert.That(new ElmahIoSettings().AssertSettings(), Is.Not.Null);
        }
    }
}
