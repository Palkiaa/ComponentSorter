using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace CompSorting.Utils
{
    public static class AssetDatabaseUtil
    {
        public static readonly string IconsPath;
        public static readonly AssetBundle EditorAssetBundle;
        private static readonly HashSet<string> _editorAssetsPaths;

        private static readonly HashSet<string> _unityTypes;
        private static readonly Dictionary<string, Texture> _editorTexturesCache;

        private static readonly HashSet<string> _projectTypes;
        private static readonly Dictionary<string, string> _projectScriptPaths;
        private static readonly Dictionary<string, Texture> _projectTexturesCache;

        public static readonly Texture UnityDefaultTexture;
        public static readonly Texture UnknownTexture;

        static AssetDatabaseUtil()
        {
            _unityTypes = GetAllTypesInAssembly(new string[] { "Unity", }).Select(s => s.AssemblyQualifiedName).ToHashSet();
            _editorTexturesCache = new Dictionary<string, Texture>();

            _projectTypes = new HashSet<string>();
            _projectScriptPaths = new Dictionary<string, string>();
            _projectTexturesCache = new Dictionary<string, Texture>();
            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in scripts)
            {
                if (script.GetClass() == null)
                    continue;

                var key = script.GetClass().AssemblyQualifiedName;

                _projectTypes.Add(key);
                _projectScriptPaths.Add(key, AssetDatabase.GetAssetPath(script));
            }

            IconsPath = GetIconsPath();
            EditorAssetBundle = GetEditorAssetBundle();
            _editorAssetsPaths = EditorAssetBundle.GetAllAssetNames().Select(s => s.ToLower()).ToHashSet();
            var path = _editorAssetsPaths.FirstOrDefault(s => s.EndsWith("previewpackageinuse.png"));
            if (!string.IsNullOrWhiteSpace(path))
                UnityDefaultTexture = EditorAssetBundle.LoadAsset<Texture>(path);

            path = _editorAssetsPaths.FirstOrDefault(s => s.EndsWith("d_console.erroricon.sml.png"));
            if (!string.IsNullOrWhiteSpace(path))
                UnknownTexture = EditorAssetBundle.LoadAsset<Texture>(path);
        }

        public static Texture GetAssetImage(SerializedType serializedType)
        {
            var isProject = _projectTypes.Contains(serializedType.AssemblyQualifiedName);
            if (isProject)
                return GetProjectAssetImage(serializedType);

            var isUnityType = _unityTypes.Contains(serializedType.AssemblyQualifiedName);
            if (isUnityType)
                return GetEditorAssetBundleImage(serializedType) ?? UnityDefaultTexture;

            return UnknownTexture;
        }

        public static Texture GetProjectAssetImage(SerializedType serializedType)
        {
            if (_projectTexturesCache.TryGetValue(serializedType.AssemblyQualifiedName, out var texture))
                return texture;

            if (_projectScriptPaths.TryGetValue(serializedType.AssemblyQualifiedName, out var pathu))
            {
                texture = AssetDatabase.GetCachedIcon(pathu);
                _projectTexturesCache.Add(serializedType.AssemblyQualifiedName, texture);
                return texture;
            }

            _projectTexturesCache.Add(serializedType.AssemblyQualifiedName, null);
            return null;
        }

        public static Texture GetEditorAssetBundleImage(SerializedType serializedType)
        {
            if (_editorTexturesCache.TryGetValue(serializedType.AssemblyQualifiedName, out var texture))
                return texture;

            var path = $"/d_{serializedType.Name} icon".ToLower();
            foreach (var assetPath in _editorAssetsPaths)
            {
                if (!assetPath.Contains(path))
                    continue;

                texture = EditorAssetBundle.LoadAsset<Texture>(assetPath);
                _editorTexturesCache.Add(serializedType.AssemblyQualifiedName, texture);
                return texture;
            }

            _editorTexturesCache.Add(serializedType.AssemblyQualifiedName, null);
            return null;
        }

        public static string GetProjectAssetPath(SerializedType serializedType)
        {
            if (_projectScriptPaths.TryGetValue(serializedType.AssemblyQualifiedName, out var pathu))
                return pathu;

            return null;
        }

        private static AssetBundle GetEditorAssetBundle()
        {
            var editorGUIUtility = typeof(EditorGUIUtility);
            var getEditorAssetBundle = editorGUIUtility.GetMethod(
                "GetEditorAssetBundle",
                BindingFlags.NonPublic | BindingFlags.Static);

            return (AssetBundle)getEditorAssetBundle.Invoke(null, null);
        }

        public static string GetIconsPath()
        {
            return EditorResources.iconsPath;
        }

        public static List<Type> GetAllTypesInAssembly(string[] pAssemblyNames)
        {
            List<Type> results = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (string assemblyName in pAssemblyNames)
                {
                    if (assembly.FullName.StartsWith(assemblyName))
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            results.Add(type);
                        }
                        break;
                    }
                }
            }
            return results;
        }
    }
}