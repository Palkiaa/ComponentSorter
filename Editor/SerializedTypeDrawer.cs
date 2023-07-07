using CompSorting.Settings;
using CompSorting.Utils;
using RoboRyanTron.SearchableEnum.Editor;
using System.Linq;
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

        private static SerializedType[] _defaultOptions = new SerializedType[0];

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

            var componentName = serializedType.Name;

            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height = EditorGUIUtility.singleLineHeight;

            var buttonArea = new Rect(position)
            {
                width = position.width * 0.75f
            };

            var imageStyle = new GUIStyle()
            {
                imagePosition = ImagePosition.ImageOnly,
                alignment = TextAnchor.MiddleCenter
            };

            var locateRect = new Rect(position)
            {
                x = buttonArea.x + buttonArea.width + EditorGUIUtility.standardVerticalSpacing,
                width = position.height
            };
            var locateScript = new GUIContent(EditorGUIUtility.IconContent("d_pick"))
            {
                tooltip = "Go to file"
            };

            var infoRect = new Rect(locateRect)
            {
                x = locateRect.x + locateRect.width + EditorGUIUtility.standardVerticalSpacing,
            };
            var infoContent = new GUIContent(EditorGUIUtility.IconContent("d__Help"))
            {
                tooltip = serializedType.AssemblyQualifiedName
            };

            GUI.Label(infoRect, infoContent, imageStyle);

            var assetPath = AssetDatabaseUtil.GetProjectAssetPath(serializedType);
            GUI.enabled = !string.IsNullOrWhiteSpace(assetPath);

            if (GUI.Button(locateRect, locateScript, imageStyle))
            {
                var @object = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                FocusProjectAsset(@object);
            }
            GUI.enabled = true;

            int id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, buttonArea);

            label = EditorGUI.BeginProperty(buttonArea, label, prop);
            buttonArea = EditorGUI.PrefixLabel(buttonArea, id, label);

            var buttonText = new GUIContent(componentName)
            {
                image = AssetDatabaseUtil.GetAssetImage(serializedType)
            };

            if (DropdownButton(id, buttonArea, buttonText))
            {
                var options = optionsAttribute?.GetOptions(serializedType) ?? _defaultOptions;
                int selected = options.ToList().FindIndex(s => s == serializedType);

                SearchablePopup.Show(buttonArea, options, selected, i =>
                {
                    var newComponentName = options[i];

                    assemblyQualifiedName.stringValue = newComponentName.AssemblyQualifiedName;
                    nameProp.stringValue = newComponentName.Name;

                    prop.serializedObject.ApplyModifiedProperties();
                    CompSortingSettingsProvider.dirty = true;
                });
            }

            EditorGUI.EndProperty();
        }

        private void FocusProjectAsset(Object @object)
        {
            var projectBrowserWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            if (projectBrowserWindowType != null)
            {
                var window = EditorWindow.GetWindow(projectBrowserWindowType);
                if (window != null)
                    window.Focus();
            }
            // Select the object in the project folder
            //Selection.activeObject = @object;
            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(@object);
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