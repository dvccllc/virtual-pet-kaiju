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
    public class MtlAssetProcessor : AssetPostprocessor
    {
#region Shader prefixes
        const string SHADERPREFIX_TEXTURE_MAP = "_MainTex";
        const string SHADERPREFIX_SPECULAR_MAP = "_SpecGlossMap";
        const string SHADERPREFIX_HEIGHT_MAP = "_ParallaxMap";
        const string SHADERPREFIX_NORMAL_MAP = "_BumpMap";
        const string SHADERPREFIX_DIFFUSE_COLOR = "_Color";
        const string SHADERPREFIX_SPECULAR_COLOR = "_SpecColor";
        const string SHADERPREFIX_GLOSSINESS = "_Glossiness";
#endregion

#region Handle waiting for Texture reimport
        public static List<UpdateNormalMap> toUpdate = new List<UpdateNormalMap>();

        public class UpdateNormalMap
        {
            public Material mat;
            public string texPath;
            public string shaderKey;

            public bool Update()
            {
                Texture asset = AssetDatabase.LoadAssetAtPath<Texture>(texPath);
                if (asset == null)
                {
                    AssetDatabase.ImportAsset(texPath);
                    asset = AssetDatabase.LoadAssetAtPath<Texture>(texPath);
                }
                if (asset == null)
                    return false;
                if (mat.GetTexture(shaderKey) == asset)
                    return true;
                Undo.RecordObject(mat, "Setting normal texture");
                mat.SetTexture(shaderKey, asset);
                mat.EnableKeyword("_NORMALMAP");
//                mat.EnableKeyword("_PARALLAXMAP"); // Needed for height map
                AssetDatabase.SaveAssets();
                return true;
            }
        }

        [InitializeOnLoadMethod]
        private static void RegisterUpdates()
        {
            EditorApplication.update += UpdatePendingNormalMaps;
        }

        public static void UpdatePendingNormalMaps()
        {
            if (toUpdate.Count == 0)
                return;
            for (int i = 0; i < toUpdate.Count; ++i)
            {
                if (toUpdate[i].Update())
                {
                    toUpdate.RemoveAt(i);
                    i--;
                }
            }
        }
#endregion

        public void OnPostprocessModel(GameObject obj)
        {
            ProcessMaterials(obj, assetPath, assetImporter as ModelImporter);
        }

        public static void ProcessMaterials(ModelImporter objImporter)
        { 
            if (objImporter == null)
                return;
            GameObject obj = AssetDatabase.LoadMainAssetAtPath(objImporter.assetPath) as GameObject;
            if (obj == null)
            {
                FHGTools.MtlUtils.Warn("Couldn't load asset for {0}.", objImporter.assetPath);
                return;
            }
            MeshRenderer[] mshRnds = obj.GetModelComponents<MeshRenderer>();
            bool foundNullMaterial = false;
            foreach (MeshRenderer rnd in mshRnds)
            {
                foundNullMaterial = foundNullMaterial || (rnd.sharedMaterial == null);
                foreach(Material m in rnd.sharedMaterials)
                    foundNullMaterial = foundNullMaterial || (m == null);
            }
            if (foundNullMaterial)
            {
                bool shouldReimport = EditorUtility.DisplayDialog(
                    "Deleted Material Files",
                    string.Format("Some materials for {0} have been deleted. Reimport the obj file to ensure all materials can be properly linked. Reimport mesh now?", obj.name),
                    "OK",
                    "Cancel");
                if (shouldReimport)
                {
                    AssetDatabase.ImportAsset(objImporter.assetPath);
                    return;
                }
            }
            ProcessMaterials(obj, objImporter.assetPath, objImporter);
        }

        public static string GetNamedFormat(bool isSharedMaterial)
        {
            return isSharedMaterial ? "Shared-{0}-MTL" : "{1}-{0}-MTL";
        }

        public static void ProcessMaterials(GameObject obj, string assetPath, ModelImporter modelImporter)
        { 
            Object mtlAsset = AssetDatabase.LoadMainAssetAtPath(assetPath.Substring(0, assetPath.Length - 3) + "mtl");
            if (mtlAsset == null || !assetPath.EndsWith(".obj", System.StringComparison.InvariantCultureIgnoreCase))
                return;

            // Load file settings
            SavedMtlProcessorSettings.SerializedMtlSettings mtlImportSettings = SavedMtlProcessorSettings.Instance[modelImporter as ModelImporter];
            if (mtlImportSettings == null)
                mtlImportSettings = SavedMtlProcessorSettings.Instance.SetDefaultSettings(modelImporter as ModelImporter);
            if (!mtlImportSettings.shouldProcessMaterials)
                return;
            
            Undo.RegisterCompleteObjectUndo(obj, "MTL Processor");
            FHGTools.MtlUtils.Log("Beginning processing MTL file for {0}", assetPath);
            bool isSharedMaterial = modelImporter.materialName != ModelImporterMaterialName.BasedOnModelNameAndMaterialName;
            HashSet<Material> mats = new HashSet<Material>();
            MeshRenderer[] mshRnds = obj.GetModelComponents<MeshRenderer>();
            foreach (MeshRenderer rnd in mshRnds)
            {
                foreach(Material m in rnd.sharedMaterials)
                {
                    if (m != null)
                        mats.Add(m);
                }
                if (rnd.sharedMaterial != null)
                    mats.Add(rnd.sharedMaterial);
            }

            Dictionary<Material, Material> toReplace = new Dictionary<Material, Material>();
            FHGTools.MtlUtils.Log("Relinking materials for {0}", assetPath);
            string matDirectory = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/Materials/";
            string nameFormat = GetNamedFormat(isSharedMaterial);
            foreach (Material mat in mats)
            {
                // Rename if necessary
                // TODO: Check if it's inside the material directory
                if (mat != null)
                {
                    // TODO: Check if the name already is of the correct format!
                    bool isProcessed = mat.name.EndsWith("-MTL", System.StringComparison.InvariantCultureIgnoreCase);
                    bool insideMaterialsFolder = AssetDatabase.GetAssetPath(mat).StartsWith(matDirectory, System.StringComparison.InvariantCultureIgnoreCase);
                    if (!isProcessed && insideMaterialsFolder)
                    {
                        string newName = string.Format(nameFormat, mat.name, obj.name);
                        if (newName.StartsWith(obj.name + "-" + obj.name + "-") && mat.name.StartsWith(obj.name + "-"))
                            newName = newName.Substring(obj.name.Length + 1);
                        FHGTools.MtlUtils.Log("Clearing Unity-generated materials from " + mat.name + " and replacing with " + newName);
                        // Rename asset, and switch to old asset if it already exists.
                        Material prevAsset = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GetAssetPath(mat).Replace(mat.name, newName));
                        if (prevAsset != null)
                            toReplace.Add(mat, prevAsset);
                        else
                            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(mat), newName);
                    }
                    else if (!insideMaterialsFolder)
                        FHGTools.MtlUtils.Log("Not remapping {0} since it's outside local materials folder. To reset all materials, set Materials search to Local Materials Folder. Full path is {1}",
                            mat.name, AssetDatabase.GetAssetPath(mat));
                }
            }

            List<Material> toIgnore = new List<Material>();
            foreach (MeshRenderer rnd in mshRnds)
            {
                for(int i = 0; i < rnd.sharedMaterials.Length; ++i)
                {
                    if (rnd.sharedMaterials[i] != null && toReplace.ContainsKey(rnd.sharedMaterials[i]))
                        rnd.sharedMaterials[i] = toReplace[rnd.sharedMaterials[i]];
                }
                if (rnd.sharedMaterial != null && toReplace.ContainsKey(rnd.sharedMaterial))
                    rnd.sharedMaterial = toReplace[rnd.sharedMaterial];
            }
            foreach (Material mat in toReplace.Keys)
            {
                if (!mtlImportSettings.shouldOverrideMaterial)
                {
                    FHGTools.MtlUtils.Log("Leaving material {0} unchanged to avoid overwriting custom changes. To overwrite this material with MTL settings, select the overwrite materials option", AssetDatabase.GetAssetPath(mat));
                    toIgnore.Add(toReplace[mat]);
                }
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mat));
            }
            ProcessMtlFile(Regex.Replace(assetPath, "\\.obj", ".mtl", RegexOptions.IgnoreCase), obj.name, isSharedMaterial, mtlImportSettings, toIgnore);
            FHGTools.MtlUtils.Log("Completed processing MTL file for {0}", assetPath);
            EditorUtility.SetDirty(obj);
        }

        public static void ProcessMtlFile(string fileLocation, string objName, bool isSharedMaterial, SavedMtlProcessorSettings.SerializedMtlSettings importSettings, List<Material> toIgnore)
        {
            string nameFormat = GetNamedFormat(isSharedMaterial);
            string matDirectory = fileLocation.Substring(0, fileLocation.LastIndexOf('/')) + "/Materials/";
            string fullPath = Path.Combine(Application.dataPath, fileLocation.Substring(7));
            System.IO.StreamReader reader = new System.IO.StreamReader(fullPath);
            string contents = reader.ReadToEnd();
            Match m = Regex.Match(contents, "newmtl\\s+(?<matname>\\S+).*?((?=\\s*newmtl)|\\s*\\z)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            while (m.Success)
            {
                HandleMaterial(matDirectory + string.Format(nameFormat, m.Groups["matname"].Value, objName) + ".mat", m.Value, importSettings, toIgnore);
                m = m.NextMatch();
            }
        }

        public static void HandleMaterial(string materialLocation, string mtlFileContents, SavedMtlProcessorSettings.SerializedMtlSettings importSettings, List<Material> toIgnore)
        {
            string texDirectory = materialLocation.Remove(materialLocation.LastIndexOf("Materials/"));
            string fullPath = Path.Combine(Application.dataPath, materialLocation.Substring(7));
            Material m = AssetDatabase.LoadAssetAtPath<Material>(materialLocation);
            if (m == null)
            {
                FHGTools.MtlUtils.Warn("Couldn't load material at {0}", materialLocation);
                return;
            }
            if (toIgnore.Contains(m))
                return;
            FHGTools.MtlUtils.Log("Reading MTL settings into material {0}", materialLocation);
            Undo.RegisterCompleteObjectUndo(m, "Read MTL settings for material");

            string testString = null;
            float testVal = 0.0f;
            int testInt = 0;
            // First replace the shader to the specular one, if necessary
            testString = LookUpMtlPrefix("illum", mtlFileContents);
            bool isSpecularSetup = (testString != null && int.TryParse(testString, out testInt) && testInt == 2);
            m.shader = Shader.Find(isSpecularSetup ? "Standard (Specular setup)" : "Standard");
            AssetDatabase.SaveAssets();

            // Set all the texture maps and colors as needed
            bool useHeightMap = false;
            m.SetTexture(SHADERPREFIX_HEIGHT_MAP, null);
            m.SetTexture(SHADERPREFIX_NORMAL_MAP, null);
            string bumpMapKey = useHeightMap ? SHADERPREFIX_HEIGHT_MAP : SHADERPREFIX_NORMAL_MAP;
            TrySetTexMap("map_Kd", SHADERPREFIX_TEXTURE_MAP, m, texDirectory, mtlFileContents);

            // Check for opacity map
            testString = LookUpMtlPrefix("map_opacity", mtlFileContents);
            if (testString == null)
                testString = LookUpMtlPrefix("map_d", mtlFileContents);
            bool hasCutout = false;
            if (testString != null)
            {
                // Have to have a main texture to have an opacity map!
                Texture mainTex = m.GetTexture(SHADERPREFIX_TEXTURE_MAP);
                Texture opacityMap = AssetDatabase.LoadAssetAtPath<Texture>(texDirectory + testString);
                if (mainTex != null && opacityMap != null)
                {
                    MtlAlphizer.TryCombineTextures(false,
                        AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mainTex)) as TextureImporter,
                        AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(opacityMap)) as TextureImporter,
                        (Texture2D combinedTex) =>
                        {
                            Material myMat = AssetDatabase.LoadAssetAtPath<Material>(materialLocation);
                            if (myMat != null && combinedTex != null)
                                myMat.SetTexture(SHADERPREFIX_TEXTURE_MAP, combinedTex);
                            AssetDatabase.SaveAssets();
                        });
                    hasCutout = true;
                }
            }
        
            TrySetTexMap("map_Ks", SHADERPREFIX_SPECULAR_MAP, m, texDirectory, mtlFileContents);
            TrySetTexMap("map_bump", bumpMapKey, m, texDirectory, mtlFileContents);
            TrySetTexMap("bump", bumpMapKey, m, texDirectory, mtlFileContents, true);
            TrySetColor("Kd", "d", SHADERPREFIX_DIFFUSE_COLOR, m, mtlFileContents);
            TrySetColor("Ks", null, SHADERPREFIX_SPECULAR_COLOR, m, mtlFileContents);
            testString = LookUpMtlPrefix("Ns", mtlFileContents);
            if (testString != null && float.TryParse(testString, out testVal))
                m.SetFloat(SHADERPREFIX_GLOSSINESS, Mathf.Clamp01(testVal * 0.005f));
            AssetDatabase.SaveAssets();

            // Set the mode to transparency if necessary
            testString = LookUpMtlPrefix("d", mtlFileContents);
            if (testString != null && float.TryParse(testString, out testVal) && testVal < 1f)
                SetShaderTagToTransparent(m, fullPath);
            else if (hasCutout)
                SetShaderTagToCutout(m, fullPath);
            else
                SetShaderTagToOpaque(m, fullPath);
            EditorUtility.SetDirty(m);
            AssetDatabase.ImportAsset(materialLocation, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }


        private static void TrySetTexMap(string mtlKey, string shaderKey, Material mat, string texDirectory, string mtlContents, bool skipReset = false)
        {
            if (!skipReset)
                mat.SetTexture(shaderKey, null);
            string texName = LookUpMtlPrefix(mtlKey, mtlContents);
            Texture asset = null;
            if (!string.IsNullOrEmpty(texName))
            {
                asset = AssetDatabase.LoadAssetAtPath<Texture>(texDirectory + texName);
                if (asset == null)
                    MtlUtils.Warn("Couldn't find texture asset at {0}", texDirectory + texName);
            }
            if (asset != null)
            {
                if (shaderKey == SHADERPREFIX_NORMAL_MAP)
                {
                    bool createdAsset = false;
                    string modifiedPath = (texDirectory + texName);
                    // Check if we are using this texture for any of the other maps
                    if (mat.GetTexture(SHADERPREFIX_TEXTURE_MAP) == asset || mat.GetTexture(SHADERPREFIX_SPECULAR_MAP) == asset)
                    {
                        // Create a separate texture copy, so that we don't use the same map for each version.
                        modifiedPath = modifiedPath.Insert(modifiedPath.LastIndexOf('.'), "_NORMAL");
                        asset = AssetDatabase.LoadAssetAtPath<Texture>(modifiedPath);
                        if (asset == null)
                        {
                            createdAsset = true;
                            AssetDatabase.CopyAsset(texDirectory + texName, modifiedPath);
                            TextureImporter texImporter = (TextureImporter.GetAtPath(modifiedPath) as TextureImporter);
                            texImporter.convertToNormalmap = true;
                            texImporter.normalmap = true;
                            texImporter.textureType = TextureImporterType.NormalMap;
                            texImporter.heightmapScale = 0.025f;
                            texImporter.normalmapFilter = TextureImporterNormalFilter.Sobel;
                            texImporter.SaveAndReimport();
                            asset = AssetDatabase.LoadAssetAtPath<Texture>(modifiedPath);
                            if (asset == null)
                            {
                                UpdateNormalMap updateInfo = new UpdateNormalMap();
                                updateInfo.mat = mat;
                                updateInfo.texPath = modifiedPath;
                                updateInfo.shaderKey = shaderKey;
                                toUpdate.Add(updateInfo);
                            }
                        }
                    }
                    if (!createdAsset)
                    {
                        TextureImporter texImporter = (TextureImporter.GetAtPath(modifiedPath) as TextureImporter);
                        if (texImporter.textureType != TextureImporterType.NormalMap || !texImporter.convertToNormalmap || !texImporter.normalmap)
                        {
                            texImporter.convertToNormalmap = true;
                            texImporter.normalmap = true;
                            texImporter.textureType = TextureImporterType.NormalMap;
                            texImporter.SaveAndReimport();
                        }
                    }
                }
                mat.SetTexture(shaderKey, asset);
            }

            if (shaderKey == SHADERPREFIX_NORMAL_MAP)
            {
                if (asset == null)
                {
                    mat.DisableKeyword("_NORMALMAP");
                    mat.DisableKeyword("_PARALLAXMAP");
                } else
                    mat.EnableKeyword("_NORMALMAP");
            } else if (shaderKey == SHADERPREFIX_SPECULAR_MAP)
            {
                if (asset == null)
                    mat.DisableKeyword("_SPECGLOSSMAP");
                else
                    mat.EnableKeyword("_SPECGLOSSMAP");
            }
        }

        private static void TrySetColor(string mtlKey, string mtlAlphaKey, string shaderKey, Material mat, string mtlContents)
        {
            mat.SetColor(shaderKey, Color.clear);
            Match m = Regex.Match(mtlContents, string.Format("{0}\\s+(?<r>\\d*\\.?\\d+)\\s+(?<g>\\d*\\.?\\d+)\\s+(?<b>\\d*\\.?\\d+)\\b", mtlKey), RegexOptions.IgnoreCase);
            if (!m.Success)
                return;
        
            Color newColor = new Color(float.Parse(m.Groups["r"].Value), float.Parse(m.Groups["g"].Value), float.Parse(m.Groups["b"].Value), 1f);
            if (mtlAlphaKey != null)
            {
                string alphaString = LookUpMtlPrefix("mtlAlphaKey", mtlContents);
                if (alphaString != null)
                    float.TryParse(alphaString, out newColor.a);
            }
            mat.SetColor(shaderKey, newColor);
        }

        private static string LookUpMtlPrefix(string mtlKey, string mtlContents)
        {
            Match m = Regex.Match(mtlContents, string.Format("\\b{0}\\s+(?<val>\\S+)\\b", mtlKey), RegexOptions.IgnoreCase);
            return m.Success ? m.Groups["val"].Value : null;
        }


        private static void ToggleShaderKeyword(Material m, string keyword, string testKeyword)
        {
            if (testKeyword != keyword)
                m.DisableKeyword(keyword);
            else
                m.EnableKeyword(keyword);
        }

        private static void SetAllKeywords(Material m, string activeKeyword)
        {
            ToggleShaderKeyword(m, "_ALPHAPREMULTIPLY_ON", activeKeyword);
            ToggleShaderKeyword(m, "_ALPHATEST_ON", activeKeyword);
            ToggleShaderKeyword(m, "_ALPHABLEND_ON", activeKeyword);
        }

        private static void SetShaderTag(Material m, string fullMaterialPath, string renderType, int renderQueue, int srcBlend, int dstBlend, int zWrite, int mode)
        {
            m.renderQueue = renderQueue;
            m.SetInt("_Mode", mode);
            m.SetInt("_SrcBlend", srcBlend);
            m.SetInt("_DstBlend", dstBlend);
            m.SetInt("_ZWrite", zWrite);
            m.SetOverrideTag("RenderType", renderType);
            AssetDatabase.SaveAssets();
        }

        private static void SetShaderTagToTransparent(Material m, string fullMaterialPath)
        {
            SetAllKeywords(m, "_ALPHAPREMULTIPLY_ON");
            SetShaderTag(m, fullMaterialPath, "Transparent", 3000, 1, 20, 0, 3);
        }

        private static void SetShaderTagToCutout(Material m, string fullMaterialPath)
        {
            SetAllKeywords(m, "_ALPHATEST_ON");
            SetShaderTag(m, fullMaterialPath, "TransparentCutout", 2450, 1, 0, 1, 1);
        }

        private static void SetShaderTagToOpaque(Material m, string fullMaterialPath)
        {
            SetAllKeywords(m, "");
            SetShaderTag(m, fullMaterialPath, "Opaque", 2000, 1, 0, 1, 0);
        }
    }
}