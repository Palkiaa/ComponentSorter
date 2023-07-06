using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompSorting
{
    /// <summary>
    /// Source: https://discussions.unity.com/t/getting-all-component-types-even-those-not-on-the-scene/213930/2
    ///
    /// From the author:
    /// It can be used to get a list of all components that are derived from a certain base class or that implement a certain interface (GetTypes method).
    /// In addition you can search for components by name by using FindComponent.
    /// Note FindComponent returns a “TypeNode” which is a simple linked list.
    /// This is used because since we have namespace support the same component name can refer to different classes.
    /// </summary>
    public static class ComponentDatabase
    {
        public class TypeNode : IEnumerable<Type>
        {
            public Type type;
            public TypeNode next = null;

            public IEnumerator<Type> GetEnumerator()
            {
                for (var t = this; t != null; t = t.next)
                    yield return t.type;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private static readonly List<Type> _types;
        private static readonly Dictionary<string, TypeNode> _dict;

        static ComponentDatabase()
        {
            //This static class is anyways reloaded after assembly reload so no need to attach it
            //UnityEditor.AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEvents_afterAssemblyReload;

            var component = typeof(Component);
            var monoBehavior = typeof(MonoBehaviour);
            var hashset = new HashSet<Type>
            {
                //typeof(Component),
                //typeof(Behaviour)
            };

            _dict = new Dictionary<string, TypeNode>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (component.IsAssignableFrom(type) && !type.IsAbstract && !hashset.Contains(type) && type != component)
                    {
                        hashset.Add(type);
                    }
                    else if (monoBehavior.IsAssignableFrom(type) && !hashset.Contains(type) && type != component)
                    {
                        hashset.Add(type);
                    }
                }
            }

            foreach (var type in hashset)
            {
                _dict.TryGetValue(type.Name, out TypeNode tn);
                _dict[type.Name] = ConvertType(type, tn);
            }

            _types = new List<Type>(hashset.Count);
            _types.AddRange(hashset);
        }

        public static TypeNode ConvertType(Type type, TypeNode next = null)
        {
            return new TypeNode { next = next, type = type };
        }

        public static TypeNode FindComponent(string componentName)
        {
            if (_dict.TryGetValue(componentName, out var tn))
                return tn;
            return null;
        }

        public static List<Type> GetTypes(Type baseType)
        {
            var res = new List<Type>();
            foreach (var t in _types)
            {
                if (baseType.IsAssignableFrom(t))
                    res.Add(t);
            }
            return res;
        }

        public static List<Type> GetTypes<T>()
        {
            return GetTypes(typeof(T));
        }

        public static IEnumerable<Type> GetAllTypes()
        {
            return _types.AsReadOnly();
        }
    }
}