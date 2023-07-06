using System;

namespace CompSorting
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

        public SerializedType(string name, string assemblyQualifiedName)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
            Name = name;
        }

        public Type ConvertToType()
        {
            var node = ComponentDatabase.FindComponent(Name);
            return node?.type ?? null;
        }
    }
}