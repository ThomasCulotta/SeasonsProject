// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;

namespace SWS
{
    public static class StylizedWaterUtilities
    {
        public static bool DEBUG = false;
    }

    public static class TexturePacker
    {       
        //Normal map
        private static Shader m_NormalRenderShader; 
        private static Shader NormalRenderShader
        {
            get
            {
                if(m_NormalRenderShader == null)
                {
                    m_NormalRenderShader = Shader.Find("Hidden/SWS/NormalMapRenderer");
                    return m_NormalRenderShader;
                }
                else
                {
                    return m_NormalRenderShader;
                }
            }
        }

        private static Material m_NormalRenderMat;
        private static Material NormalRenderMat
        {
            get
            {
                if(m_NormalRenderMat == null)
                {
                    m_NormalRenderMat = new Material(NormalRenderShader);
                    return m_NormalRenderMat;
                }
                else
                {
                    return m_NormalRenderMat;
                }
            }
        }

        private const float NORMALSTRENGTH = 6f;
        private const float NORMALOFFSET = 0.005f;

        //Shader map
        private static Shader m_ShaderMapShader;
        private static Shader ShaderMapShader
        {
            get
            {
                if (m_ShaderMapShader == null)
                {
                    m_ShaderMapShader = Shader.Find("Hidden/SWS/ShaderMapRenderer");
                    return m_ShaderMapShader;
                }
                else
                {
                    return m_ShaderMapShader;
                }
            }
        }

        private static Material m_ShaderMapRenderMat;
        private static Material ShaderMapRenderMat
        {
            get
            {
                if (m_ShaderMapRenderMat == null)
                {
                    m_ShaderMapRenderMat = new Material(ShaderMapShader);
                    return m_ShaderMapRenderMat;
                }
                else
                {
                    return m_ShaderMapRenderMat;
                }
            }
        }

        //Fixed for now, max texture size can be overridden in the inspector by user
        private static int resolution = 1024; 

        public static bool useCompression = false;

        public static Texture2D CreateShadermap(Material targetMaterial, int intersectionStyle, int waveStyle, int heightmapStyle, bool useCompression = false, Texture2D customIntersectionTex = null)
        {
            StylizedWaterResources r = StylizedWaterResources.Instance;

            //Set compression setting
            TexturePacker.useCompression = useCompression;

            Texture2D intersectionTex;
            if (customIntersectionTex)
            {
                intersectionTex = customIntersectionTex;
            }
            else 
            {
                intersectionTex = r.intersectionStyles[intersectionStyle];
            }

            Texture2D surfaceHighlightTex = r.waveStyles[waveStyle];
            Texture2D heightmapTex = r.heightmapStyles[heightmapStyle];

            Texture2D shadermap = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
            {
                name = "_shadermap", //Prefix and suffix to be appended upon saving
                anisoLevel = 2,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            RenderTexture rt = new RenderTexture(resolution, resolution, 0);
            RenderTexture.active = rt;

            //Constants
            ShaderMapRenderMat.SetTexture("_RedInput", surfaceHighlightTex);
            ShaderMapRenderMat.SetTexture("_GreenInput", heightmapTex);
            ShaderMapRenderMat.SetTexture("_BlueInput", intersectionTex);
            //ShaderMapRenderMat.SetTexture("_AlphaInput", null);

            //Pack textures on GPU
            Graphics.Blit(null, rt, ShaderMapRenderMat);

            //Copy result into texture
            shadermap.ReadPixels(new Rect(0, 0, shadermap.width, shadermap.height), 0, 0);
            shadermap.Apply();

            //Cleanup
            RenderTexture.active = null;

            shadermap = SaveAndGetTexture(targetMaterial, shadermap);

            return shadermap;
        }

        public static Texture2D RenderNormalMap(Material targetMaterial, int waveStyle, bool useCompression = false, Texture2D customNormalTex = null)
        {
            StylizedWaterResources r = StylizedWaterResources.Instance;

            //If a custom texture is assigned, return that
            if (customNormalTex) return customNormalTex;

            //Set compression setting
            TexturePacker.useCompression = useCompression;

            Texture2D heightmap = r.waveStyles[waveStyle];

            Texture2D normalTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, true)
            {
                name = "_normal",
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            RenderTexture rt = new RenderTexture(resolution, resolution, 0);
            RenderTexture.active = rt;

            //Constants
            NormalRenderMat.SetFloat("_Offset", NORMALOFFSET);
            NormalRenderMat.SetFloat("_Strength", NORMALSTRENGTH);

            //Convert heightmap to normal on GPU
            Graphics.Blit(heightmap, rt, NormalRenderMat);

            //Copy result into texture
            normalTexture.ReadPixels(new Rect(0, 0, normalTexture.width, normalTexture.height), 0, 0);
            normalTexture.Apply();

            //Cleanup
            RenderTexture.active = null;

            normalTexture = SaveAndGetTexture(targetMaterial, normalTexture);

            return normalTexture;
        }

        // Save the sourceTexture in a "Textures" folder next to the targetMaterial
        // And return the file reference
        private static Texture2D SaveAndGetTexture(Material targetMaterial, Texture2D sourceTexture)
        {
            //Material root folder
            string targetFolder = AssetDatabase.GetAssetPath(targetMaterial);
            targetFolder = targetFolder.Replace(targetMaterial.name + ".mat", string.Empty);

            //Append textures folder
            targetFolder += "Textures/";

            //Create Textures folder if it doesn't exist
            if (!Directory.Exists(targetFolder))
            {
                Debug.Log("[Stylized Water] Directory: " + targetFolder + " doesn't exist, creating...");
                Directory.CreateDirectory(targetFolder);

                AssetDatabase.Refresh();
            }

            //Compose file path
            string path = targetFolder + targetMaterial.name + sourceTexture.name + "_baked.png";

            File.WriteAllBytes(path, sourceTexture.EncodeToPNG());

            if(StylizedWaterUtilities.DEBUG) Debug.Log("Written file to: " + path);

            AssetDatabase.Refresh();

            //Trigger SWSImporter
            AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);

            //Grab the file
            sourceTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

            //Return file reference
            return sourceTexture;
        }
    }

    //Catch the normal map when it is being imported and flag it accordingly
    internal sealed class SWSImporter : AssetPostprocessor
    {

        TextureImporter textureImporter;

        private void OnPreprocessTexture()
        {
            textureImporter = assetImporter as TextureImporter;

            //Only run for SWS created textures, which have the _baked suffix
            if (!assetPath.Contains("_baked")) return;

            if (TexturePacker.useCompression)
            {
                textureImporter.textureType = TextureImporterType.Default;
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
#else
                textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGB2;
#endif
            }
            else
            {
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
#else
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
            }

            //Look for the given name, this will also apply to textures outside of the SWS package, but it's unlikely this naming convention will occur
            if (assetPath.Contains("_normal_baked"))
            {
#if !UNITY_5_5_OR_NEWER
                textureImporter.normalmap = true;
#endif
                textureImporter.textureType = TextureImporterType.NormalMap;
                textureImporter.wrapMode = TextureWrapMode.Repeat;

            }
            else if (assetPath.Contains("_shadermap_baked"))
            {
#if !UNITY_5_5_OR_NEWER
                textureImporter.normalmap = false;
#endif
                textureImporter.alphaIsTransparency = false;
                textureImporter.wrapMode = TextureWrapMode.Repeat;
                textureImporter.textureType = TextureImporterType.Default;
            }

        }
    }

}//Namespace
#endif