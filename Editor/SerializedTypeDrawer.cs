using System.Collections.Generic;
using System.Linq;
using CompSorting.Settings;
using CompSorting.Utils;
using RoboRyanTron.SearchableEnum.Editor;
using UnityEditor;
using UnityEngine;

namespace CompSorting
{
    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private static readonly int _idHash = "SearchableEnumDrawer".GetHashCode();

        public static List<string> Options = new();

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        static SerializedTypeDrawer()
        {
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var assemblyQualifiedName = prop.FindPropertyRelative(nameof(SerializedType.AssemblyQualifiedName));
            var nameProp = prop.FindPropertyRelative(nameof(SerializedType.Name));

            BaseSerializedTypeOptionsAttribute optionsAttribute = null;

            var attributes = prop.GetAttributes<BaseSerializedTypeOptionsAttribute>(true);
            if (attributes.Any())
                optionsAttribute = attributes.First();

            var serializedType = new SerializedType(nameProp.stringValue, assemblyQualifiedName.stringValue);

            var options = optionsAttribute?.GetOptions(serializedType).ToList() ?? Options;

            var componentName = nameProp.stringValue;

            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height = EditorGUIUtility.singleLineHeight;

            int selected = options.IndexOf(componentName);
            int oldSelection = selected;

            var buttonArea = new Rect(position);

            int id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, buttonArea);

            label = EditorGUI.BeginProperty(buttonArea, label, prop);
            buttonArea = EditorGUI.PrefixLabel(buttonArea, id, label);

            // If the enum has changed, a blank entry // Not in this case we don't! The option may no longer be available, so still show the text.
            if (selected < 0 || Options.Count <= selected)
            {
                componentName = nameProp.stringValue;
            }
            else
            {
                componentName = Options[selected];
            }
            var type = ComponentDatabase.FindComponent(componentName);

            var buttonText = new GUIContent(componentName)
            {
                image = AssetDatabaseUtil.GetAssetImage(componentName),
                tooltip = assemblyQualifiedName.stringValue
            };

            if (DropdownButton(id, buttonArea, buttonText))
            {
                SearchablePopup.Show(buttonArea, options.ToArray(), selected, i =>
                                {
                                    selected = i;
                                    var newComponentName = options[i];

                                    nameProp.stringValue = newComponentName;

                                    var typeNode = ComponentDatabase.FindComponent(newComponentName);
                                    if (typeNode != null && typeNode.type != null)
                                        assemblyQualifiedName.stringValue = typeNode.type.AssemblyQualifiedName;

                                    prop.serializedObject.ApplyModifiedProperties();
                                    CompSortingSettingsProvider.dirty = true;
                                });
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;

                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }
}