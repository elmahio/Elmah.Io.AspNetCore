using System;
using Elmah.Io.AspNetCore.Extensions;
using NUnit.Framework;

namespace Elmah.Io.AspNetCore.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [TestCase("")]
        [TestCase(null)]
        public void CanInvalidate(string value)
        {
            Assert.Throws<ArgumentException>(() => value.AssertApiKey());
        }

        public void CanCalidateString()
        {
            var apiKey = "APIKEY";
            Assert.That(apiKey.AssertApiKey(), Is.EqualTo(apiKey));
        }
    }
}