using System;
using System.Collections.Generic;
using System.Linq;

namespace CompSorting
{
    public static class SerializedTypeExtensions
    {
        public static IEnumerable<Type> ConvertSerializedTypesToSystemTypes(this IEnumerable<SerializedType> typeReps)
        {
            return typeReps.Select(s => s.ConvertToType()).Where(s => s != null);
        }
    }
}