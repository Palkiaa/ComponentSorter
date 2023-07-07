using System;

namespace CompSorting
{
    [Serializable]
    public struct SerializedType
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

        public override int GetHashCode()
        {
            return AssemblyQualifiedName.GetHashCode();
        }

        public Type ConvertToType()
        {
            var node = ComponentDatabase.FindComponent(AssemblyQualifiedName);
            return node?.type ?? null;
        }

        public static bool operator ==(SerializedType a, SerializedType b)
        {
            return a.AssemblyQualifiedName == b.AssemblyQualifiedName;
        }

        public static bool operator !=(SerializedType a, SerializedType b)
        {
            return a.AssemblyQualifiedName != b.AssemblyQualifiedName;
        }

        public static bool operator ==(SerializedType a, Type b)
        {
            return a.AssemblyQualifiedName == b.AssemblyQualifiedName;
        }

        public static bool operator !=(SerializedType a, Type b)
        {
            return a.AssemblyQualifiedName != b.AssemblyQualifiedName;
        }

        public static bool operator ==(Type a, SerializedType b)
        {
            return b == a;
        }

        public static bool operator !=(Type a, SerializedType b)
        {
            return b != a;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}