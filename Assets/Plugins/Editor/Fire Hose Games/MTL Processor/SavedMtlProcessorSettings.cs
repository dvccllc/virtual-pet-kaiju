//--------------------------------------
//           .MTL Processor           
//  Copyright © 2016 Fire Hose Games  
//--------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FHGTools
{
    public class SavedMtlProcessorSettings : ScriptableObject
    {
#region Saved Info
        [System.Serializable]
        public class SerializedMtlSettings
        {
            public ModelImporter assetRef;
            public bool shouldProcessMaterials;
            [System.NonSerialized] public bool shouldProcessMaterialsMixed = false;
            public bool shouldOverrideMaterial;
            [System.NonSerialized] public bool shouldOverrideMaterialMixed = false;

            public SerializedMtlSettings Clone()
            {
                SerializedMtlSettings ret = new SerializedMtlSettings();
                ret.assetRef = assetRef;
                ret.shouldProcessMaterials = shouldProcessMaterials;
                ret.shouldOverrideMaterial = shouldOverrideMaterial;
                return ret; 
            }

            public void CombineProperties(SerializedMtlSettings other)
            {
                if (other.shouldProcessMaterials != shouldProcessMaterials || other.shouldProcessMaterialsMixed)
                {
                    shouldProcessMaterials = false;
                    shouldProcessMaterialsMixed = true;
                }
                if (other.shouldOverrideMaterial != shouldOverrideMaterial || other.shouldOverrideMaterialMixed)
                {
                    shouldOverrideMaterial = false;
                    shouldOverrideMaterialMixed = true;
                }
            }
        }
#endregion

#region Fields
        public const string ASSET_NAME = "All MTL Processor Saved Settings.asset";
        public const string TOOLTIP_shouldProcessMaterials = "When checked, will convert imported OBJ material file to Unity Standard Shader.";
        public const string TOOLTIP_shouldOverrideMaterial = "WARNING: When checked, will overwrite existing changes to materials that have already been converted.";
        public const string TOOLTIP_showLoggingInfo = "Display debug logging information to the console.";
        public const string TOOLTIP_AlphizerFileSuffix = "Naming format for Alph-ized versions of textures added at the end of the filename of the created file";
        private static bool IS_FRESH_INSTALL = false;

        [HideInInspector]
        public List<SerializedMtlSettings> allSavedInfo = new List<SerializedMtlSettings>();
        [Header("Default MTL Import Settings")]
        [Tooltip(TOOLTIP_shouldProcessMaterials)]
        public bool shouldProcessMaterials = true;
        [Tooltip(TOOLTIP_shouldOverrideMaterial)]
        public bool shouldOverrideMaterial = false;
        [Header("Settings")]
        [Tooltip(TOOLTIP_showLoggingInfo)]
        public bool showLoggingInfo = false;
        [Tooltip(TOOLTIP_AlphizerFileSuffix)]
        public string AlphizerFileSuffix = "_ALPHIZED";
#endregion

#region Instance info
        private static SavedMtlProcessorSettings _Instance = null;

        public static SavedMtlProcessorSettings Instance {
            get {
                if (_Instance == null)
                    CreateInstance();
                return _Instance;
            }
        }

        [InitializeOnLoadMethod]
        private static void CheckInitialInstall()
        {
            if (Instance != null && IS_FRESH_INSTALL)
            {
                IS_FRESH_INSTALL = false;
                const string MSG_TEXT = "Would you like to process the MTL files on the models that have already been imported?";
                if (EditorUtility.DisplayDialog("MTL Processor", MSG_TEXT, "Yes", "No"))
                    Instance.ProcessAllModels();
            }
        }

        private static void CreateInstance()
        {
            string[] foundMatching = AssetDatabase.FindAssets("t:SavedMtlProcessorSettings");
            if (foundMatching.Length > 0)
            {
                string foundAssetPath = AssetDatabase.GUIDToAssetPath(foundMatching[0]);
                _Instance = AssetDatabase.LoadAssetAtPath<SavedMtlProcessorSettings>(foundAssetPath);
                _Instance.ClearDeprecated();
            }
            if (_Instance == null)
            {
                _Instance = ScriptableObject.CreateInstance<SavedMtlProcessorSettings>();
                MonoScript ms = MonoScript.FromScriptableObject(_Instance);
                string assetPath;
                if (ms != null)
                    assetPath = AssetDatabase.GetAssetPath(ms).Replace(ms.name + ".cs", ASSET_NAME);
                else
                    assetPath = "Assets/" + ASSET_NAME;
                AssetDatabase.CreateAsset(_Instance, assetPath);
                IS_FRESH_INSTALL = true;
                EditorApplication.delayCall += CheckInitialInstall;
            }
        }
#endregion

#region Other functions
        public SerializedMtlSettings SetDefaultSettings(ModelImporter key)
        {
            SerializedMtlSettings curVal = this[key];
            if (curVal == null)
                curVal = new SerializedMtlSettings();
            curVal.assetRef = key;
            curVal.shouldProcessMaterials = shouldProcessMaterials;
            curVal.shouldOverrideMaterial = shouldOverrideMaterial;
            this[key] = curVal;
            return curVal;
        }

        private void ClearDeprecated()
        {
            int numRemoved = allSavedInfo.RemoveAll((SerializedMtlSettings sms) =>
            {
                return sms.assetRef == null;
            });
            if (numRemoved > 0)
                EditorUtility.SetDirty(this);
        }
        
        public void ResetToDefault(List<SerializedMtlSettings> objsToClear)
        {
            int numRemoved = allSavedInfo.RemoveAll((SerializedMtlSettings sms) => {
                return objsToClear.Contains(sms);
            });
            if (numRemoved > 0)
                EditorUtility.SetDirty(this);
        }

        public void ProcessAllModels()
        {
            string[] allModels = AssetDatabase.FindAssets("t:Model");
            foreach(string guid in allModels)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadMainAssetAtPath(assetPath.Substring(0, assetPath.Length - 3) + "mtl"))
                {
                    ModelImporter importer = ModelImporter.GetAtPath(assetPath) as ModelImporter;
                    MtlAssetProcessor.ProcessMaterials(importer);
                    EditorUtility.SetDirty(importer);
                }
            }
        }
        
        public void ApplyDefaultSettingsOnAllModels()
        {
            allSavedInfo.Clear();
            string[] allModels = AssetDatabase.FindAssets("t:Model");
            foreach(string guid in allModels)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadMainAssetAtPath(assetPath.Substring(0, assetPath.Length - 3) + "mtl"))
                {
                    ModelImporter importer = ModelImporter.GetAtPath(assetPath) as ModelImporter;
                    if (importer != null)
                        SetDefaultSettings(importer);
                }
            }
        }

        public SerializedMtlSettings this[ModelImporter key] {
            get {
                int index = allSavedInfo.FindIndex((SerializedMtlSettings info) =>
                {
                    return info.assetRef == key;
                });
                return (index >= 0) ? allSavedInfo[index] : null;
            }
            set {
                int index = allSavedInfo.FindIndex((SerializedMtlSettings info) =>
                {
                    return info.assetRef == key;
                });
                allSavedInfo.RemoveAll((SerializedMtlSettings info) =>
                {
                    return info.assetRef == null;
                });
                if (index >= 0)
                    allSavedInfo[index] = value;
                else
                    allSavedInfo.Add(value);
                EditorUtility.SetDirty(this);
            }
        }
#endregion
    }

    public class MtlOptionsWindow : EditorWindow
    {
#region Fields
        public const float WINDOW_HEIGHT = 163f;
        public const float WINDOW_WIDTH = 220f;
        
        static MtlOptionsWindow windowInstance = null;
#endregion
        
        [MenuItem("Window/MTL Processor/Options")]
        public static void Init()
        {
            if (windowInstance == null)
            {
                windowInstance = EditorWindow.CreateInstance<MtlOptionsWindow>();
                windowInstance.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            }
            windowInstance.titleContent = new GUIContent("MTL Options");
            windowInstance.Show();
        }
        
        private void OnGUI()
        {
            SavedMtlProcessorSettings settings = SavedMtlProcessorSettings.Instance;
            Undo.RecordObject(settings, "Updated MTL Processor settings");
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Label("Default MTL Import Settings", EditorStyles.boldLabel);
                settings.shouldProcessMaterials = EditorGUILayout.ToggleLeft(
                    new GUIContent("Process .MTL's", SavedMtlProcessorSettings.TOOLTIP_shouldProcessMaterials),
                    settings.shouldProcessMaterials);
                settings.shouldOverrideMaterial = EditorGUILayout.ToggleLeft(
                    new GUIContent("Override materials", SavedMtlProcessorSettings.TOOLTIP_shouldOverrideMaterial),
                    settings.shouldOverrideMaterial);
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(settings);

            if (GUILayout.Button(new GUIContent("Apply these settings to all models", "Applies settings from this window to every model in the project"), EditorStyles.toolbarButton) && settings.allSavedInfo.Count > 0)
            {
                Undo.RecordObject(settings, "Cleared all custom MTL Processor settings");
                settings.ApplyDefaultSettingsOnAllModels();
                EditorUtility.SetDirty(settings);
            }
            if (GUILayout.Button(new GUIContent("Process materials for all models", "Process materials for all models"), EditorStyles.toolbarButton))
            {
                settings.ProcessAllModels();
            }

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Label("Settings", EditorStyles.boldLabel);
                settings.showLoggingInfo = EditorGUILayout.ToggleLeft(
                    new GUIContent("Show logging info", SavedMtlProcessorSettings.TOOLTIP_showLoggingInfo),
                    settings.showLoggingInfo);
                settings.AlphizerFileSuffix = EditorGUILayout.TextField(
                    new GUIContent("Alph-izer file suffix", SavedMtlProcessorSettings.TOOLTIP_AlphizerFileSuffix),
                    settings.AlphizerFileSuffix);
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(settings);
        }
    }
}