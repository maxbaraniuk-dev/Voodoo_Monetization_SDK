#if UNITY_EDITOR
using System.IO;
using Runtime.PublicAPI.Core;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

namespace Editor
{
    public class MonetizationEditorWindow : EditorWindow
    {
        private const string ConfigLoadPath = "Assets/Resources/monetizationConfig.json";
        
        ConfigData _config;
        
        [MenuItem("Tools/Monetization configuration")]
        public static void OpenWindow()
        {
            var w = GetWindow<MonetizationEditorWindow>("Monetization config Editor");
            w.minSize = new Vector2(500, 240);
        }

        private void OnEnable()
        {
            LoadFromFile();
        }

        private void OnGUI()
        {
            if (_config == null)
                return;
            
            EditorGUILayout.Space();
            _config.appId = EditorGUILayout.TextField("Application Id", _config.appId, GUILayout.Width(450));
            
            if (GUI.changed)
                SaveToFile();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(ConfigLoadPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigLoadPath) ?? string.Empty);
                _config = new ConfigData();
                SaveToFile();
            }
            
            _config = JsonUtility.FromJson<ConfigData>(File.ReadAllText(ConfigLoadPath));
            if (_config == null)
            {
                Debug.LogError("Failed to load config from file");
                _config = new ConfigData();
                SaveToFile();
            }
            
            Repaint();
        }

        private void SaveToFile()
        {
            var json = JsonUtility.ToJson(_config);
            File.WriteAllText(ConfigLoadPath, json);
        }
    }
}
#endif