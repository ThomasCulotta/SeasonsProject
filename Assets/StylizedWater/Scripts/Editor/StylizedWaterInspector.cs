// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SWS
{
#if UNITY_EDITOR
    [CustomEditor(typeof(StylizedWater))]
    public class StylizedWaterInspector : Editor
    {

        //Non serialized, local
        StylizedWater stylizedWater;
        SerializedProperty material;

        new SerializedObject serializedObject;

        string[] shaderNames;
        SerializedProperty shaderIndex;

        //Variables
        #region Shader parameters
        private Shader shader;

        //Colors
        SerializedProperty waterColor;
        SerializedProperty fresnelColor;
        SerializedProperty rimColor;

        //Surface
        SerializedProperty normalStrength;
        SerializedProperty worldSpaceTiling;
        SerializedProperty tiling;
        SerializedProperty transparency;
        SerializedProperty fresnel;
        SerializedProperty glossiness;
        SerializedProperty refractionAmount;

        //Intersection
        SerializedProperty rimSize;
        SerializedProperty rimFalloff;
        SerializedProperty rimDistance;
        SerializedProperty rimTiling;

        //Surface highlights
        SerializedProperty useIntersectionHighlight;
        SerializedProperty surfaceHighlightPanning;
        SerializedProperty surfaceHighlight;
        SerializedProperty surfaceHighlightSize;
        SerializedProperty surfaceHighlightTiling;

        //Depth
        SerializedProperty depth;
        SerializedProperty depthDarkness;

        //Waves
        SerializedProperty waveSpeed;
        SerializedProperty waveStrength;
        SerializedProperty waveTint;
        SerializedProperty waveSize;

        //Other
        SerializedProperty reflection;
        SerializedProperty tessellation;
        #endregion

        #region Texture parameters
        public string[] intersectionStyleNames;
        public string[] waveStyleList;
        public string[] waveHeightmapList;

        SerializedProperty useCustomIntersection;
        SerializedProperty intersectionStyle;
        SerializedProperty useCustomNormals;
        SerializedProperty waveStyle;
        SerializedProperty waveHeightmapStyle;
        #endregion

        #region Local variables
        //Input textures
        SerializedProperty customIntersection;
        SerializedProperty customNormal;
        SerializedProperty reflectionCubemap;

        GameObject selected;
        public bool isMobileAdvanced;
        public bool isMobileBasic;
        #endregion

        #region BETA
        SerializedProperty isBeta;
        SerializedProperty isUnlit; //Colors
        SerializedProperty waveFoam; //Waves
        SerializedProperty waveDirection; //Waves
        private float waveDirectionX; //Waves
        private float waveDirectionZ; //Waves
        SerializedProperty waterShallowColor; //Colors
        private SerializedProperty showReflection; //Meta
        private bool showHelpReflection; //Help

        //Reflection
        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        SerializedProperty reflectLayers;
        SerializedProperty useReflection;
        private int currentReflectionRes;
        SerializedProperty reflectionRes;
        SerializedProperty reflectionStrength;
        SerializedProperty reflectionFresnel;
        #endregion

        #region Meta
        string shaderName = null;
        SerializedProperty useCompression;

        //Section toggles
        private SerializedProperty showColors;
        private SerializedProperty showSurface;
        private SerializedProperty showIntersection;
        private SerializedProperty showHighlights;
        private SerializedProperty showDepth;
        private SerializedProperty showWaves;
        private SerializedProperty showAdvanced;

        //Help toggles
        private bool showHelp;
        private bool showHelpColors;
        private bool showHelpSurface;
        private bool showHelpIntersection;
        private bool showHelpHighlights;
        private bool showHelpDepth;
        private bool showHelpWaves;
        private bool showHelpAdvanced;

        private SerializedProperty isWaterLayer;
        private SerializedProperty hasShaderParams;
        private Shader currentShader;
#if !UNITY_5_5_OR_NEWER
        private SerializedProperty hideWireframe;
#endif
        private SerializedProperty hideMaterialInspector;

        private bool isReady = false;

        #endregion

        void OnEnable()
        {
            //Prevent OnInspectorGUI from running before these functions have been called
            isReady = false;

            selected = Selection.activeGameObject;

            if (!selected) return;

            if (!stylizedWater) stylizedWater = selected.GetComponent<StylizedWater>();

            serializedObject = new SerializedObject(stylizedWater);

            stylizedWater.Init();

            GetProperties();

            isWaterLayer.boolValue = (selected.gameObject.layer == 4) ? true : false;

            EditorUtility.SetDirty(target);

            isReady = true;
        }

        public override void OnInspectorGUI()
        {
            if (!isReady) return;

            //Prevent null values when component is added
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (stylizedWater.hasMaterial) Undo.RecordObject(material.objectReferenceValue, "Material");

            /*-------------------*/
            DrawAllFields();
            /*-------------------*/

            //Apply values
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                stylizedWater.SetShaderProperties();

                //Changes applied, grab new values
                GetProperties();

                EditorUtility.SetDirty(serializedObject.targetObject);
            }

        }

        //Get shader and Substance values
        void GetProperties()
        {
            //During runtime, nothing may be selected
            if (!selected) return;

            //Get all shader and Substance properties
            stylizedWater.GetProperties();
            hasShaderParams = serializedObject.FindProperty("hasShaderParams");
#if !UNITY_5_5_OR_NEWER
            hideWireframe = serializedObject.FindProperty("hideWireframe");
#endif
            hideMaterialInspector = serializedObject.FindProperty("hideMaterialInspector");

            material = serializedObject.FindProperty("material");
            shader = stylizedWater.shader;
            shaderIndex = serializedObject.FindProperty("shaderIndex");
            shaderNames = stylizedWater.shaderNames;

            useCompression = serializedObject.FindProperty("useCompression");
            isWaterLayer = serializedObject.FindProperty("isWaterLayer");

            shaderName = stylizedWater.shaderName;

            isMobileAdvanced = stylizedWater.isMobileAdvanced;
            isMobileBasic = stylizedWater.isMobileBasic;

            //Meta
            showColors = serializedObject.FindProperty("showColors");
            showSurface = serializedObject.FindProperty("showSurface");
            showIntersection = serializedObject.FindProperty("showIntersection");
            showHighlights = serializedObject.FindProperty("showHighlights");
            showDepth = serializedObject.FindProperty("showDepth");
            showWaves = serializedObject.FindProperty("showWaves");
            showAdvanced = serializedObject.FindProperty("showAdvanced");

            //If the shader is not an SWS shader, abort
            if (!shaderName.Contains("StylizedWater")) return;

            //BETA
            isBeta = serializedObject.FindProperty("isBeta");
            isUnlit = serializedObject.FindProperty("isUnlit");
            waterShallowColor = serializedObject.FindProperty("waterShallowColor");
            waveDirection = serializedObject.FindProperty("waveDirection");
            waveDirectionX = waveDirection.vector4Value.x;
            waveDirectionZ = waveDirection.vector4Value.z;
            waveFoam = serializedObject.FindProperty("waveFoam");
            reflectionStrength = serializedObject.FindProperty("reflectionStrength");
            reflectionFresnel = serializedObject.FindProperty("reflectionFresnel");
            showReflection = serializedObject.FindProperty("showReflection");

            useReflection = serializedObject.FindProperty("useReflection");
            reflectionRes = serializedObject.FindProperty("reflectionRes");
            reflectLayers = serializedObject.FindProperty("reflectLayers");

            #region Shader parameters
            waterColor = serializedObject.FindProperty("waterColor");
            fresnelColor = serializedObject.FindProperty("fresnelColor");
            rimColor = serializedObject.FindProperty("rimColor");
            waveTint = serializedObject.FindProperty("waveTint");

            normalStrength = serializedObject.FindProperty("normalStrength");
            transparency = serializedObject.FindProperty("transparency");
            glossiness = serializedObject.FindProperty("glossiness");
            reflectionCubemap = serializedObject.FindProperty("reflectionCubemap");
            fresnel = serializedObject.FindProperty("fresnel");

            useIntersectionHighlight = serializedObject.FindProperty("useIntersectionHighlight");
            surfaceHighlightPanning = serializedObject.FindProperty("surfaceHighlightPanning");
            surfaceHighlight = serializedObject.FindProperty("surfaceHighlight");
            surfaceHighlightSize = serializedObject.FindProperty("surfaceHighlightSize");
            surfaceHighlightTiling = serializedObject.FindProperty("surfaceHighlightTiling");

            depth = serializedObject.FindProperty("depth");
            depthDarkness = serializedObject.FindProperty("depthDarkness");

            rimSize = serializedObject.FindProperty("rimSize");
            rimFalloff = serializedObject.FindProperty("rimFalloff");
            rimDistance = serializedObject.FindProperty("rimDistance");

            worldSpaceTiling = serializedObject.FindProperty("worldSpaceTiling");
            tiling = serializedObject.FindProperty("tiling");
            rimTiling = serializedObject.FindProperty("rimTiling");

            refractionAmount = serializedObject.FindProperty("refractionAmount");

            waveSpeed = serializedObject.FindProperty("waveSpeed");
            waveStrength = serializedObject.FindProperty("waveStrength");
            waveSize = serializedObject.FindProperty("waveSize");

            tessellation = serializedObject.FindProperty("tessellation");

            #endregion

            #region Texture parameters
            intersectionStyle = serializedObject.FindProperty("intersectionStyle");
            waveStyle = serializedObject.FindProperty("waveStyle");
            waveHeightmapStyle = serializedObject.FindProperty("waveHeightmapStyle");

            useCustomNormals = serializedObject.FindProperty("useCustomNormals");
            useCustomIntersection = serializedObject.FindProperty("useCustomIntersection");

            //Substance input textures
            customIntersection = serializedObject.FindProperty("customIntersection");
            customNormal = serializedObject.FindProperty("customNormal");

            //Arrays
            intersectionStyleNames = stylizedWater.intersectionStyleNames;
            waveStyleList = stylizedWater.waveStyleNames;
            waveHeightmapList = stylizedWater.waveHeightmapNames;

            #endregion

        }

        private void BakeShaderMap()
        {
            stylizedWater.GetShaderMap(useCompression.boolValue, useCustomIntersection.boolValue);
        }

        private void BakeNormalMap()
        {
            stylizedWater.GetNormalMap(useCompression.boolValue, useCustomNormals.boolValue);
        }

        //Draw inspector fields
        void DrawAllFields()
        {
            DoHeader();

            if (!stylizedWater.hasMaterial || !shaderName.Contains("StylizedWater"))
            {
                EditorGUILayout.HelpBox("Please assign a material with a \"StylizedWater\" shader to this object.", MessageType.Error);
                return;
            }
            else
            {
                shaderIndex.intValue = EditorGUILayout.Popup("Current shader:", (int)shaderIndex.intValue, shaderNames, new GUILayoutOption[0]);
                if (shader != currentShader)
                {
                    //Shader changed, current values of material are unknown
                    //Also executed OnEnable
                    hasShaderParams.boolValue = false;
                    currentShader = shader;
                    GetProperties();
                }

                EditorGUILayout.Space();
            }

            if (stylizedWater.isMobilePlatform && shaderName.Contains("Desktop"))
            {
                EditorGUILayout.HelpBox("You are using a desktop shader on a mobile platform, which is not supported.\n\nThis will be automatically corrected.", MessageType.Error);
            }

            //StandaloneOSXIntel was removed in 2017.3, throws a warning on MacOS
#if !UNITY_2017_3_OR_NEWER
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel && shaderName.Contains("DX11"))
#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX && shaderName.Contains("DX11"))
#endif
            {
                EditorGUILayout.HelpBox("The tesellation shader is not supported on macOS.\n\nSwitch back to the standard Desktop shader.", MessageType.Error);
            }

            DoColors();

            DoSurface();

            if (isBeta.boolValue)
            {
                DoReflection();
            }

            DoInterSection();

            DoSurfaceHighlights();

            //Depth is moved to the Colors section
            if (!isBeta.boolValue)
            {
                DoDepth();
            }

            DoWaves();

            DoAdvanced();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply all settings", GUILayout.MaxHeight(35)))
            {
                BakeShaderMap();
                BakeNormalMap();
            }
            EditorGUILayout.EndHorizontal();



            DoFooter();
        }

#region Fields
        void DoHeader()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Stylized Water Shader " + StylizedWater.VERSION, new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                fontSize = 12,
            });
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            EditorGUILayout.EndHorizontal();


            if (showHelp)
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Open online documentation"))
                {
                    Application.OpenURL(StylizedWater.DOC_URL);
                }
            }

            //Show deprecation warning for desktop
            if (!isMobileAdvanced && !isMobileBasic)
            {
                if (!isBeta.boolValue)
                    EditorGUILayout.HelpBox("This shader will be removed from the package in the next update.\n\nPlease use the Desktop Beta shader, which is modernized and updated with new features.", MessageType.Error);
            }
            if (isBeta.boolValue)
            {
                EditorGUILayout.HelpBox("This shader will replace the deprecated Desktop shaders in the next update.", MessageType.Info);
            }

            if (showHelp) EditorGUILayout.HelpBox("This GUI allows you to customize the water material and bake textures containing an intersection, wave and heightmap style per material\n\nBaked textures are saved in a \"/Textures/\" folder next to the Material file.", MessageType.Info);

            EditorGUILayout.Space();

        }

        void DoColors()
        {
            //Head
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((showColors.boolValue) ? "-" : "+", CollapseButton))
            {
                showColors.boolValue = !showColors.boolValue;
            }
            if (GUILayout.Button("Colors", GroupFoldout))
            {
                showColors.boolValue = !showColors.boolValue;
            }
            showHelpColors = GUILayout.Toggle((showColors.boolValue) ? showHelpColors : false, HelpIcon, HelpButtonStyle);
            EditorGUILayout.EndHorizontal();

            //Body
            if (showColors.boolValue)
            {
                EditorGUILayout.Space();

                //BETA
                if (isBeta.boolValue)
                {
                    EditorGUILayout.PropertyField(isUnlit);
                    if (showHelpColors) EditorGUILayout.HelpBox("Unlit mode will make the surface emissive, essentially overriding any lighting.", MessageType.Info);
                }
                EditorGUILayout.PropertyField(waterColor, new GUIContent("Water"));
                //BETA
                if (isBeta.boolValue)
                {
                    EditorGUILayout.PropertyField(waterShallowColor, new GUIContent("Shallow"));
                    if (showHelpColors) EditorGUILayout.HelpBox("The color where the water is shallow, alpha channel controls the opacity.\nThe Depth parameter has an influence on this effect.", MessageType.Info);

                    if (showHelpColors) EditorGUILayout.HelpBox("If the depth effect doesn't appear, be sure to add the \"EnableDepthBuffer\" script to your camera.", MessageType.Warning);

                    EditorGUILayout.PropertyField(depth, new GUIContent("Depth"));
                    if (showHelpColors) EditorGUILayout.HelpBox("Sets the depth of the water.", MessageType.Info);
                }

                if (!isMobileBasic && !isMobileAdvanced)
                {
                    EditorGUILayout.PropertyField(fresnelColor, new GUIContent("Fresnel"));
                    if (showHelpColors) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);
                    EditorGUILayout.PropertyField(fresnel, new GUIContent("Fresnel distance"));
                    if (showHelpColors) EditorGUILayout.HelpBox("Controls the distance of the fresnel effect. Higher values push it further towards the horizon.", MessageType.Info);


                }
                EditorGUILayout.PropertyField(rimColor, new GUIContent("Intersection"));
                if (showHelpColors) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);

                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.PropertyField(waveTint, new GUIContent("Wave tint"));
                    if (showHelpColors) EditorGUILayout.HelpBox("Brightens or darkerns the water, based on the chosen heightmap in the Waves options section.", MessageType.Info);

                }
            }

            EditorGUILayout.Space();
        }

        void DoSurface()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showSurface.boolValue) ? "-" : "+", CollapseButton))
            {
                showSurface.boolValue = !showSurface.boolValue;
            }
            if (GUILayout.Button("Surface", GroupFoldout))
            {
                showSurface.boolValue = !showSurface.boolValue;
            }
            showHelpSurface = GUILayout.Toggle((showSurface.boolValue) ? showHelpSurface : false, HelpIcon, HelpButtonStyle);
            EditorGUILayout.EndHorizontal();

            //Body
            if (showSurface.boolValue)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(normalStrength, new GUIContent("Normal strength"));

                if (showHelpSurface) EditorGUILayout.HelpBox("The normal are also used for the reflection highlight, so the normal strength also has an effect on it.", MessageType.Info);
                EditorGUILayout.PropertyField(worldSpaceTiling, new GUIContent("World-space tiling"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Rather than using the mesh's UV, you can opt to use world-space coordinates. Which are independent from the object's scale and position.", MessageType.Info);
                EditorGUILayout.PropertyField(tiling, new GUIContent("Tiling"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Sets the size of the entire material at which it repeats.", MessageType.Info);


                EditorGUILayout.PropertyField(transparency, new GUIContent("Transparency"));

                EditorGUILayout.PropertyField(glossiness, new GUIContent("Glossiness"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Determine how glossy the material is. Higher values show more light reflection.", MessageType.Info);

                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.PropertyField(refractionAmount, new GUIContent("Refraction"));
                    if (showHelpSurface) EditorGUILayout.HelpBox("Bends the light passing through the water surface, creating a visual distortion.", MessageType.Info);

                    if (!isBeta.boolValue)
                    {
                        EditorGUILayout.PropertyField(reflectionCubemap, new GUIContent("Reflection cubemap"));
                        if (showHelpSurface) EditorGUILayout.HelpBox("The glossiness value has a direct effect on the intensity of this cubemap.", MessageType.Info);
                    }

                }

            }
            EditorGUILayout.Space();
        }

        void DoReflection()
        {
            if (!isMobileAdvanced && !isMobileBasic)
            {
                //Head
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button((showReflection.boolValue) ? "-" : "+", CollapseButton))
                {
                    showReflection.boolValue = !showReflection.boolValue;
                }
                if (GUILayout.Button("Planar reflections", GroupFoldout))
                {
                    showReflection.boolValue = !showReflection.boolValue;
                }
                showHelpReflection = GUILayout.Toggle((showReflection.boolValue) ? showHelpReflection : false, HelpIcon, HelpButtonStyle);
                EditorGUILayout.EndHorizontal();

                if (showReflection.boolValue)
                {
                    EditorGUILayout.Space();
                    if (showHelpReflection) EditorGUILayout.HelpBox("This feature creates a secondary camera, to render a reflection.\nThis can have a huge performance impact, be sure to make use of the layer culling option.", MessageType.Warning);

                    useReflection.boolValue = EditorGUILayout.Toggle("Enable reflections", useReflection.boolValue);


                    if (useReflection.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Resolution");
                        reflectionRes.intValue = EditorGUILayout.Popup(reflectionRes.intValue, reslist, new GUILayoutOption[0]);
                        if (reflectionRes.intValue != currentReflectionRes)
                        {
                            stylizedWater.CreateReflectionTexture();
                            currentReflectionRes = reflectionRes.intValue;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(reflectionStrength, new GUIContent("Strength"));
                        EditorGUILayout.PropertyField(reflectionFresnel, new GUIContent("Fresnel"));

                        EditorGUILayout.PropertyField(reflectLayers);
                        if (showHelpReflection) EditorGUILayout.HelpBox("Choose which layers should be reflected. The \"Water\" layer is always ignored.", MessageType.Warning);

                    }
                    //Disable reflection functionality
                    else
                    {
                        stylizedWater.DestroyReflectionCam();
                        reflectionStrength.floatValue = 0f;
                    }

                }

                EditorGUILayout.Space();

            }

        }

        void DoInterSection()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showIntersection.boolValue) ? "-" : "+", CollapseButton))
            {
                showIntersection.boolValue = !showIntersection.boolValue;
            }
            if (GUILayout.Button("Intersection", GroupFoldout))
            {
                showIntersection.boolValue = !showIntersection.boolValue;
            }
            showHelpIntersection = GUILayout.Toggle((showIntersection.boolValue) ? showHelpIntersection : false, HelpIcon, HelpButtonStyle);
            EditorGUILayout.EndHorizontal();

            //Body
            if (showIntersection.boolValue)
            {
                EditorGUILayout.Space();

                if (showHelpIntersection) EditorGUILayout.HelpBox("If the intersection effect doesn't appear, be sure to add the \"EnableDepthBuffer\" script to your camera.", MessageType.Warning);


                EditorGUILayout.PropertyField(useCustomIntersection, new GUIContent("Use custom texture"));
                if (showHelpIntersection) EditorGUILayout.HelpBox("Choose a black/white texture, it will automatically be packed into the the \"shadermap\" texture channel.", MessageType.Info);

                if (useCustomIntersection.boolValue)
                {
                    EditorGUILayout.PropertyField(customIntersection, new GUIContent("Grayscale texture"));
                    if (customIntersection.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Texture field may not be empty", MessageType.Warning);
                    }

                }
                EditorGUILayout.BeginHorizontal();
                if (!useCustomIntersection.boolValue)
                {
                    intersectionStyle.intValue = EditorGUILayout.Popup("Style", (int)intersectionStyle.intValue, intersectionStyleNames, new GUILayoutOption[0]);
                }

                if (GUILayout.Button("Apply", EditorStyles.miniButton))
                {
                    BakeShaderMap();
                }
                EditorGUILayout.EndHorizontal();
                if (showHelpIntersection) EditorGUILayout.HelpBox("Pick one of the built-in intersection texture styles.", MessageType.Info);

                EditorGUILayout.PropertyField(rimSize, new GUIContent("Size"));
                if (showHelpIntersection) EditorGUILayout.HelpBox("Increase the size of the intersection effect.", MessageType.Info);

                EditorGUILayout.PropertyField(rimFalloff, new GUIContent("Falloff"));
                if (showHelpIntersection) EditorGUILayout.HelpBox("Controls how strongly the effect should taper off.", MessageType.Info);


                if (!isMobileBasic)
                {
                    EditorGUILayout.PropertyField(rimDistance, new GUIContent("Distance"));
                    if (showHelpIntersection) EditorGUILayout.HelpBox("Adds an offset to the effect from the intersecting object.", MessageType.Info);

                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    rimTiling.floatValue -= .5f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue -= .1f;
                }
                EditorGUILayout.PropertyField(rimTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue += .1f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    rimTiling.floatValue += .5f;
                }
                EditorGUILayout.EndHorizontal();
                if (rimTiling.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (showHelpIntersection) EditorGUILayout.HelpBox("Controls the tiling size of the intersection texture.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);

            }

            EditorGUILayout.Space();
        }

        void DoSurfaceHighlights()
        {
            if (!isMobileBasic)
            {
                //Head
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button((showHighlights.boolValue) ? "-" : "+", CollapseButton))
                {
                    showHighlights.boolValue = !showHighlights.boolValue;
                }
                if (GUILayout.Button("Surface highlights", GroupFoldout))
                {
                    showHighlights.boolValue = !showHighlights.boolValue;
                }
                showHelpHighlights = GUILayout.Toggle((showHighlights.boolValue) ? showHelpHighlights : false, HelpIcon, HelpButtonStyle);
                EditorGUILayout.EndHorizontal();

                if (showHighlights.boolValue)
                {

                    EditorGUILayout.Space();

                    if (!isMobileAdvanced)
                    {

                        EditorGUI.BeginDisabledGroup(intersectionStyle.intValue == 0);
                        EditorGUILayout.PropertyField(useIntersectionHighlight, new GUIContent("Use intersection texture"));
                        if (showHelpHighlights) EditorGUILayout.HelpBox("Instead of sampling the highlight from the wave normal map, use the intersection texture.", MessageType.Info);
                        EditorGUI.EndDisabledGroup();
                        if (intersectionStyle.intValue == 0)
                        {
                            EditorGUILayout.HelpBox("Intersection style is set to \"None\", this option is disabled.", MessageType.Info);
                            useIntersectionHighlight.boolValue = false;
                        }

                        if (useIntersectionHighlight.boolValue)
                        {
                            EditorGUILayout.PropertyField(surfaceHighlightPanning, new GUIContent("Cross-panning textures"));
                            if (showHelpHighlights) EditorGUILayout.HelpBox("Enable an additional cross-panning texture.", MessageType.Info);

                        }
                    }
                    else
                    {
                        useIntersectionHighlight.boolValue = false;
                        surfaceHighlightPanning.boolValue = false;
                    }

                    EditorGUILayout.PropertyField(surfaceHighlight, new GUIContent("Opacity"));
                    if (showHelpHighlights) EditorGUILayout.HelpBox("Controls the intensity of the effect.", MessageType.Info);


                    if (!useIntersectionHighlight.boolValue)
                    {
                        EditorGUILayout.PropertyField(surfaceHighlightSize, new GUIContent("Size"));

                    }

                    if (isBeta.boolValue)
                    {
                        EditorGUILayout.PropertyField(waveFoam, new GUIContent("Wave Foam"));
                        if (showHelpHighlights) EditorGUILayout.HelpBox("Shows the surface highlight based on the heightmap", MessageType.Info);
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tiling");
                    if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                    {
                        surfaceHighlightTiling.floatValue -= .5f;
                    }
                    if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                    {
                        surfaceHighlightTiling.floatValue -= .1f;
                    }
                    EditorGUILayout.PropertyField(surfaceHighlightTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                    if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                    {
                        surfaceHighlightTiling.floatValue += .1f;
                    }
                    if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                    {
                        surfaceHighlightTiling.floatValue += .5f;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (surfaceHighlightTiling.floatValue == 0)
                    {
                        EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                    }
                    if (showHelpHighlights) EditorGUILayout.HelpBox("Controls the tiling size of the surface highlight texture.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);
                }

                EditorGUILayout.Space();
            }//not mobile basic
        }

        void DoDepth()
        {
            if (!isMobileBasic)
            {
                //Head
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button((showDepth.boolValue) ? "-" : "+", CollapseButton))
                {
                    showDepth.boolValue = !showDepth.boolValue;
                }
                if (GUILayout.Button("Depth", GroupFoldout))
                {
                    showDepth.boolValue = !showDepth.boolValue;
                }
                showHelpDepth = GUILayout.Toggle((showDepth.boolValue) ? showHelpDepth : false, HelpIcon, HelpButtonStyle);
                EditorGUILayout.EndHorizontal();


                if (showDepth.boolValue)
                {
                    EditorGUILayout.Space();

                    if (showHelpDepth) EditorGUILayout.HelpBox("If the depth effect doesn't appear, be sure to add the \"EnableDepthBuffer\" script to your camera.", MessageType.Warning);

                    EditorGUILayout.PropertyField(depth, new GUIContent("Depth"));
                    if (showHelpDepth) EditorGUILayout.HelpBox("Sets the depth of the water.", MessageType.Info);

                    EditorGUILayout.PropertyField(depthDarkness, new GUIContent("Darkness"));
                    if (showHelpDepth) EditorGUILayout.HelpBox("Controls the darkness of the deep water.", MessageType.Info);

                }

                EditorGUILayout.Space();
            }
        }

        void DoWaves()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showWaves.boolValue) ? "-" : "+", CollapseButton))
            {
                showWaves.boolValue = !showWaves.boolValue;
            }
            if (GUILayout.Button("Waves", GroupFoldout))
            {
                showWaves.boolValue = !showWaves.boolValue;
            }
            showHelpWaves = GUILayout.Toggle((showWaves.boolValue) ? showHelpWaves : false, HelpIcon, HelpButtonStyle);
            EditorGUILayout.EndHorizontal();

            //Body
            if (showWaves.boolValue)
            {

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useCustomNormals, new GUIContent("Use custom normal map"));
                if (showHelpWaves) EditorGUILayout.HelpBox("Opt to use a custom normal map for the waves", MessageType.Info);

                if (useCustomNormals.boolValue)
                {
                    EditorGUILayout.PropertyField(customNormal, new GUIContent("Normal map"));

                    if (customNormal.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Field cannot be empty", MessageType.Warning);
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (!useCustomNormals.boolValue)
                {
                    waveStyle.intValue = EditorGUILayout.Popup("Small waves style", (int)waveStyle.intValue, waveStyleList, new GUILayoutOption[0]);

                }
                if (GUILayout.Button("Apply", EditorStyles.miniButton))
                {
                    BakeNormalMap();

                }
                EditorGUILayout.EndHorizontal();
                if (showHelpWaves) EditorGUILayout.HelpBox("Choose from one of the built-in normal maps", MessageType.Info);

                EditorGUILayout.Space();

                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.BeginHorizontal();
                    waveHeightmapStyle.intValue = EditorGUILayout.Popup("Big waves animation", (int)waveHeightmapStyle.intValue, waveHeightmapList, new GUILayoutOption[0]);

                    if (GUILayout.Button("Apply", EditorStyles.miniButton))
                    {
                        BakeShaderMap();

                    }
                    EditorGUILayout.EndHorizontal();
                    if (showHelpWaves) EditorGUILayout.HelpBox("Choose from one of the built-in height maps.", MessageType.Info);

                    EditorGUILayout.Space();
                }

                EditorGUILayout.PropertyField(waveSpeed, new GUIContent("Speed"));
                if (showHelpWaves) EditorGUILayout.HelpBox("Overal speed of the wave animations.", MessageType.Info);

                //BETA
                if (isBeta.boolValue)
                {
                    waveDirectionX = EditorGUILayout.Slider("Direction X", waveDirectionX, 0f, 1f);
                    waveDirectionZ = EditorGUILayout.Slider("Direction Z", waveDirectionZ, 0f, 1f);

                    waveDirection.vector4Value = new Vector4(waveDirectionX, 0f, waveDirectionZ, 0f);
                }

                if (!isMobileBasic)
                {
                    EditorGUILayout.PropertyField(waveStrength, new GUIContent("Height"));
                    if (showHelpWaves) EditorGUILayout.HelpBox("height of the waves, this can also be controlled through the Y-axis scale of the object, at least for planes.", MessageType.Info);
                }

                if (!isMobileAdvanced && !isMobileBasic)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tiling");
                    if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                    {
                        waveSize.floatValue -= .1f;
                    }
                    if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                    {
                        waveSize.floatValue -= .01f;
                    }
                    EditorGUILayout.PropertyField(waveSize, new GUIContent(""), GUILayout.MaxWidth(45));
                    if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                    {
                        waveSize.floatValue += .01f;
                    }
                    if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                    {
                        waveSize.floatValue += .1f;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (waveSize.floatValue == 0)
                    {
                        EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                    }
                    if (showHelpWaves) EditorGUILayout.HelpBox("Controls the tiling size of the wave animation.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);
                }
            }


            EditorGUILayout.Space();
        }

        void DoAdvanced()
        {

            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showAdvanced.boolValue) ? "-" : "+", CollapseButton))
            {
                showAdvanced.boolValue = !showAdvanced.boolValue;
            }
            if (GUILayout.Button("Advanced", GroupFoldout))
            {
                showAdvanced.boolValue = !showAdvanced.boolValue;
            }
            showHelpAdvanced = GUILayout.Toggle((showAdvanced.boolValue) ? showHelpAdvanced : false, HelpIcon, HelpButtonStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //Body
            if (showAdvanced.boolValue)
            {
#if !UNITY_5_5_OR_NEWER
                EditorGUILayout.PropertyField(hideWireframe, new GUIContent("Hide wireframe"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Hides the wireframe of the selected object, for easier visual tuning.", MessageType.Info);
#endif
                EditorGUILayout.PropertyField(hideMaterialInspector, new GUIContent("Hide material inspector"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Hides material inspector, for less clutter", MessageType.Info);

                if (shaderName.Contains("Tessellation") || shaderName.Contains("Desktop Beta"))
                {
                    EditorGUILayout.PropertyField(tessellation, new GUIContent("Tessellation"));
                    if (showHelpAdvanced) EditorGUILayout.HelpBox("Tessellation creates additional mesh vertices, resulting in higher detail wave animations.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(useCompression, new GUIContent("Compressed textures"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Bake the textures using compression, trading in quality for a smaller file size.\n\nPVRTC compression is used for mobile, DXT5 is used on other platforms.", MessageType.Info);
            }

            EditorGUILayout.Space();

        }

        void DoFooter()
        {
            EditorGUILayout.Space();

            GUILayout.Label("- Staggart Creations -", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });
        }
#endregion

#region Styles
        private static GUIStyle _HelpButtonStyle;
        public static GUIStyle HelpButtonStyle
        {
            get
            {
                if (_HelpButtonStyle == null)
                {
                    _HelpButtonStyle = new GUIStyle(EditorStyles.miniButtonRight)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 50,
                        fixedHeight = 21.5f
                    };
                }

                return _HelpButtonStyle;
            }
        }

        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }

                return _HelpIcon;
            }
        }

        private static GUIContent _HelpButtonContent;
        public static GUIContent HelpButtonContent
        {
            get
            {
                if (_HelpButtonContent == null)
                {
                    _HelpButtonContent = new GUIContent()
                    {
                        image = HelpIcon,
                    };
                }

                return _HelpButtonContent;
            }
        }

        private static GUIStyle _CollapseButton;
        public static GUIStyle CollapseButton
        {
            get
            {
                if (_CollapseButton == null)
                {
                    _CollapseButton = new GUIStyle(EditorStyles.miniButtonLeft)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 30,
                        fixedHeight = 21.5f,
                    };
                }

                return _CollapseButton;
            }
        }

        private static GUIStyle _GroupFoldout;
        public static GUIStyle GroupFoldout
        {
            get
            {
                if (_GroupFoldout == null)
                {
                    _GroupFoldout = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        fontSize = 11,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        padding = new RectOffset()
                        {
                            left = 10,
                            top = 4,
                            bottom = 5
                        }
                    };
                }

                return _GroupFoldout;
            }
        }


#endregion

    }//Class
}//Namespace
#endif
