using System.Collections.Generic;
using UnityEditor;

using UnitySettings = UnityEditor.SettingsManagement.Settings;

namespace CompSorting
{
    /// <summary>
    /// This class will act as a manager for the <see cref="Settings"/> singleton.
    /// </summary>
    public static class CompSortingRepository
    {
        private static UnitySettings _instance;

        public static UnitySettings Instance => _instance ??= new UnitySettings(PackageInfo.PackageName);

        private const string _enabledKey = "Enabled";
        private const string _typesKey = "Types";

        public static bool GetEnabled()
        {
            return Get(_enabledKey, false);
        }

        public static void SetEnabled(bool enabled)
        {
            Set(_enabledKey, enabled);
        }

        public static List<SerializedType> GetTypes()
        {
            return Get(_typesKey, new List<SerializedType>());
        }

        public static void SetTypes(List<SerializedType> types)
        {
            Set(_typesKey, types);
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