//--------------------------------------
//           .MTL Processor           
//  Copyright © 2016 Fire Hose Games  
//--------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FHGTools
{
    public static class MtlAlphizer
    {
        public static List<PendingOp> pending = new List<PendingOp>();

        [System.Serializable]
        public class PendingOp
        {
            public enum Phase
            {
                ReadyImporterForCopy,
                AwaitingFinalImport
            };

            public Phase myPhase = Phase.ReadyImporterForCopy;
            public TextureImporter srcTex = null;
            public TextureImporter srcTexOpacity = null;
            public TextureImporter destTex = null;
            public bool wasReadable = false;
            public bool whiteIsOpaque = false;
            public string destFile = null;
            public System.Action<Texture2D> callback = null;

            public bool CheckUpdate()
            {
                switch (myPhase)
                {
                    case Phase.ReadyImporterForCopy:
                        return FinishCombiningTextures(this);
                    case Phase.AwaitingFinalImport:
                        Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(destFile);
                        MtlUtils.Log("Created {0} Alph-ized version of {1} and {2}", loadedTexture.name, srcTex.name, srcTexOpacity.name);
                        if (callback != null && loadedTexture != null)
                            callback(loadedTexture);
                        return loadedTexture != null;
                }
                Debug.LogError("Invalid phase for TextureCombiner! " + myPhase);
                return true;
            }
        }

        public static string ConvertFilePath(string assetPath)
        {
            return Regex.Replace(assetPath, "\\.[^\\s]+\\b", SavedMtlProcessorSettings.Instance.AlphizerFileSuffix+".png");;
        }

        public static void TryCombineTextures(bool whiteIsOpaque, TextureImporter srcImporter, TextureImporter srcOpacityImporter, System.Action<Texture2D> onCompleteCallback = null, string destFile = null)
        {
            if (destFile == null)
                destFile = ConvertFilePath(srcImporter.assetPath);
            if (!destFile.EndsWith(".png"))
            {
                Debug.LogError("Couldn't replace extension for " + destFile);
                return;
            }
            TextureImporter destImporter = AssetImporter.GetAtPath(destFile) as TextureImporter;
            if (destImporter != null)
            {
                FHGTools.MtlUtils.Log("No need to Alph-ize. Already have image file at: " + destFile);
                if (onCompleteCallback != null)
                    onCompleteCallback(AssetDatabase.LoadAssetAtPath<Texture2D>(destImporter.assetPath));
                return;
            }

            if (destImporter == null)
            {
                string fullPath = Path.Combine(Application.dataPath, destFile.Substring(7));
                System.IO.FileStream writer = new System.IO.FileStream(fullPath, FileMode.Create, FileAccess.Write);
                writer.Write(new byte[]{ }, 0, 0);
                writer.Close();
                AssetDatabase.ImportAsset(destFile);
                destImporter = AssetImporter.GetAtPath(destFile) as TextureImporter;
            }
            destImporter = AssetImporter.GetAtPath(destFile) as TextureImporter;
            PendingOp op = new PendingOp();
            op.destFile = destFile;
            op.destTex = destImporter;
            op.srcTex = srcImporter;
            op.srcTexOpacity = srcOpacityImporter;
            op.wasReadable = srcImporter.isReadable;
            op.callback = onCompleteCallback;
            op.whiteIsOpaque = whiteIsOpaque;
            TextureImporterSettings importSettings = new TextureImporterSettings();
            srcImporter.ReadTextureSettings(importSettings);
            if (destImporter != null)
            {
                destImporter.SetTextureSettings(importSettings);
                destImporter.isReadable = true;
                destImporter.alphaIsTransparency = true;
                destImporter.SaveAndReimport();
            }
            if (!srcOpacityImporter.isReadable)
            {
                srcOpacityImporter.isReadable = true;
                srcOpacityImporter.SaveAndReimport();
            }
            if (!srcImporter.isReadable)
            {
                srcImporter.isReadable = true;
                srcImporter.SaveAndReimport();
            }
            pending.Add(op);
        }

        public static bool FinishCombiningTextures(PendingOp op)
        {
            if (!op.srcTexOpacity.isReadable || !op.srcTex.isReadable)
                return false;
            if (op.destTex == null)
                op.destTex = TextureImporter.GetAtPath(op.destFile) as TextureImporter;
            if (op.destTex == null)
                return false;
        
            Texture2D srcTexOpacity = AssetDatabase.LoadAssetAtPath<Texture2D>(op.srcTexOpacity.assetPath);
            Texture2D srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(op.srcTex.assetPath);
            if (srcTex == null || srcTexOpacity == null)
                return false;
            Color32[] opacity = srcTexOpacity.GetPixels32();
            Color32[] myColors = srcTex.GetPixels32();
            if (opacity.Length != myColors.Length)
            {
                FHGTools.MtlUtils.Warn("Image size mismatch between opacity and texture map!: opacity: ({2}x{3}) {0} bytes, texture:({4}x{5}) {1} bytes",
                    opacity.Length, myColors.Length, srcTexOpacity.width, srcTexOpacity.height, srcTex.width, srcTex.height);
                return true;
            }

            Texture2D newTexture = new Texture2D(srcTex.width, srcTex.height, TextureFormat.RGBA32, false);
            // Write out color data
            if (op.whiteIsOpaque)
            {
                for (int i = 0; i < opacity.Length; ++i)
                    myColors[i].a = (byte)((opacity[i].r + opacity[i].g + opacity[i].b) / 3);
            }
            else
            {
                for (int i = 0; i < opacity.Length; ++i)
                    myColors[i].a = (byte)~((opacity[i].r + opacity[i].g + opacity[i].b) / 3);
            }

            newTexture.SetPixels32(myColors);
            newTexture.Apply(); 
            string fullPath = Path.Combine(Application.dataPath, op.destFile.Substring(7));
            System.IO.FileStream writer = new System.IO.FileStream(fullPath, FileMode.Create, FileAccess.Write);
            byte[] toWrite = newTexture.EncodeToPNG();
            writer.Write(newTexture.EncodeToPNG(), 0, toWrite.Length);
            writer.Close();

            op.srcTexOpacity.isReadable = op.wasReadable;
            op.srcTexOpacity.SaveAndReimport();
            op.srcTex.isReadable = op.wasReadable;
            op.srcTex.SaveAndReimport();

            TextureImporterSettings importSettings = new TextureImporterSettings();
            op.srcTex.ReadTextureSettings(importSettings);
            op.destTex.SetTextureSettings(importSettings);
            op.destTex.alphaIsTransparency = true;
            op.destTex.isReadable = op.wasReadable;
            op.destTex.SaveAndReimport();
            op.myPhase = PendingOp.Phase.AwaitingFinalImport;

            return false;
        }

        public static void Update()
        {
            for (int i = 0; i < pending.Count; ++i)
            {
                if (pending[i].CheckUpdate())
                {
                    pending.RemoveAt(i);
                    i--;
                }
            }
        }


        [InitializeOnLoadMethod]
        public static void OnInit()
        {
            EditorApplication.update += Update;
        }
    }

    public class MtlAlphizerWindow : EditorWindow
    {
#region Fields
        public Texture srcTexture = null;
        public Texture opacityTexture = null;
        public string destFileLocation = null;
        public bool isInverted = false;
        public const float NON_DESCRIPTION_HEIGHT = 192f;
        public const float MIN_DESCRIPTION_HEIGHT = 16f;
        public const float WINDOW_WIDTH = 250f;

        static MtlAlphizerWindow windowInstance = null;
#endregion

        [MenuItem("Window/MTL Processor/MTL Alph-izer")]
        public static void Init()
        {
            if (windowInstance == null)
            {
                windowInstance = EditorWindow.CreateInstance<MtlAlphizerWindow>();
                windowInstance.minSize = new Vector2(WINDOW_WIDTH, NON_DESCRIPTION_HEIGHT + MIN_DESCRIPTION_HEIGHT);
            }
            windowInstance.titleContent = new GUIContent("MTL Alph-izer");
            windowInstance.Show();
        }

        private void OnGUI()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;
            GUILayoutOption[] options = new GUILayoutOption[]{GUILayout.MaxHeight(Mathf.Min(100f, position.height - NON_DESCRIPTION_HEIGHT))};
            GUILayout.Label("Got an old opacity map? Need a .png with an alpha channel? The Alph-izer can take any two textures and combine them into a 32bit .png. Select a color source, and an alpha source (b&w opacity map) and the Alph-izer will do the rest!", labelStyle, options);
            srcTexture = EditorGUILayout.ObjectField(new GUIContent("Color Source", "Select a texture that needs a channel for transparency"), srcTexture, typeof(Texture), false) as Texture;
            opacityTexture = EditorGUILayout.ObjectField(new GUIContent("Alpha Source", "Select the texture to use as an alpha cutout"), opacityTexture, typeof(Texture), false) as Texture;
            if (string.IsNullOrEmpty(destFileLocation))
                destFileLocation = MtlAlphizer.ConvertFilePath(AssetDatabase.GetAssetPath(srcTexture));
            isInverted = EditorGUILayout.ToggleLeft(new GUIContent("White=Opaque","Change white to be opaque, black to be transparent"), isInverted);

            destFileLocation = EditorGUILayout.TextField(new GUIContent("New File Destination", "Location of produced file"), destFileLocation);

            // Check if we have sufficient data to start
            bool alreadyHasTexture = false;
            GUI.enabled = srcTexture != null && opacityTexture != null &&
                !string.IsNullOrEmpty(destFileLocation) && destFileLocation.EndsWith(".png", System.StringComparison.InvariantCultureIgnoreCase)
                    && !(alreadyHasTexture = AssetImporter.GetAtPath(destFileLocation) as TextureImporter != null);
            TextureImporter srcImporter = null, opacityImporter = null;
            if (GUI.enabled)
            {
                srcImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(srcTexture)) as TextureImporter;
                opacityImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(opacityTexture)) as TextureImporter;
                GUI.enabled = srcImporter != null && opacityImporter != null;
            }
            if (GUILayout.Button("ALPH-IZE!"))
                MtlAlphizer.TryCombineTextures(isInverted, srcImporter, opacityImporter, null, destFileLocation);
            if (alreadyHasTexture)
                GUILayout.Label("File already exists at " + destFileLocation, labelStyle);
            GUI.enabled = true;
        }
    }
}