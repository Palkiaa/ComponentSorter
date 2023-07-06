using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace CompSorting
{
    public static class AssetDatabaseUtil
    {
        public static readonly string IconsPath;
        public static readonly AssetBundle EditorAssetBundle;

        private static readonly string[] _editorPaths;
        private static readonly string[] _projectPaths;

        private static readonly Dictionary<string, Texture> _editorCache;

        private static readonly Dictionary<string, Texture> _projectCache;

        public static readonly Texture DefaultTexture;

        static AssetDatabaseUtil()
        {
            IconsPath = GetIconsPath();
            EditorAssetBundle = GetEditorAssetBundle();
            _editorPaths = EditorAssetBundle.GetAllAssetNames();
            _editorCache = new Dictionary<string, Texture>();
            _projectCache = new Dictionary<string, Texture>();

            _projectPaths = AssetDatabase.GetAllAssetPaths();

            var path = _editorPaths.FirstOrDefault(s => s.ToLower().Contains("previewpackageinuse"));
            if (!string.IsNullOrWhiteSpace(path))
                DefaultTexture = EditorAssetBundle.LoadAsset<Texture>(path);
        }

        public static Texture GetAssetImage(string assetName)
        {
            return GetEditorAssetBundleImage(assetName)
                ?? GetProjectAssetImage($"{assetName}.cs")
                ?? DefaultTexture;
        }

        public static Texture GetProjectAssetImage(string assetName)
        {
            if (_projectCache.TryGetValue(assetName, out var texture))
                return texture;

            foreach (var assetPath in _projectPaths)
            {
                var fileName = Path.GetFileName(assetPath);
                if (fileName != assetName)
                    continue;

                texture = AssetDatabase.GetCachedIcon(assetPath);
                _projectCache.Add(assetName, texture);
                return texture;
            }

            _projectCache.Add(assetName, null);
            return null;
        }

        public static Texture GetPackageAssetImage(string assetName)
        {
            if (_projectCache.TryGetValue(assetName, out var texture))
                return texture;

            foreach (var assetPath in _projectPaths)
            {
                var fileName = Path.GetFileName(assetPath);
                if (fileName != assetName)
                    continue;

                texture = AssetDatabase.GetCachedIcon(assetPath);
                _projectCache.Add(assetName, texture);
                return texture;
            }

            _projectCache.Add(assetName, null);
            return null;
        }

        public static Texture GetEditorAssetBundleImage(string name)
        {
            if (_editorCache.TryGetValue(name, out var texture))
                return texture;

            foreach (var assetPath in _editorPaths)
            {
                if (!assetPath.ToLower().Contains($"/d_{name} icon".ToLower()))
                    continue;

                texture = EditorAssetBundle.LoadAsset<Texture>(assetPath);
                _editorCache.Add(name, texture);
                return texture;
            }

            _editorCache.Add(name, null);
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
    }
}