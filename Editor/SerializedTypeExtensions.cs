using System;
using System.Collections.Generic;
using System.Linq;

namespace CompSorting.Editor
{
    public static class SerializedTypeExtensions
    {
        public static IEnumerable<Type> ConvertTypeRepsToTypes(this IEnumerable<SerializedType> typeReps)
        {
            return typeReps.Select(s => s.ConvertToType()).Where(s => s != null);
        }
    }
}