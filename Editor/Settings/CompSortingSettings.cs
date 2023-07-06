using System;
using System.Collections.Generic;
using UnityEngine;

namespace CompSorting.Settings
{
    internal class CompSortingSettings : ScriptableObject
    {
        public static readonly List<Type> defaultOrder = new()
        {
            // Transform is always first (though that doesn't really matter, as we can't move it anyway).
            typeof(Transform),

            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(SpriteRenderer),

            typeof(Rigidbody),
            typeof(Collider),

            typeof(Component),
            typeof(Behaviour),
            typeof(MonoBehaviour),
        };

        public bool Enabled;

        [CustomSerializedFieldOptions]
        public List<SerializedType> Types;
    }
}