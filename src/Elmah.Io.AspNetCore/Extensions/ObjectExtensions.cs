using System;
using System.Net;

namespace Elmah.Io.AspNetCore.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsValidForItems(this object obj)
        {
            if (obj == null) return false;
            var valueType = obj.GetType();
            return valueType.IsPrimitive
                || valueType.Equals(typeof(string))
                || valueType.Equals(typeof(Version))
                || valueType.Equals(typeof(IPAddress));
        }
    }
}
