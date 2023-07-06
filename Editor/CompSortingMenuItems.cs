using UnityEditor;

namespace CompSorting
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
                    continue;

                ComponentsSorter.OrganiseGameObject(selectedGameObject);
            }
        }
    }
}