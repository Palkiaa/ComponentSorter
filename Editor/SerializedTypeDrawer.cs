using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RoboRyanTron.SearchableEnum.Editor;

using UnityEditor;

using UnityEngine;

namespace CompSorting.Editor
{
    //https://answers.unity.com/questions/1549215/getting-all-component-types-even-those-not-on-the.html

    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        //private const float _height = ;

        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private int idHash;

        public static List<string> options = new List<string>();

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private IEnumerable<Type> UnityComponentTypes()
        {
            Assembly unityAssembly = Assembly.GetAssembly(typeof(Component));
            return unityAssembly.GetTypes().Where(s => s.IsSubclassOf(typeof(Component)));
        }

        private IEnumerable<Type> AllComponentTypes()
        {
            return ComponentDatabase.GetAllTypes();
        }

        private List<string> MapOptions(IEnumerable<Type> types)
        {
            return types.Select(s => s.Name).ToList();
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var valueProp = prop.FindPropertyRelative(nameof(SerializedType.Name));
            int selected = options.IndexOf(valueProp.stringValue);
            int oldSelection = selected;

            if (idHash == 0) idHash = "SearchableEnumDrawer".GetHashCode();
            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);


            label = EditorGUI.BeginProperty(position, label, prop);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
            // If the enum has changed, a blank entry // Not in this case we don't! The option may no longer be available, so still show the text.
            if (selected < 0 || options.Count <= selected)
            {
                buttonText = new GUIContent(valueProp.stringValue);
            }
            else
            {
                buttonText = new GUIContent(options[selected]);
            }

            if (DropdownButton(id, position, buttonText))
            {
                SearchablePopup.Show(position, options.ToArray(), selected, i =>
                                {
                                    selected = i;
                                    valueProp.stringValue = options[i];
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