using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

    private static List<Type> m_Types;
    private static Dictionary<string, TypeNode> m_Dict;

    static ComponentDatabase()
    {
        var comp = typeof(Component);
        var hashset = new HashSet<Type>
        {
            typeof(Component),
            typeof(Behaviour)
        };

        m_Dict = new Dictionary<string, TypeNode>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (comp.IsAssignableFrom(type) && !type.IsAbstract && !hashset.Contains(type) && type != comp)
                {
                    hashset.Add(type);
                }
            }
        }

        foreach (var type in hashset)
        {
            m_Dict.TryGetValue(type.Name, out TypeNode tn);
            m_Dict[type.Name] = ConvertType(type, tn);
        }

        m_Types = new List<Type>(hashset.Count);
        m_Types.AddRange(hashset);
    }

    public static TypeNode ConvertType(Type type, TypeNode next = null)
    {
        return new TypeNode { next = next, type = type };
    }

    public static TypeNode FindComponent(string aComponentName)
    {
        if (m_Dict.TryGetValue(aComponentName, out var tn))
            return tn;
        return null;
    }

    public static List<Type> GetTypes(Type aBaseType)
    {
        var res = new List<Type>();
        foreach (var t in m_Types)
        {
            if (aBaseType.IsAssignableFrom(t))
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
        return m_Types.AsReadOnly();
    }
}