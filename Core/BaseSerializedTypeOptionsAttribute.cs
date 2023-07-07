using System;

namespace CompSorting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class BaseSerializedTypeOptionsAttribute : Attribute
    {
        public abstract SerializedType[] GetOptions(SerializedType serializedType);
    }
}