using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SettingsManagement;

using UnitySettings = UnityEditor.SettingsManagement.Settings;

namespace CompSorting.Editor
{
    /// <summary>
    /// This class will act as a manager for the <see cref="Settings"/> singleton.
    /// </summary>
    public static class CompSortingRepository
    {
        // Project settings will be stored in a JSON file in a directory matching this name.
        private const string PackageName = "CompSorter";

        private static UnitySettings _instance;

        public static UnitySettings Instance => _instance ??= new UnitySettings(PackageName);

        private const string EnabledKey = "Enabled";
        private const string TypesKey = "Types";

        public static bool GetEnabled()
        {
            return Get(EnabledKey, false);
        }

        public static void SetEnabled(bool enabled)
        {
            Set(EnabledKey, enabled);
        }

        public static List<SerializedType> GetTypes()
        {
            return Get(TypesKey, new List<SerializedType>());
        }

        public static void SetTypes(List<SerializedType> types)
        {
            Set(TypesKey, types);
        }

        #region GetSetStuff

        public static void Save()
        {
            Instance.Save();
        }

        private static T Get<T>(string key, T fallback = default)
        {
            return Instance.Get(key, SettingsScope.Project, fallback);
        }

        private static void Set<T>(string key, T value)
        {
            Instance.Set(key, value, SettingsScope.Project);
        }

        private static bool ContainsKey<T>(string key)
        {
            return Instance.ContainsKey<T>(key, SettingsScope.Project);
        }

        #endregion GetSetStuff
    }
}