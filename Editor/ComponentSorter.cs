using System.Linq;
using CompSorting.Settings;
using UnityEditor;
using UnityEngine;

namespace CompSorting
{
    [InitializeOnLoad]
    public static class ComponentsSorter
    {
        static ComponentsSorter()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        public static void OnSelectionChanged()
        {
            if (Selection.activeGameObject == null || !CompSortingRepository.GetEnabled())
                return;

            OrganiseGameObject(Selection.activeGameObject);
        }

        public static void OrganiseGameObject(GameObject gameObject)
        {
            var objectComponents = gameObject.GetComponents<Component>();
            if (!objectComponents.Any())
                return;

            var sortedComponents = objectComponents.Where(component => component != null && component.GetType() != typeof(Transform)).ToList();

            var settings = CompSortingRepository.GetTypes().ConvertSerializedTypesToSystemTypes().ToList();

            sortedComponents.Sort(new ComponentComparer(settings));

            for (var i = 0; i < sortedComponents.Count; i++)
            {
                var sortedComponent = sortedComponents[i];
                var components = gameObject.GetComponents<Component>()
                    .Where(Component => Component.GetType() != typeof(Transform)).ToList();
                var currentIndex = components.IndexOf(sortedComponent);

                if (currentIndex < i)
                {
                    for (; currentIndex < i; currentIndex++)
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(sortedComponent);
                }
                else
                {
                    for (; currentIndex > i; currentIndex--)
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(sortedComponent);
                }
            }
        }
    }
}