//--------------------------------------
//           .MTL Processor           
//  Copyright © 2016 Fire Hose Games  
//--------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FHGTools
{
    [CustomEditor(typeof(ModelImporter))][CanEditMultipleObjects]
    public class ModelImporterInspectorWithMtlOptions : Editor
    {
#region Original Inspector
        bool _foldoutOpened = true;
        private Editor m_ActiveEditor = null;
        private Editor _objPreview = null;
        private int m_ActiveEditorIndex = 0;
        protected System.Type[] m_SubEditorTypes;
        protected string[] m_SubEditorNames;
        private System.Type _oldType = System.Type.GetType("UnityEditor.ModelInspector,UnityEditor");

        public System.Type oldType {
            get {
                if (_oldType == null)
                    _oldType = System.Type.GetType("UnityEditor.ModelInspector,UnityEditor");
                return _oldType;
            }
        }

        public Editor objPreview {
            get {
                if (_objPreview == null)
                    InitObjPreview();
                return _objPreview;
            }
        }

        public Editor activeEditor {
            get {
                if (m_ActiveEditor == null)
                    m_ActiveEditor = Editor.CreateEditor(this.targets, oldType);
                return m_ActiveEditor;
            }
        }
#endregion

#region Inspector override
        public override void OnPreviewSettings()
        {
            objPreview.OnPreviewSettings();
        }

        protected override void OnHeaderGUI()
        {
            Rect r = EditorGUILayout.BeginHorizontal();
            base.OnHeaderGUI();
            EditorGUILayout.EndHorizontal();
            // For some reason, the base implementation only renders the title when we have multiple objects selected
            if (objPreview.target != null && targets.Length == 1)
            {
                Rect labelRect = new Rect(r.min.x + 44f, r.min.y + 4f, r.size.x - 86f, 20f);
                string title = string.Format("{0} Import Settings", objPreview.target.name);
                GUI.Label(labelRect, title, EditorStyles.largeLabel);
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            objPreview.OnInteractivePreviewGUI(r, background);
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        private void OnEnable()
        {
            if (this.m_SubEditorTypes == null)
            {
                this.m_SubEditorTypes = new System.Type[] {
                    System.Type.GetType("UnityEditor.ModelImporterModelEditor,UnityEditor"),
                    System.Type.GetType("UnityEditor.ModelImporterRigEditor,UnityEditor"),
                    System.Type.GetType("UnityEditor.ModelImporterClipEditor,UnityEditor")
                };
                this.m_SubEditorNames = new string[] {
                    "Model",
                    "Rig",
                    "Animations"
                };
            }
            this.m_ActiveEditorIndex = EditorPrefs.GetInt(base.GetType().Name + "ActiveEditorIndex", 0);
            if (m_ActiveEditor != null)
            {
                UnityEngine.Object.DestroyImmediate(m_ActiveEditor);
                this.m_ActiveEditor = null;
            }
            this.m_ActiveEditor = (Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]) as Editor);
            InitObjPreview();
        }

        private void InitObjPreview()
        {
            if (_objPreview != null)
            {
                UnityEngine.Object.DestroyImmediate(_objPreview);
                this._objPreview = null;
            }
            _objPreview = Editor.CreateEditor(AssetDatabase.LoadAssetAtPath<GameObject>((target as ModelImporter).assetPath), System.Type.GetType("UnityEditor.ObjectPreview"));
        }

        private void OnDestroy()
        {
            Editor myActiveEditor = this.activeEditor;
            if (myActiveEditor != null)
            {
                UnityEngine.Object.DestroyImmediate(myActiveEditor);
                this.m_ActiveEditor = null;
            }
            if (_objPreview != null)
            {
                UnityEngine.Object.DestroyImmediate(_objPreview);
                this._objPreview = null;
            }
        }

        private void BaseInspector()
        {
            // Base inspector
            EditorGUI.BeginDisabledGroup(false);
            GUI.enabled = true;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            this.m_ActiveEditorIndex = GUILayout.Toolbar(this.m_ActiveEditorIndex, this.m_SubEditorNames, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(base.GetType().Name + "ActiveEditorIndex", this.m_ActiveEditorIndex);
                Editor curActiveEditor = this.activeEditor;
                this.m_ActiveEditor = null;
                UnityEngine.Object.DestroyImmediate(curActiveEditor);
                this.m_ActiveEditor = (Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]) as Editor);
//                this.m_ActiveEditor.assetEditor = this.assetEditor;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            this.activeEditor.OnInspectorGUI();        
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            BaseInspector();
            EditorGUILayout.EndVertical();

            // If not under the Model tab, don't show anything
            if (this.m_ActiveEditorIndex != 0)
                return;

            // Look up info
            List<SavedMtlProcessorSettings.SerializedMtlSettings> vals = new List<SavedMtlProcessorSettings.SerializedMtlSettings>();
            SavedMtlProcessorSettings.SerializedMtlSettings mixedVal = null, changedVal = null;
            foreach (Object objTarget in targets)
            {
                ModelImporter importer = objTarget as ModelImporter;
                if (!importer.assetPath.EndsWith(".obj", true, System.Globalization.CultureInfo.InvariantCulture))
                    continue;
            
                SavedMtlProcessorSettings.SerializedMtlSettings curVal = SavedMtlProcessorSettings.Instance[importer];
                if (curVal == null)
                    curVal = SavedMtlProcessorSettings.Instance.SetDefaultSettings(importer);
                if (mixedVal == null)
                    mixedVal = curVal.Clone();
                else
                    mixedVal.CombineProperties(curVal);
                vals.Add(curVal);
            }

            if (vals.Count == 0)
                return;

            GUILayout.BeginHorizontal();
            _foldoutOpened = EditorGUILayout.Foldout(_foldoutOpened, "MTL Processor Options");
            if (GUILayout.Button("Global MTL Options", EditorStyles.toolbarButton, GUILayout.MaxWidth(110f)))
                MtlOptionsWindow.Init();
            GUILayout.EndHorizontal();
            if (!_foldoutOpened)
                return;

            changedVal = mixedVal.Clone();

            bool hadChange = false;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = mixedVal.shouldProcessMaterialsMixed;
            changedVal.shouldProcessMaterials = EditorGUILayout.Toggle(new GUIContent("Process .MTL's", "When checked, will convert imported OBJ material file to Unity Standard Shader."), mixedVal.shouldProcessMaterials);
            if (EditorGUI.EndChangeCheck())
            {
                hadChange = true;
                Undo.RecordObject(SavedMtlProcessorSettings.Instance, "Updated \"Process .MTL's\" import settings");
                foreach (SavedMtlProcessorSettings.SerializedMtlSettings val in vals)
                    val.shouldProcessMaterials = changedVal.shouldProcessMaterials;
            }

            EditorGUI.showMixedValue = mixedVal.shouldOverrideMaterialMixed;
            EditorGUI.BeginChangeCheck();
            changedVal.shouldOverrideMaterial = EditorGUILayout.Toggle(new GUIContent("Override materials", "WARNING: When checked, will overwrite existing changes to materials that have already been converted."), mixedVal.shouldOverrideMaterial);
            if (EditorGUI.EndChangeCheck())
            {
                hadChange = true;
                Undo.RecordObject(SavedMtlProcessorSettings.Instance, "Updated \"Override materials\" import settings");
                foreach (SavedMtlProcessorSettings.SerializedMtlSettings val in vals)
                    val.shouldOverrideMaterial = changedVal.shouldOverrideMaterial;
            }

            if (hadChange)
                EditorUtility.SetDirty(SavedMtlProcessorSettings.Instance);

            EditorGUI.showMixedValue = false;
            // Force repaint if we get an undo/redo message
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
                Repaint();

            if (GUILayout.Button("Reimport materials(will not reimport mesh)", EditorStyles.toolbarButton))
            {
                foreach (var val in vals)
                    MtlAssetProcessor.ProcessMaterials(val.assetRef);
            }

            if (GUILayout.Button("Reset to Default MTL Processor Options", EditorStyles.toolbarButton))
            {
                Undo.RecordObject(SavedMtlProcessorSettings.Instance, "Reset import settings to default");
                SavedMtlProcessorSettings.Instance.ResetToDefault(vals);
            }
        }
#endregion

#region Other pass-through overrides
        // Bottom 3D model
        public override void DrawPreview(Rect previewArea)
        {
            objPreview.DrawPreview(previewArea);
        }

        public override string GetInfoString()
        {
            return objPreview.GetInfoString();
        }

        public override GUIContent GetPreviewTitle()
        {
            return objPreview.GetPreviewTitle();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return objPreview.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        // Icon at the top
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            objPreview.OnPreviewGUI(r, background);
        }
#endregion
    }
}