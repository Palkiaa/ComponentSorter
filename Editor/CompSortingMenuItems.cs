
using UnityEditor;

namespace CompSorting.Editor
{
    public static class CompSortingMenuItems
    {
        public const string SortComponentsShortcut = "%#`";

        [MenuItem("Edit/Sort Components " + SortComponentsShortcut)]
        private static void SortComponents()
        {
            var selectedGameObjects = Selection.gameObjects;
            foreach (var selectedGameObject in selectedGameObjects)
            {
                if (selectedGameObject == null)
                    return;

                ComponentsSorter.ReOrderGameObject(selectedGameObject);
            }
        }
    }
}