using System;
using System.Reflection;
using UnityEditor;

namespace CompSorting.Utils
{
    //Source: https://forum.unity.com/threads/serialiedproperty-check-if-it-has-a-propertyattribute.436103/#post-8359626
    public static class EditorUtils
    {
        private const BindingFlags _allBindingFlags = (BindingFlags)(-1);

        /// <summary>
        /// Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>.
        /// </summary>
        public static TAttribute[] GetAttributes<TAttribute>(this SerializedProperty serializedProperty, bool inherit)
            where TAttribute : Attribute
        {
            if (serializedProperty == null)
            {
                throw new ArgumentNullException(nameof(serializedProperty));
            }

            var targetObjectType = serializedProperty.serializedObject.targetObject.GetType();

            if (targetObjectType == null)
            {
                throw new ArgumentException($"Could not find the {nameof(targetObjectType)} of {nameof(serializedProperty)}");
            }

            foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
            {
                var fieldInfo = targetObjectType.GetField(pathSegment, _allBindingFlags);
                if (fieldInfo != null)
                {
                    return (TAttribute[])fieldInfo.GetCustomAttributes<TAttribute>(inherit);
                }

                var propertyInfo = targetObjectType.GetProperty(pathSegment, _allBindingFlags);
                if (propertyInfo != null)
                {
                    return (TAttribute[])propertyInfo.GetCustomAttributes<TAttribute>(inherit);
                }
            }

            throw new ArgumentException($"Could not find the field or property of {nameof(serializedProperty)}");
        }
    }
}