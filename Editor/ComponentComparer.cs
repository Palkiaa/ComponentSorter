using System;
using System.Collections.Generic;

using UnityEngine;

namespace CompSorting
{
    public class ComponentComparer : IComparer<Component>
    {
        private readonly IEnumerable<Type> TypeOrder;

        public ComponentComparer(IEnumerable<Type> typeOrder)
        {
            TypeOrder = typeOrder;
        }

        private int GetIndex(Component Component)
        {
            var componentType = Component.GetType();

            Type bestMatch = typeof(UnityEngine.Object);
            var bestIndex = int.MaxValue;

            var index = 0;
            foreach (var type in TypeOrder)
            {
                // If we found the exact type in the list, then this is the right index.
                if (componentType == type)
                    return index;

                // If we found a parent, then we switch to its place if it is more
                // "recent" (in the inheritance tree) than previously found parents.
                if (componentType.IsSubclassOf(type))
                {
                    if (type.IsSubclassOf(bestMatch))
                    {
                        bestMatch = type;
                        bestIndex = index;
                    }
                }

                index++;
            }

            return bestIndex;
        }

        public int Compare(Component First, Component Second)
        {
            return Comparer<int>.Default.Compare(GetIndex(First), GetIndex(Second));
        }
    }
}