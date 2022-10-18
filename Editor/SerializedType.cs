using System;

namespace CompSorting.Editor
{
    [Serializable]
    public class SerializedType
    {
        public string AssemblyQualifiedName;
        public string Name;

        public SerializedType(Type type)
        {
            AssemblyQualifiedName = type.AssemblyQualifiedName;
            Name = type.Name;
        }

        public Type ConvertToType()
        {
            var node = ComponentDatabase.FindComponent(Name);
            return node?.type ?? null;
        }
    }
}