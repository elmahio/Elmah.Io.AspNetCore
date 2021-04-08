namespace Elmah.Io.AspNetCore
{
    internal static class ObjectExtensions
    {
        internal static bool IsPrimitiveOrString(this object obj)
        {
            if (obj == null) return false;
            var valueType = obj.GetType();
            return valueType.IsPrimitive || valueType.Equals(typeof(string));
        }
    }
}
