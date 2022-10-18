using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;
using UnityEngine.UIElements;

namespace CompSorting.Editor
{
    public class CompSortingSettingsProvider : SettingsProvider
    {
        private CompSortingSettings compSortingSettings;
        private SerializedObject m_CustomSettings;
        private ReorderableList reordableList;

        private IEnumerable<Type> allTypes;
        public static bool dirty;

        private class Styles
        {
            public static GUIContent types = new GUIContent("Component Types");
            public static GUIContent enabled = new GUIContent("Enabled");
            public static GUIContent reset = new GUIContent("Reset to defaults");
        }

        public CompSortingSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            compSortingSettings = ScriptableObject.CreateInstance<CompSortingSettings>();
            compSortingSettings.Enabled = CompSortingRepository.GetEnabled();
            compSortingSettings.Types = CompSortingRepository.GetTypes();

            m_CustomSettings = new SerializedObject(compSortingSettings);

            reordableList = new ReorderableList(m_CustomSettings, m_CustomSettings.FindProperty(nameof(CompSortingSettings.Types)))
            {
                drawElementCallback = DrawListItems,
                drawHeaderCallback = DrawHeader,
                onAddCallback = AddItem
            };
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reordableList.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Styles.types);
        }

        private void AddItem(ReorderableList reorderableList)
        {
            reorderableList.serializedProperty.InsertArrayElementAtIndex(reordableList.count);

            var type = new SerializedType(allTypes.FirstOrDefault());

            var prop = reordableList.serializedProperty.GetArrayElementAtIndex(reordableList.count - 1);

            prop.FindPropertyRelative(nameof(SerializedType.Name)).stringValue = type.Name;
            prop.FindPropertyRelative(nameof(SerializedType.AssemblyQualifiedName)).stringValue = type.AssemblyQualifiedName;
        }

        public override void OnGUI(string searchContext)
        {
            var obj = (CompSortingSettings)m_CustomSettings.targetObject;

            m_CustomSettings.Update();

            if (GUILayout.Button(Styles.reset))
            {
                obj.Types = CompSortingSettings.defaultOrder.Select(s => new SerializedType(s)).ToList();
                dirty = true;
            }

            EditorGUILayout.Space();

            //text = EditorGUILayout.TextField(Styles.number, text);
            allTypes = ComponentDatabase.GetAllTypes();
            allTypes = allTypes.Where(s => !obj.Types.Any(t => t.Name == s.Name)).ToList();

            SerializedTypeDrawer.options = allTypes.Select(s => s.Name).ToList();

            EditorGUI.BeginChangeCheck();

            obj.Enabled = EditorGUILayout.Toggle(Styles.enabled, obj.Enabled);

            EditorGUILayout.Space();

            reordableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck() || dirty)
            {
                m_CustomSettings.ApplyModifiedProperties();

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
            var provider = new CompSortingSettingsProvider("Project/CompSorting", SettingsScope.Project)
            {
                // Automatically extract all keywords from the Styles.
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;
        }
    }
}