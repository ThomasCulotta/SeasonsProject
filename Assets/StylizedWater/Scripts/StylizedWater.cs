// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SWS
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [HelpURL("http://staggart.xyz/unity/stylized-water-shader/documentation/")]
    public class StylizedWater : MonoBehaviour
    {
        //Meta meta
        public static readonly string VERSION = "1.50";
        public static readonly string DOC_URL = "http://staggart.xyz/unity/stylized-water-shader/documentation/";

        #region Shader data
        public string[] shaderNames;
        public int shaderIndex = 0;

        public Shader shader;
        private Shader DesktopShader;
        private Shader DesktopTessellationShader;
        private Shader DesktopBetaShader;
        private Shader MobileAdvancedShader;
        private Shader MobileBasicShader;
        #endregion

        #region BETA
        public bool isUnlit;
        public Color waterShallowColor;
        [Range(0f, 1f)]
        public float waveFoam;
        public Vector4 waveDirection;
        [Range(0f, 1f)]
        public float reflectionStrength = 1f;
        [Range(2f, 10f)]
        public float reflectionFresnel = 10f;
        public bool showReflection;
        #endregion

        #region Shader properties
        //Color
        public Color waterColor;
        public Color fresnelColor;
        public float fresnel;
        public Color rimColor;
        [Range(0f, 1f)]
        public float normalStrength;
        [Range(-0.2f, 0.2f)]
        public float waveTint;

        //Surface
        [Range(0f, 1f)]
        public float transparency;
        [Range(0f, 1f)]
        public float glossiness;
        public bool worldSpaceTiling;
        [Range(0f, 1f)]
        public float tiling;
        public Texture reflectionCubemap;
        [Range(0f, 0.2f)]
        public float refractionAmount;

        //Surface highlight
        public bool useIntersectionHighlight;
        [Range(-1, 1)]
        public float surfaceHighlight;
        public float surfaceHighlightTiling;
        [Range(0f, 1f)]
        public float surfaceHighlightSize;
        public bool surfaceHighlightPanning;

        //Depth
        [Range(0f, 30f)]
        public float depth;
        [Range(0f, 1f)]
        public float depthDarkness;

        //Intersection
        [Range(0f, 10f)]
        public float rimSize;
        [Range(0f, 5f)]
        public float rimFalloff;
        [Range(0f, 1f)]
        public float rimDistance;
        public float rimTiling;

        //Waves
        [Range(0.01f, 10f)]
        public float waveSpeed;
        [Range(0f, 1f)]
        public float waveStrength;

        public Texture2D customIntersection;
        public Texture2D customNormal;

        public Texture2D normals;
        public Texture2D shadermap;

        [Range(0.01f, 10f)]
        public float tessellation;
        #endregion

        #region Texture variables

        //WebGL fix, ProceduralMaterial is not supported
        public string[] intersectionStyleNames;
        public int intersectionStyle = 1;

        public string[] waveStyleNames;
        public int waveStyle;

        public string[] waveHeightmapNames;
        public int waveHeightmapStyle;
        public float waveSize;

        public bool useCustomIntersection;
        public bool useCustomNormals;
        #endregion

        #region Reflection
        Camera reflectionCamera;
        public bool useReflection;

        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        public int reflectionRes;

        public int reflectionTextureSize = 256;

        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectLayers = -1;
        public LayerMask depthLayers = -1;

        private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table

        private RenderTexture m_ReflectionTexture;

        private int m_OldReflectionTextureSize;
        #endregion

        #region Script vars
        private MeshRenderer meshRenderer;
        public Material material;
        public bool isBeta;
        public bool isMobilePlatform;
        public bool isMobileAdvanced = false;
        public bool isMobileBasic = false;
        public string shaderName = null;
        public bool isWaterLayer;
        public bool hasShaderParams = false;
        public bool hasMaterial;
        #endregion

        #region Toggles
        [Header("Toggles")]
        public bool showColors = true;
        public bool showSurface = true;
        public bool showIntersection;
        public bool showHighlights;
        public bool showDepth;
        public bool showWaves;
        public bool showAdvanced;
#if !UNITY_5_5_OR_NEWER
        public bool hideWireframe = false;
#endif
        public bool hideMaterialInspector = true;
        #endregion

        #region Texture baking
        public bool useCompression = false;
        #endregion

        #region Editor functions
#if UNITY_EDITOR
        public void GetShaderMap(bool useCompression, bool useCustomIntersection)
        {
            //Save settings
            this.useCompression = useCompression;
            this.useCustomIntersection = useCustomIntersection;

            //Before baking, make sure all settings are applied
            SetShaderProperties();
            //SetSubstanceProperties(false);

            //Combine chosen textures into packed texture
            shadermap = TexturePacker.CreateShadermap(material, intersectionStyle, waveStyle, waveHeightmapStyle, useCompression, (useCustomIntersection) ? customIntersection : null);

            //Set baked textures to shader
            material.SetTexture("_Shadermap", shadermap);
        }

        public void GetNormalMap(bool useCompression, bool useCustomNormals)
        {
            SetShaderProperties();

            normals = TexturePacker.RenderNormalMap(material, waveStyle, useCompression, (useCustomNormals) ? customNormal : null);
            material.SetTexture("_Normals", normals);
        }

        //Called through inspector OnEnable
        public void Init()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();

#if UNITY_EDITOR

            //Avoids a null ref on start up, instance will be created some frames later(?)
            if (StylizedWaterResources.Instance == null) return;

            //Compose dropdown menus
            intersectionStyleNames = new string[StylizedWaterResources.Instance.intersectionStyles.Length];
            for (int i = 0; i < StylizedWaterResources.Instance.intersectionStyles.Length; i++)
            {
                intersectionStyleNames[i] = StylizedWaterResources.Instance.intersectionStyles[i].name.Replace("SWS_Intersection_", string.Empty);
            }

            waveStyleNames = new string[StylizedWaterResources.Instance.waveStyles.Length];
            for (int i = 0; i < StylizedWaterResources.Instance.waveStyles.Length; i++)
            {
                waveStyleNames[i] = StylizedWaterResources.Instance.waveStyles[i].name.Replace("SWS_Waves_", string.Empty);
            }

            waveHeightmapNames = new string[StylizedWaterResources.Instance.heightmapStyles.Length];
            for (int i = 0; i < StylizedWaterResources.Instance.heightmapStyles.Length; i++)
            {
                waveHeightmapNames[i] = StylizedWaterResources.Instance.heightmapStyles[i].name.Replace("SWS_Heightmap_", string.Empty);
            }

#endif
        }

        public void OnDestroy()
        {
#if UNITY_EDITOR
            if (!meshRenderer || !meshRenderer.sharedMaterial) return;

#if !UNITY_5_5_OR_NEWER
            EditorUtility.SetSelectedWireframeHidden(meshRenderer, false);
#endif
            meshRenderer.sharedMaterial.hideFlags = HideFlags.None;
#endif
        }

        //Called through: OnEnable, OnInspectorGUI
        public void GetProperties()
        {
            //Debug.Log("StylizedWater.cs: getProperties()");

            //Determine platform, so limitations can be applied
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
             EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                isMobilePlatform = true;
                shaderNames = new string[] { "Mobile Advanced", "Mobile Basic" };
                //Array.Copy(MobileShaderNames, shaderNames, 2);
            }
            else
            {
                isMobilePlatform = false;
                shaderNames = new string[] { "[Deprecated] Desktop", "[Deprecated] Desktop (DX11 Tessellation)", "Desktop Beta", "Mobile Advanced", "Mobile Basic" };
                //Array.Copy(DesktopShaderNames, shaderNames, 4);
            }

            //Requires typeof so is always succesfull
            material = meshRenderer.sharedMaterial;

            GetShaderProperties();

#if !UNITY_5_5_OR_NEWER
            EditorUtility.SetSelectedWireframeHidden(meshRenderer, hideWireframe);
#endif
            meshRenderer.sharedMaterial.hideFlags = (hideMaterialInspector) ? HideFlags.HideInInspector : HideFlags.None;

            //If maps are missing, bake new ones
            if (shadermap == null)
            {
                GetShaderMap(useCompression, useCustomIntersection);
                GetNormalMap(useCompression, useCustomNormals);
            }

        }

        //Grab material values
        private void GetShaderProperties()
        {
            if (!material)
            {
                hasMaterial = false;
                return;
            }

            hasMaterial = true;

            //Find shaders
            DesktopShader = Shader.Find("StylizedWater/Desktop");
            DesktopTessellationShader = Shader.Find("StylizedWater/Desktop (DX11 Tessellation)");
            DesktopBetaShader = Shader.Find("StylizedWater/Desktop Beta");
            MobileAdvancedShader = Shader.Find("StylizedWater/Mobile Advanced");
            MobileBasicShader = Shader.Find("StylizedWater/Mobile Basic");

            //get current shader
            shader = material.shader;
            shaderName = shader.name;

            //Not an SWS shader
            if (!shaderName.Contains("StylizedWater")) return;

            //Recognize shader
            if (isMobilePlatform)
            {
                //Shader level specific properties
                if (shader == MobileBasicShader)
                {
                    isMobileBasic = true;
                    isMobileAdvanced = false;

                    shaderIndex = 1;
                }
                if (shader == MobileAdvancedShader)
                {
                    isMobileAdvanced = true;
                    isMobileBasic = false;

                    shaderIndex = 0;
                }
            }
            else
            {
                if (shader == DesktopShader)
                {
                    isMobileAdvanced = false;
                    isMobileBasic = false;
                    isBeta = false;

                    shaderIndex = 0;
                }
                if (shader == DesktopTessellationShader)
                {
                    isMobileAdvanced = false;
                    isMobileBasic = false;
                    isBeta = false;

                    shaderIndex = 1;
                }
                if (shader == DesktopBetaShader)
                {
                    isMobileAdvanced = false;
                    isMobileBasic = false;
                    isBeta = true;

                    shaderIndex = 2;
                }
                if (shader == MobileAdvancedShader)
                {
                    isMobileAdvanced = true;
                    isMobileBasic = false;
                    isBeta = false;

                    shaderIndex = 3;
                }
                if (shader == MobileBasicShader)
                {
                    isMobileBasic = true;
                    isMobileAdvanced = false;
                    isBeta = false;

                    shaderIndex = 4;
                }
            }

            if (hasShaderParams == false)

            //Shared by all
            shadermap = material.GetTexture("_Shadermap") as Texture2D;
            normals = material.GetTexture("_Normals") as Texture2D;

            //Color
            waterColor = material.GetColor("_WaterColor");
            rimColor = material.GetColor("_RimColor");

            //Surface
            normalStrength = material.GetFloat("_NormalStrength");
            worldSpaceTiling = (material.GetFloat("_Worldspacetiling") == 1) ? true : false;
            tiling = material.GetFloat("_Tiling");
            transparency = material.GetFloat("_Transparency");
            glossiness = material.GetFloat("_Glossiness");

            //Intersection
            rimSize = material.GetFloat("_RimSize");
            rimFalloff = material.GetFloat("_Rimfalloff");
            rimTiling = material.GetFloat("_Rimtiling");

            //Waves
            waveSpeed = material.GetFloat("_Wavesspeed");

            //Desktop only
            if (!isMobileAdvanced && !isMobileBasic)
            {
                //Color
                waveTint = material.GetFloat("_Wavetint");
                fresnelColor = material.GetColor("_FresnelColor");
                fresnel = material.GetFloat("_Fresnelexponent");

                //Surface
                refractionAmount = material.GetFloat("_RefractionAmount");
                if (!isBeta)
                {
                    reflectionCubemap = material.GetTexture("_Reflection");
                }

                //Intersection
                useIntersectionHighlight = (material.GetFloat("_UseIntersectionHighlight") == 1) ? true : false;

                //Surface highlights
                surfaceHighlightPanning = (material.GetFloat("_HighlightPanning") == 1) ? true : false;

                //Waves
                if (isBeta)
                {
                    waterShallowColor = material.GetColor("_WaterShallowColor");
                    isUnlit = (material.GetFloat("_Unlit") == 1) ? true : false;
                    waveDirection = material.GetVector("_WaveDirection");
                    waveFoam = material.GetFloat("_WaveFoam");
                    tessellation = material.GetFloat("_Tessellation");
                    reflectionStrength = material.GetFloat("_ReflectionStrength");
                    reflectionFresnel = material.GetFloat("_ReflectionFresnel");
                    waveStrength = material.GetFloat("_WaveHeight");
                }
                else
                {
                    waveStrength = material.GetFloat("_Wavesstrength");
                }
                waveSize = material.GetFloat("_WaveSize");
            }

            //Desktop and Mobile Advanced
            if (!isMobileBasic)
            {
                surfaceHighlight = material.GetFloat("_SurfaceHighlight");
                surfaceHighlightTiling = material.GetFloat("_SurfaceHightlighttiling");
                surfaceHighlightSize = material.GetFloat("_Surfacehightlightsize");

                depth = material.GetFloat("_Depth");
                if (!isBeta)
                {
                    depthDarkness = material.GetFloat("_Depthdarkness");
                }

                rimDistance = material.GetFloat("_RimDistance");
            }

            //Desktop Tesselation shader only
            if (shader == DesktopTessellationShader || shader == DesktopBetaShader)
            {
                tessellation = material.GetFloat("_Tessellation");
            }

            hasShaderParams = true;
        }

        //Apply values to material
        public void SetShaderProperties()
        {
            if (!material) return;

            //Shader level specific properties
            if (isMobilePlatform)
            {
                if (shaderIndex == 0)
                {
                    shader = MobileAdvancedShader;
                }
                else
                {
                    shader = MobileBasicShader;
                }

            }
            else
            {
                switch (shaderIndex)
                {
                    case 0:
                        shader = DesktopShader;
                        break;
                    case 1:
                        shader = DesktopTessellationShader;
                        break;
                    case 2:
                        shader = DesktopBetaShader;
                        break;
                    case 3:
                        shader = MobileAdvancedShader;
                        break;
                    case 4:
                        shader = MobileBasicShader;
                        break;
                }

            }

            material.shader = shader;

            //Shared by all
            if (shadermap && normals)
            {
                material.SetTexture("_Shadermap", shadermap);
                material.SetTexture("_Normals", normals);
            }

            //Color
            material.SetColor("_WaterColor", waterColor);
            material.SetColor("_RimColor", rimColor);

            //Surface
            material.SetFloat("_NormalStrength", normalStrength);
            material.SetFloat("_Glossiness", glossiness);
            material.SetFloat("_Worldspacetiling", (worldSpaceTiling == true) ? 1 : 0);
            material.SetFloat("_Tiling", tiling);

            //Intersection
            material.SetFloat("_RimSize", rimSize);
            material.SetFloat("_Rimfalloff", rimFalloff);

            material.SetFloat("_Rimtiling", rimTiling);

            //Waves
            material.SetFloat("_Wavesspeed", waveSpeed);

            //Desktop only
            if (!isMobileAdvanced || !isMobileBasic)
            {
                //BETA
                if (isBeta)
                {
                    material.SetColor("_WaterShallowColor", waterShallowColor);
                    material.SetFloat("_Unlit", (isUnlit == true) ? 1 : 0);
                    material.SetVector("_WaveDirection", waveDirection);
                    material.SetFloat("_WaveFoam", waveFoam);
                    material.SetFloat("_Tessellation", tessellation);
                    material.SetFloat("_ReflectionStrength", reflectionStrength);
                    material.SetFloat("_ReflectionFresnel", reflectionFresnel);
                    material.SetFloat("_WaveHeight", waveStrength);
                }

                //Color
                material.SetFloat("_Wavetint", waveTint);

                //Surface
                material.SetFloat("_Transparency", transparency);
                material.SetFloat("_RefractionAmount", refractionAmount);
                material.SetTexture("_Reflection", reflectionCubemap);

                //Surface hightlighs
                material.SetFloat("_UseIntersectionHighlight", (useIntersectionHighlight == true) ? 1 : 0);
                material.SetFloat("_HighlightPanning", (surfaceHighlightPanning == true) ? 1 : 0);

                //Waves
                material.SetFloat("_Wavesstrength", waveStrength);
                material.SetFloat("_WaveSize", waveSize);

            }

            //Desktop and Mobile Advanced
            if (!isMobileBasic)
            {
                //Color
                material.SetColor("_FresnelColor", fresnelColor);
                material.SetFloat("_Fresnelexponent", fresnel);

                //Surface highlights
                material.SetFloat("_SurfaceHighlight", surfaceHighlight);
                material.SetFloat("_SurfaceHightlighttiling", surfaceHighlightTiling);
                material.SetFloat("_Surfacehightlightsize", surfaceHighlightSize);

                //Intersection
                material.SetFloat("_RimDistance", rimDistance);

                //Depth
                material.SetFloat("_Depth", depth);
                material.SetFloat("_Depthdarkness", depthDarkness);

            }

            //Desktop tessellation shader only
            if (shader == DesktopTessellationShader || shader == DesktopBetaShader)
            {
                material.SetFloat("_Tessellation", tessellation);
            }

        }

#endif
        #endregion//Editor functions end

        #region Reflection render functions
        public void OnWillRenderObject()
        {

            if (!enabled || !material)
            {
                return;
            }

            Camera cam = Camera.current;
            if (!cam)
            {
                return;
            }

            Camera reflectionCamera;
            CreateWaterObjects(cam, out reflectionCamera);

            // find out the reflection plane: position and normal in world space
            Vector3 pos = transform.position;
            Vector3 normal = transform.up;

            #region Reflection
            // Render reflection
            if (useReflection)
            {
                UpdateCameraModes(cam, reflectionCamera);

                // Reflect camera around reflection plane
                float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
                Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                Matrix4x4 reflection = Matrix4x4.zero;
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                Vector3 oldpos = cam.transform.position;
                Vector3 newpos = reflection.MultiplyPoint(oldpos);
                reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
                reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

                reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
                reflectionCamera.targetTexture = m_ReflectionTexture;
                bool oldCulling = GL.invertCulling;
                GL.invertCulling = !oldCulling;
                reflectionCamera.transform.position = newpos;
                Vector3 euler = cam.transform.eulerAngles;
                reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

                //Manually invoke render method
                reflectionCamera.Render();
                reflectionCamera.transform.position = oldpos;
                GL.invertCulling = oldCulling;

                material.SetTexture("_ReflectionTex", m_ReflectionTexture);
            }
            #endregion


        }

        void UpdateCameraModes(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }
            // set water camera to clear the same way as current camera
            dest.clearFlags = src.clearFlags;
            dest.backgroundColor = src.backgroundColor;

            // update other values to match current camera.
            // even if we are supplying custom camera&projection matrices,
            // some of values are used elsewhere (e.g. skybox uses far plane)
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;
            dest.fieldOfView = src.fieldOfView;
            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
        }

        public void CreateReflectionTexture()
        {
            //Clear texture
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = null;
            }

            // Reflection render texture
            if (!m_ReflectionTexture || m_OldReflectionTextureSize != reflectionTextureSize)
            {

                // sws.reflectLayers = reflectLayers;
                switch (reflectionRes)
                {
                    case 0:
                        reflectionTextureSize = 64;
                        break;
                    case 1:
                        reflectionTextureSize = 128;
                        break;
                    case 2:
                        reflectionTextureSize = 256;
                        break;
                    case 3:
                        reflectionTextureSize = 512;
                        break;
                    case 4:
                        reflectionTextureSize = 1024;
                        break;
                    case 5:
                        reflectionTextureSize = 2048;
                        break;
                }

                m_ReflectionTexture = new RenderTexture(reflectionTextureSize, reflectionTextureSize, 16);
                m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
                m_ReflectionTexture.isPowerOfTwo = true;
                m_ReflectionTexture.hideFlags = HideFlags.DontSave;
                m_OldReflectionTextureSize = reflectionTextureSize;
            }
        }

        // On-demand create any objects we need for water
        void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera)
        {

            reflectionCamera = null;

            #region Reflection cam
            if (useReflection)
            {

                CreateReflectionTexture();

                // Camera for reflection
                m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
                if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    GameObject go = new GameObject("", typeof(Camera));
                    go.name = "Reflection Camera " + go.GetInstanceID() + " for " + currentCamera.name;
                    go.hideFlags = HideFlags.HideAndDontSave;

                    reflectionCamera = go.GetComponent<Camera>();
                    //Disable component as Render() is to be called manually
                    reflectionCamera.enabled = false;

                    reflectionCamera.useOcclusionCulling = false;
                    reflectionCamera.transform.position = transform.position;
                    reflectionCamera.transform.rotation = transform.rotation;

                    reflectionCamera.gameObject.AddComponent<FlareLayer>();

                    m_ReflectionCameras[currentCamera] = reflectionCamera;
                }


            }
            #endregion
        }

        // Given position/normal of the plane, calculates plane in camera space.
        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            DestroyReflectionCam();
        }

        public void DestroyReflectionCam()
        {
            //Clear texture
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = null;
            }

            //Clear cameras
            foreach (var kvp in m_ReflectionCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
            m_ReflectionCameras.Clear();
        }
        #endregion

    }//SWS class end

}//Namespace


//Easter egg, good job :)