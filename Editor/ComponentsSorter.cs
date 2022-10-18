using System.Linq;

using CompSorting.Editor;

using UnityEditor;

using UnityEngine;

namespace CompSorting
{
    //https://answers.unity.com/questions/31784/changing-the-order-of-components.html
    [InitializeOnLoad]
    public static class ComponentsSorter
    {
        static ComponentsSorter()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        public static void OnSelectionChanged()
        {
            if (Selection.activeGameObject != null && CompSortingRepository.GetEnabled())
            {
                ReOrderGameObject(Selection.activeGameObject);
            }
        }

        public static void ReOrderGameObject(GameObject gameObject)
        {
            var initialComponents = gameObject.GetComponents<Component>();
            if (!initialComponents.Any())
            {
                return;
            }

            var sortedComponents = initialComponents.Where(Component => Component.GetType() != typeof(Transform)).ToList();

            var settings = CompSortingRepository.GetTypes().ConvertTypeRepsToTypes().ToList();

            sortedComponents.Sort(new ComponentComparer(settings));

            for (var i = 0; i < sortedComponents.Count; i++)
            {
                var sortedComponent = sortedComponents[i];
                var components = gameObject.GetComponents<Component>()
                    .Where(Component => Component.GetType() != typeof(Transform)).ToList();
                var currentIndex = components.IndexOf(sortedComponent);
                if (currentIndex < i)
                {
                    for (var moveIndex = currentIndex; moveIndex < i; moveIndex++)
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(sortedComponent);
                }
                else
                {
                    for (var MoveIndex = currentIndex; MoveIndex > i; MoveIndex--)
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(sortedComponent);
                }
            }
        }
    }
}