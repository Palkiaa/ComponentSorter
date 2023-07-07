using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace CompSorting.Settings
{
    public class CompSortingSettingsProvider : SettingsProvider
    {
        private CompSortingSettings _compSortingSettings;
        private SerializedObject _customSettings;
        private ReorderableList _reorderableList;

        public static bool dirty;

        private class Styles
        {
            public static GUIContent types = new("Component Types");
            public static GUIContent enabled = new("Enabled");
            public static GUIContent reset = new("Reset to defaults");
        }

        public CompSortingSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            CompSortingRepository.Instance.afterSettingsSaved += Instance_afterSettingsSaved;
            Load();
        }

        public override void OnDeactivate()
        {
            if (CompSortingRepository.Instance != null)
                CompSortingRepository.Instance.afterSettingsSaved -= Instance_afterSettingsSaved;
        }

        private void Load()
        {
            _compSortingSettings = ScriptableObject.CreateInstance<CompSortingSettings>();
            _compSortingSettings.Enabled = CompSortingRepository.GetEnabled();
            _compSortingSettings.Types = CompSortingRepository.GetTypes();

            _customSettings = new SerializedObject(_compSortingSettings);

            _reorderableList = new ReorderableList(_customSettings, _customSettings.FindProperty(nameof(CompSortingSettings.Types)))
            {
                drawElementCallback = DrawListItems,
                drawHeaderCallback = DrawHeader,
                onAddCallback = AddItem
            };
        }

        private void Instance_afterSettingsSaved()
        {
            Load();
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Styles.types);
        }

        private void AddItem(ReorderableList reorderableList)
        {
            reorderableList.serializedProperty.InsertArrayElementAtIndex(_reorderableList.count);

            var type = CustomSerializedFieldOptionsAttribute.FirstUnselected();
            if (type == null)
                return;

            var serializedType = new SerializedType(type);

            var prop = _reorderableList.serializedProperty.GetArrayElementAtIndex(_reorderableList.count - 1);

            prop.FindPropertyRelative(nameof(SerializedType.Name)).stringValue = serializedType.Name;
            prop.FindPropertyRelative(nameof(SerializedType.AssemblyQualifiedName)).stringValue = serializedType.AssemblyQualifiedName;
        }

        public override void OnGUI(string searchContext)
        {
            var obj = (CompSortingSettings)_customSettings.targetObject;

            _customSettings.Update();

            if (GUILayout.Button(Styles.reset))
            {
                obj.Types = CompSortingSettings.defaultOrder.Select(s => new SerializedType(s)).ToList();
                dirty = true;
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            obj.Enabled = EditorGUILayout.Toggle(Styles.enabled, obj.Enabled);

            EditorGUILayout.Space();

            _reorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck() || dirty)
            {
                _customSettings.ApplyModifiedProperties();

                CompSortingRepository.SetEnabled(obj.Enabled);
                CompSortingRepository.SetTypes(obj.Types);
                CompSortingRepository.Save();

                dirty = false;
            }
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new CompSortingSettingsProvider($"Project/{PackageInfo.DisplayName}", SettingsScope.Project)
            {
                // Automatically extract all keywords from the Styles.
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;
        }
    }
}