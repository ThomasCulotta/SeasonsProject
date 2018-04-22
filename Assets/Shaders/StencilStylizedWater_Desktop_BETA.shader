// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/StencilWater"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector] _DummyTex( "", 2D ) = "white" {}
        _StencilMask ("StencilMask", int) = 0
		_WaterColor("Water Color", Color) = (0.1176471,0.6348885,1,0)
		_WaterShallowColor("WaterShallowColor", Color) = (0.4191176,0.7596349,1,0)
		_FresnelColor("Fresnel Color", Color) = (1,1,1,0.484)
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_NormalStrength("NormalStrength", Range( 0 , 1)) = 1
		_Transparency("Transparency", Range( 0 , 1)) = 0.75
		_Glossiness("Glossiness", Range( 0 , 1)) = 1
		_Fresnelexponent("Fresnel exponent", Float) = 4
		_ReflectionStrength("Reflection Strength", Range( 0 , 1)) = 0
		_ReflectionFresnel("Reflection Fresnel", Range( 2 , 10)) = 0.5
		_RefractionAmount("Refraction Amount", Range( 0 , 0.1)) = 0
		_Worldspacetiling("Worldspace tiling", Float) = 1
		_Tiling("Tiling", Range( 0 , 1)) = 0.9
		_RimDistance("Rim Distance", Range( 0.01 , 3)) = 0.2448298
		_RimSize("Rim Size", Range( 0 , 20)) = 0
		_Rimfalloff("Rim falloff", Range( 0.1 , 50)) = 0
		_Rimtiling("Rim tiling", Float) = 2
		_SurfaceHighlight("Surface Highlight", Range( -1 , 1)) = 0.05
		_HighlightPanning("HighlightPanning", Float) = 0
		_Surfacehightlightsize("Surface hightlight size", Float) = 0
		_SurfaceHightlighttiling("Surface Hightlight tiling", Float) = 0.25
		_Depth("Depth", Range( 0 , 100)) = 30
		_Wavesspeed("Waves speed", Range( 0 , 10)) = 0.75
		_WaveHeight("Wave Height", Range( 0 , 1)) = 0
		_Tessellation("Tessellation", Range( 0.1 , 100)) = 0.1
		_Wavetint("Wave tint", Range( -1 , 1)) = 0
		_WaveFoam("Wave Foam", Range( 0 , 10)) = 0
		_WaveSize("Wave Size", Range( 0 , 10)) = 0.5
		_WaveDirection("WaveDirection", Vector) = (0,0,0,0)
		[NoScaleOffset][Normal]_Normals("Normals", 2D) = "bump" {}
		[NoScaleOffset]_Shadermap("Shadermap", 2D) = "black" {}
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "gray" {}
		[Toggle]_Unlit("Unlit", Float) = 0
		_UseIntersectionHighlight("UseIntersectionHighlight", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		LOD 200

        Stencil
        {
            Ref[_StencilMask]
            Comp equal
            Pass keep
            Fail keep
        }

		Cull Back
		GrabPass{ "_GrabScreen0" }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard alpha:fade keepalpha nodirlightmap vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_DummyTex;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _Normals;
		uniform float _Worldspacetiling;
		uniform sampler2D _DummyTex;
		uniform float _Tiling;
		uniform float _Wavesspeed;
		uniform float4 _WaveDirection;
		uniform float _SurfaceHighlight;
		uniform float _Surfacehightlightsize;
		uniform sampler2D _Shadermap;
		uniform float _WaveSize;
		uniform float _SurfaceHightlighttiling;
		uniform float _UseIntersectionHighlight;
		uniform float _HighlightPanning;
		uniform float4 _RimColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Rimfalloff;
		uniform float _Rimtiling;
		uniform float _RimSize;
		uniform float _NormalStrength;
		uniform float _Unlit;
		uniform sampler2D _GrabScreen0;
		uniform float _RefractionAmount;
		uniform float4 _WaterShallowColor;
		uniform float4 _WaterColor;
		uniform float _Depth;
		uniform float _Transparency;
		uniform sampler2D _ReflectionTex;
		uniform float _ReflectionStrength;
		uniform float _ReflectionFresnel;
		uniform float _Wavetint;
		uniform float4 _FresnelColor;
		uniform float _Fresnelexponent;
		uniform float _WaveFoam;
		uniform float _Glossiness;
		uniform float _RimDistance;
		uniform float _WaveHeight;
		uniform float _Tessellation;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			float4 temp_cast_1 = (_Tessellation).xxxx;
			return temp_cast_1;
		}

		void vertexDataFunc( inout appdata v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			v.texcoord.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 Tiling21 = ( lerp(( -20.0 * v.texcoord.xy ),(ase_worldPos).xz,_Worldspacetiling) * ( 1.0 - _Tiling ) );
			float2 appendResult500 = (float2(_WaveDirection.x , _WaveDirection.z));
			float2 WaveSpeed40 = ( ( ( _Wavesspeed * 0.1 ) * _Time.y ) * appendResult500 );
			float2 temp_output_344_0 = ( ( ( Tiling21 * _WaveSize ) * 0.1 ) + ( WaveSpeed40 * 0.5 ) );
			float4 tex2DNode94 = tex2Dlod( _Shadermap, float4( temp_output_344_0, 0.0 , 0.0 ) );
			float temp_output_95_0 = ( _WaveHeight * tex2DNode94.g );
			float3 Displacement100 = ( ase_vertexNormal * temp_output_95_0 );
			v.vertex.xyz += Displacement100;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 texCoordDummy12 = i.uv_DummyTex*float2( 1,1 ) + float2( 0,0 );
			float3 ase_worldPos = i.worldPos;
			float2 Tiling21 = ( lerp(( -20.0 * texCoordDummy12 ),(ase_worldPos).xz,_Worldspacetiling) * ( 1.0 - _Tiling ) );
			float2 appendResult500 = (float2(_WaveDirection.x , _WaveDirection.z));
			float2 WaveSpeed40 = ( ( ( _Wavesspeed * 0.1 ) * _Time.y ) * appendResult500 );
			float2 temp_output_339_0 = ( ( Tiling21 * 0.25 ) + WaveSpeed40 );
			float3 temp_output_51_0 = BlendNormals( UnpackNormal( tex2D( _Normals, ( Tiling21 + -WaveSpeed40 ) ) ) , UnpackNormal( tex2D( _Normals, temp_output_339_0 ) ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float2 temp_output_344_0 = ( ( ( Tiling21 * _WaveSize ) * 0.1 ) + ( WaveSpeed40 * 0.5 ) );
			float4 tex2DNode94 = tex2D( _Shadermap, temp_output_344_0 );
			float Heightmap99 = tex2DNode94.g;
			float temp_output_609_0 = ( Heightmap99 * 0.1 );
			float4 tex2DNode66 = tex2D( _Shadermap, ( ( ( Tiling21 * 0.5 ) + temp_output_609_0 ) * _SurfaceHightlighttiling ) );
			float lerpResult600 = lerp( tex2DNode66.r , tex2DNode66.b , _UseIntersectionHighlight);
			float4 tex2DNode67 = tex2D( _Shadermap, ( _SurfaceHightlighttiling * ( Tiling21 + -temp_output_609_0 ) ) );
			float lerpResult598 = lerp( tex2DNode67.r , tex2DNode67.b , _UseIntersectionHighlight);
			float lerpResult601 = lerp( step( _Surfacehightlightsize , ( lerpResult600 - lerpResult598 ) ) , ( 1.0 - lerp(lerpResult598,( lerpResult600 * lerpResult598 ),_HighlightPanning) ) , _UseIntersectionHighlight);
			float SurfaceHighlights73 = ( _SurfaceHighlight * lerpResult601 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float screenDepth493 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth493 = abs( ( screenDepth493 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / ( 1.0 ) );
			float DepthTexture494 = distanceDepth493;
			float2 temp_output_24_0 = ( Tiling21 * _Rimtiling );
			float3 NormalsBlended362 = temp_output_51_0;
			float temp_output_30_0 = ( tex2D( _Shadermap, ( float3( ( 0.5 * temp_output_24_0 ) ,  0.0 ) + ( NormalsBlended362 * 0.1 ) ).xy ).b * tex2D( _Shadermap, ( temp_output_24_0 + WaveSpeed40 ) ).b );
			float clampResult438 = clamp( ( _RimColor.a * ( 1.0 - ( ( ( DepthTexture494 / _Rimfalloff ) * temp_output_30_0 * 3.0 ) + ( DepthTexture494 / _RimSize ) ) ) ) , 0.0 , 1.0 );
			float Intersection42 = clampResult438;
			float3 lerpResult82 = lerp( temp_output_51_0 , ase_vertexNormal , ( SurfaceHighlights73 + Intersection42 ));
			float3 lerpResult621 = lerp( float3(0.5,0.5,1) , lerpResult82 , _NormalStrength);
			float3 NormalMap52 = lerpResult621;
			o.Normal = NormalMap52;
			float4 ase_screenPos266 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale266 = -1.0;
			#else
			float scale266 = 1.0;
			#endif
			float halfPosW266 = ase_screenPos266.w * 0.5;
			ase_screenPos266.y = ( ase_screenPos266.y - halfPosW266 ) * _ProjectionParams.x* scale266 + halfPosW266;
			#ifdef UNITY_SINGLE_PASS_STEREO
			ase_screenPos266.xy = TransformStereoScreenSpaceTex(ase_screenPos266.xy, ase_screenPos266.w);
			#endif
			ase_screenPos266.xyzw /= ase_screenPos266.w;
			float2 appendResult501 = (float2(ase_screenPos266.r , ase_screenPos266.g));
			float3 temp_output_359_0 = ( ( _RefractionAmount * NormalsBlended362 ) + float3( appendResult501 ,  0.0 ) );
			float4 screenColor372 = tex2D( _GrabScreen0, temp_output_359_0.xy );
			float4 RefractionResult126 = screenColor372;
			float screenDepth105 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth105 = abs( ( screenDepth105 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / ( _Depth ) );
			float clampResult144 = clamp( distanceDepth105 , 0.0 , 1.0 );
			float Depth479 = clampResult144;
			float4 lerpResult478 = lerp( _WaterShallowColor , _WaterColor , Depth479);
			float clampResult133 = clamp( ( ( _Transparency + Intersection42 ) - ( ( 1.0 - Depth479 ) * ( 1.0 - _WaterShallowColor.a ) ) ) , 0.0 , 1.0 );
			float Opacity121 = clampResult133;
			float4 lerpResult374 = lerp( RefractionResult126 , lerpResult478 , Opacity121);
			float4 Reflection265 = tex2D( _ReflectionTex, temp_output_359_0.xy );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float fresnelFinalVal508 = (0.0 + _ReflectionFresnel*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , 5.0));
			float4 lerpResult297 = lerp( lerpResult374 , Reflection265 , saturate( ( ( Opacity121 * _ReflectionStrength ) * fresnelFinalVal508 ) ));
			float4 WaterColor350 = lerpResult297;
			float4 temp_cast_6 = (( Heightmap99 * _Wavetint )).xxxx;
			float4 RimColor102 = _RimColor;
			float4 lerpResult61 = lerp( ( WaterColor350 - temp_cast_6 ) , ( RimColor102 * 3.0 ) , Intersection42);
			float4 FresnelColor206 = _FresnelColor;
			float fresnelFinalVal199 = (0.0 + 1.0*pow( 1.0 - dot( ase_vertexNormal, worldViewDir ) , ( _Fresnelexponent * 100.0 )));
			float clampResult505 = clamp( ( _FresnelColor.a * fresnelFinalVal199 ) , 0.0 , 1.0 );
			float Fresnel205 = clampResult505;
			float4 lerpResult207 = lerp( ( lerpResult61 + SurfaceHighlights73 ) , FresnelColor206 , Fresnel205);
			float4 temp_cast_7 = (1.0).xxxx;
			float SurfaceHighlightTex244 = lerpResult601;
			float clampResult401 = clamp( ( pow( ( tex2DNode94.g * _WaveFoam ) , 2.0 ) * SurfaceHighlightTex244 ) , 0.0 , 1.0 );
			float WaveFoam221 = clampResult401;
			float4 lerpResult223 = lerp( lerpResult207 , temp_cast_7 , WaveFoam221);
			float4 FinalColor114 = lerpResult223;
			o.Albedo = ( ( 1.0 - _Unlit ) * FinalColor114 ).rgb;
			o.Emission = ( _Unlit * FinalColor114 ).rgb;
			o.Smoothness = _Glossiness;
			float temp_output_498_0 = ( DepthTexture494 / _RimDistance );
			o.Alpha = saturate( temp_output_498_0 );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=13108
1927;29;1906;1004;2976.527;2258.761;1.09674;True;False
Node;AmplifyShaderEditor.CommentaryNode;347;-5386.759,-1596.891;Float;False;1730.402;709.7;Comment;10;12;16;17;18;13;20;15;19;21;14;UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;348;-5365.659,-516.9902;Float;False;1494.001;706.4609;Comment;9;37;35;38;320;36;39;337;40;500;Speed/direction;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;16;-5308.559,-1216.891;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-5336.759,-1420.591;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-5298.959,-1546.891;Float;False;Constant;_Float0;Float 0;4;0;-20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-5315.659,-466.9902;Float;False;Property;_Wavesspeed;Waves speed;22;0;0.75;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;37;-5222.758,-351.2891;Float;False;Constant;_Float1;Float 1;9;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-5044.56,-1450.29;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SwizzleNode;17;-5041.56,-1187.891;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector4Node;320;-5110.518,-17.52931;Float;False;Property;_WaveDirection;WaveDirection;28;0;0,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-4992.657,-420.189;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;38;-5136.958,-230.29;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-5282.958,-1002.19;Float;False;Property;_Tiling;Tiling;12;0;0.9;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;20;-4867.558,-1031.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;15;-4754.56,-1296.891;Float;False;Property;_Worldspacetiling;Worldspace tiling;11;0;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4715.158,-331.1895;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;500;-4842.429,16.47314;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;381;-4535.249,1990.391;Float;False;1019.923;590.8805;Comment;7;46;47;343;342;340;339;341;Cross panning UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-4508.721,-195.2295;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-4504.56,-1124.192;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;47;-4485.25,2196.391;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-3899.358,-1123.991;Float;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;343;-4132.526,2466.271;Float;False;Constant;_Float12;Float 12;34;0;0.25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;46;-4242.85,2040.391;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-4176.856,-194.9893;Float;False;WaveSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.NegateNode;340;-4130.545,2182.237;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-3900.328,2250.271;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;339;-3675.531,2281.672;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;341;-3669.329,2140.572;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;502;-2848.648,2196.59;Float;False;2098.804;676.0051;Comment;13;45;50;51;362;83;384;312;363;82;128;52;622;621;Small Wave Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;45;-2795.852,2246.59;Float;True;Property;_TextureSample2;Texture Sample 2;29;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;349;-2980.004,-251.1301;Float;False;3332.455;1003.958;Comment;12;353;618;30;620;470;356;22;23;355;24;354;619;Intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;50;-2798.648,2473.488;Float;True;Property;_TextureSample3;Texture Sample 3;29;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;51;-2354.247,2358.689;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;23;-2893.248,298.7199;Float;False;Property;_Rimtiling;Rim tiling;16;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;22;-2883.451,171.1539;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;620;-2666.217,419.6857;Float;False;Constant;_IntersectionDistortion;IntersectionDistortion;37;0;0.1;0;0.2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;355;-2617.46,59.20661;Float;False;Constant;_Float13;Float 13;34;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;618;-2653.992,320.648;Float;False;362;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;362;-2081.441,2288.368;Float;False;NormalsBlended;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2620.168,201.0484;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;41;-2755.501,561.9035;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;382;-2980.637,-1888.427;Float;False;1573.089;864.3926;Comment;15;265;260;126;372;359;361;360;269;266;490;491;492;493;494;501;Reflection/Refraction/Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;-2355.667,152.5738;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;619;-2303.155,295.9035;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;353;-2074.599,346.0621;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.DepthFade;493;-2133.029,-1098.929;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;356;-2133.195,529.7621;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;5;-1883.169,48.08686;Float;False;Property;_Rimfalloff;Rim falloff;15;0;0;0.1;50;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;494;-1687.128,-1295.229;Float;False;DepthTexture;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;407;-3022.625,3076.09;Float;False;2762.213;843.6829;Comment;24;344;94;92;91;90;87;302;89;301;220;219;232;98;100;95;218;99;231;230;229;401;221;581;96;Waves;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;495;-1791.495,-119.6358;Float;False;494;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;28;-1886.472,330.4239;Float;True;Property;_TextureSample0;Texture Sample 0;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;-1884.086,526.1537;Float;True;Property;_TextureSample1;Texture Sample 1;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1443.245,479.2134;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;302;-2972.625,3343.579;Float;False;Property;_WaveSize;Wave Size;27;0;0.5;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;87;-2866.145,3226.497;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;440;-1379.275,300.5343;Float;False;Constant;_Float6;Float 6;33;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-1904.116,196.0108;Float;False;Property;_RimSize;Rim Size;14;0;0;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;496;-1495.094,33.76423;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;91;-2602.742,3727.092;Float;False;Constant;_Float5;Float 5;16;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-2600.625,3277.579;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;90;-2614.742,3559.092;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;89;-2603.592,3428.811;Float;False;Constant;_Float4;Float 4;16;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;497;-1501.595,180.6643;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;504;-2366.36,-3909.979;Float;False;1178.949;207.293;Comment;4;104;105;144;479;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;-1175.175,117.735;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2358.742,3582.092;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-2387.947,3346.796;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;104;-2316.36,-3817.686;Float;False;Property;_Depth;Depth;21;0;30;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;425;-919.8746,213.9351;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;426;-724.575,207.9351;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;344;-2130.825,3469.473;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ColorNode;10;-868.8976,-209.3241;Float;False;Property;_RimColor;Rim Color;3;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DepthFade;105;-1977.834,-3821.543;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;380;-2383.622,-3473.776;Float;False;1667.978;443.2719;Comment;8;133;149;151;134;119;480;487;488;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;94;-1942.843,3444.292;Float;True;Property;_TextureSample6;Texture Sample 6;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;444;-300.9756,6.9344;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;503;-2490.313,-2903.259;Float;False;1544.134;783.9609;Comment;16;477;482;60;478;377;367;271;374;298;456;297;350;106;508;509;627;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;144;-1711.533,-3859.979;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-1408.541,3485.688;Float;False;Heightmap;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;608;-4320.395,1771.448;Float;False;Constant;_SurfaceHighlightsDistortion;SurfaceHighlightsDistortion;36;0;0.1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;606;-4220.316,1623.399;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;480;-2350.512,-3118.96;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;438;-114.6765,-117.7656;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;479;-1430.411,-3848.06;Float;False;Depth;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;477;-2440.313,-2853.259;Float;False;Property;_WaterShallowColor;WaterShallowColor;1;0;0.4191176,0.7596349,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;605;-3641.392,1505.222;Float;False;Constant;_Float14;Float 14;36;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;609;-3880.803,1588.583;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;98.80185,-65.92109;Float;False;Intersection;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;117;-2333.622,-3423.776;Float;False;Property;_Transparency;Transparency;5;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;151;-2099.962,-3222;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;488;-2101.708,-3113.859;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;119;-2283.998,-3309.402;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;607;-3458.382,1744.495;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;311;-3027.042,1265.19;Float;False;2806.331;669.9998;Comment;16;63;64;65;67;66;74;69;75;68;76;70;73;244;600;598;71;Surface highlights;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;604;-3439.203,1517.725;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-1862.436,-3411.029;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;487;-1787.109,-3195.259;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;269;-2909.617,-1649.332;Float;False;362;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;602;-3182.718,1580.509;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;360;-2930.637,-1766.433;Float;False;Property;_RefractionAmount;Refraction Amount;10;0;0;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;-3002.032,1401.854;Float;False;Property;_SurfaceHightlighttiling;Surface Hightlight tiling;20;0;0.25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;603;-3150.212,1734.011;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;149;-1606.138,-3410.348;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GrabScreenPosition;266;-2928.616,-1554.035;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2666.634,1670.709;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;361;-2583.436,-1749.433;Float;False;2;2;0;FLOAT;0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.DynamicAppendNode;501;-2672.129,-1532.727;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2655.434,1508.632;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ClampOpNode;133;-1295.653,-3321.168;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;67;-2441.687,1688.739;Float;True;Property;_TextureSample5;Texture Sample 5;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;359;-2345.136,-1679.332;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT2;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;66;-2437.616,1454.107;Float;True;Property;_TextureSample4;Texture Sample 4;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-993.4441,-3410.152;Float;False;Opacity;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;271;-2445.613,-2357.412;Float;False;Property;_ReflectionStrength;Reflection Strength;8;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;599;-2282.809,2029.568;Float;False;Property;_UseIntersectionHighlight;UseIntersectionHighlight;34;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;377;-2129.696,-2518.808;Float;False;121;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;106;-2445.133,-2213.643;Float;False;Property;_ReflectionFresnel;Reflection Fresnel;9;0;0.5;2;10;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;372;-2019.626,-1838.427;Float;False;Global;_GrabScreen0;Grab Screen 0;34;0;Object;-1;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;600;-1962.077,1479.417;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;598;-1957.324,1725.599;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;60;-2429.441,-2657.682;Float;False;Property;_WaterColor;Water Color;0;0;0.1176471,0.6348885,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;456;-1878.173,-2449.371;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;508;-2083.906,-2292.63;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;482;-2405.114,-2482.561;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1756.314,1708.322;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;367;-2134.343,-2871.358;Float;False;126;0;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;260;-2067.917,-1583.235;Float;True;Property;_ReflectionTex;ReflectionTex;31;1;[HideInInspector];None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-1691.149,-1825.214;Float;False;RefractionResult;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;509;-1665.529,-2407.551;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;378;-2979.134,-875.097;Float;False;1619.181;563.468;Comment;4;202;211;212;213;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;478;-2051.41,-2718.959;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-1687.858,1472.809;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;75;-1587.187,1642.001;Float;False;Property;_HighlightPanning;HighlightPanning;18;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;352;-3052.353,4240.27;Float;False;3666.101;461.6461;Comment;19;114;223;315;207;224;210;208;61;475;141;62;79;111;351;103;476;112;110;625;Master lerp;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;298;-1754.346,-2602.505;Float;False;265;0;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;213;-2886.606,-426.6295;Float;False;Constant;_Float10;Float 10;26;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;374;-1710.825,-2767.75;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;265;-1669.316,-1569.436;Float;False;Reflection;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;69;-1652.994,1346.007;Float;False;Property;_Surfacehightlightsize;Surface hightlight size;19;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;627;-1494.052,-2512.787;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;201;-2929.134,-523.7986;Float;False;Property;_Fresnelexponent;Fresnel exponent;7;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;-2647.807,-476.2287;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;110;-2977.4,4396.651;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;76;-1302.29,1653.294;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;297;-1317.789,-2676.809;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.NormalVertexDataNode;200;-2688.977,-736.9108;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StepOpNode;70;-1347.171,1443.604;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;220;-1814.714,3695.973;Float;False;Property;_WaveFoam;Wave Foam;26;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;211;-2657.407,-573.8296;Float;False;Constant;_Float8;Float 8;26;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-3002.353,4547.916;Float;False;Property;_Wavetint;Wave tint;25;0;0;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;232;-1458.014,3804.772;Float;False;Constant;_Float7;Float 7;25;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1459.513,3613.473;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;351;-2647.53,4290.27;Float;False;350;0;1;COLOR
Node;AmplifyShaderEditor.LerpOp;601;-882.9985,1713.577;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2328.22,4428.309;Float;False;102;0;1;COLOR
Node;AmplifyShaderEditor.FresnelNode;199;-2407.307,-602.4576;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1002.891,1435.762;Float;False;Property;_SurfaceHighlight;Surface Highlight;17;0;0.05;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-1142.759,-2678.372;Float;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-2612.288,4385.408;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;476;-2327.409,4526.541;Float;False;Constant;_Float2;Float 2;34;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-610.3175,-177.3301;Float;False;RimColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;202;-2418.637,-825.0979;Float;False;Property;_FresnelColor;Fresnel Color;2;0;1,1,1,0.484;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;229;-1242.914,3791.573;Float;False;244;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;244;-519.71,1570.168;Float;False;SurfaceHighlightTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;475;-2080.409,4437.541;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;624;-648.6583,1456.325;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;231;-1252.414,3617.073;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;62;-2115.24,4577.445;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2100.862,-643.3526;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;141;-2371.558,4302.319;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;61;-1712.118,4305.361;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-923.914,3621.772;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;79;-1609.659,4486.451;Float;False;73;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;505;-1829.128,-639.2252;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-506.8405,1428.191;Float;False;SurfaceHighlights;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;401;-717.4038,3622.245;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;83;-2127.446,2591.788;Float;False;73;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;208;-778.3857,4473.661;Float;False;205;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;384;-2092.227,2757.595;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;625;-1234.768,4308.532;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;210;-795.2321,4377.799;Float;False;206;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;206;-2087.412,-792.5879;Float;False;FresnelColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-1574.954,-663.1936;Float;False;Fresnel;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;315;-297.6274,4363.029;Float;False;Constant;_Float9;Float 9;32;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;224;-233.1308,4480.808;Float;False;221;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;312;-1844.619,2645.57;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;207;-469.9587,4300.934;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.NormalVertexDataNode;363;-1992.141,2441.069;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;221;-503.4121,3621.273;Float;False;WaveFoam;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;96;-1617.643,3194.592;Float;False;Property;_WaveHeight;Wave Height;23;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1283.044,3319.088;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;622;-1501.788,2474.983;Float;False;Constant;_Vector0;Vector 0;36;0;0.5,0.5,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;128;-1607.249,2638.489;Float;False;Property;_NormalStrength;NormalStrength;4;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;1018.963,101.1444;Float;False;Property;_RimDistance;Rim Distance;13;0;0.2448298;0.01;3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;458;700.8949,-369.1569;Float;False;Property;_Unlit;Unlit;32;1;[Toggle];0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;223;34.14053,4299.374;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-1306.842,3126.09;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;82;-1515.746,2345.388;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;489;1070.571,6.471436;Float;False;494;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;310.277,4301.388;Float;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-965.142,3185.088;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;460;959.8949,-370.1569;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;621;-1217.088,2367.483;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;498;1352.871,26.17273;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;115;685.358,-231.2139;Float;False;114;0;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;470;-2460.069,567.034;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ClampOpNode;499;1754.871,-46.82727;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-1001.844,2344.189;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;234;1126.286,302.6716;Float;False;Property;_Tessellation;Tessellation;24;0;0.1;0.1;100;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;233;-1324.382,597.2639;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;459;1234.895,-360.1569;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;492;-2152.23,-1175.528;Float;False;Constant;_Float11;Float 11;36;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;490;-2259.729,-1380.828;Float;True;Property;_DepthTexture;_DepthTexture;33;0;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;515;-3390.761,2536.529;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;1115.356,-68.0119;Float;False;Property;_Glossiness;Glossiness;6;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-733.541,3180.291;Float;False;Displacement;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;101;1169.459,227.189;Float;False;100;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;53;1163.457,-243.0103;Float;False;52;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;581;-2043.53,3676.983;Float;False;HeightmapUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;514;-3127.757,2402.993;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;27;1061.068,-736.4829;Float;True;Property;_Shadermap;Shadermap;30;1;[NoScaleOffset];None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;457;1245.895,-163.1569;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;-1100.38,573.364;Float;False;IntersectionTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;491;-1899.529,-1308.128;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;626;1695.037,61.36169;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;43;1474.061,-735.0397;Float;True;Property;_Normals;Normals;29;2;[NoScaleOffset];[Normal];None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-980.01,3324.471;Float;False;WaveHeight;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2093.9,-246;Float;False;True;6;Float;;200;0;Standard;StylizedWater/Desktop Beta;False;False;False;False;False;False;False;False;True;False;False;False;False;False;True;True;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;200;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;17;0;16;0
WireConnection;36;0;35;0
WireConnection;36;1;37;0
WireConnection;20;0;18;0
WireConnection;15;0;13;0
WireConnection;15;1;17;0
WireConnection;39;0;36;0
WireConnection;39;1;38;2
WireConnection;500;0;320;1
WireConnection;500;1;320;3
WireConnection;337;0;39;0
WireConnection;337;1;500;0
WireConnection;19;0;15;0
WireConnection;19;1;20;0
WireConnection;21;0;19;0
WireConnection;40;0;337;0
WireConnection;340;0;47;0
WireConnection;342;0;46;0
WireConnection;342;1;343;0
WireConnection;339;0;342;0
WireConnection;339;1;47;0
WireConnection;341;0;46;0
WireConnection;341;1;340;0
WireConnection;45;1;341;0
WireConnection;50;1;339;0
WireConnection;51;0;45;0
WireConnection;51;1;50;0
WireConnection;362;0;51;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;354;0;355;0
WireConnection;354;1;24;0
WireConnection;619;0;618;0
WireConnection;619;1;620;0
WireConnection;353;0;354;0
WireConnection;353;1;619;0
WireConnection;356;0;24;0
WireConnection;356;1;41;0
WireConnection;494;0;493;0
WireConnection;28;1;353;0
WireConnection;29;1;356;0
WireConnection;30;0;28;3
WireConnection;30;1;29;3
WireConnection;496;0;495;0
WireConnection;496;1;5;0
WireConnection;301;0;87;0
WireConnection;301;1;302;0
WireConnection;497;0;495;0
WireConnection;497;1;3;0
WireConnection;420;0;496;0
WireConnection;420;1;30;0
WireConnection;420;2;440;0
WireConnection;92;0;90;0
WireConnection;92;1;91;0
WireConnection;88;0;301;0
WireConnection;88;1;89;0
WireConnection;425;0;420;0
WireConnection;425;1;497;0
WireConnection;426;0;425;0
WireConnection;344;0;88;0
WireConnection;344;1;92;0
WireConnection;105;0;104;0
WireConnection;94;1;344;0
WireConnection;444;0;10;4
WireConnection;444;1;426;0
WireConnection;144;0;105;0
WireConnection;99;0;94;2
WireConnection;438;0;444;0
WireConnection;479;0;144;0
WireConnection;609;0;606;0
WireConnection;609;1;608;0
WireConnection;42;0;438;0
WireConnection;151;0;480;0
WireConnection;488;0;477;4
WireConnection;607;0;609;0
WireConnection;604;0;46;0
WireConnection;604;1;605;0
WireConnection;134;0;117;0
WireConnection;134;1;119;0
WireConnection;487;0;151;0
WireConnection;487;1;488;0
WireConnection;602;0;604;0
WireConnection;602;1;609;0
WireConnection;603;0;46;0
WireConnection;603;1;607;0
WireConnection;149;0;134;0
WireConnection;149;1;487;0
WireConnection;65;0;63;0
WireConnection;65;1;603;0
WireConnection;361;0;360;0
WireConnection;361;1;269;0
WireConnection;501;0;266;1
WireConnection;501;1;266;2
WireConnection;64;0;602;0
WireConnection;64;1;63;0
WireConnection;133;0;149;0
WireConnection;67;1;65;0
WireConnection;359;0;361;0
WireConnection;359;1;501;0
WireConnection;66;1;64;0
WireConnection;121;0;133;0
WireConnection;372;0;359;0
WireConnection;600;0;66;1
WireConnection;600;1;66;3
WireConnection;600;2;599;0
WireConnection;598;0;67;1
WireConnection;598;1;67;3
WireConnection;598;2;599;0
WireConnection;456;0;377;0
WireConnection;456;1;271;0
WireConnection;508;2;106;0
WireConnection;74;0;600;0
WireConnection;74;1;598;0
WireConnection;260;1;359;0
WireConnection;126;0;372;0
WireConnection;509;0;456;0
WireConnection;509;1;508;0
WireConnection;478;0;477;0
WireConnection;478;1;60;0
WireConnection;478;2;482;0
WireConnection;68;0;600;0
WireConnection;68;1;598;0
WireConnection;75;0;598;0
WireConnection;75;1;74;0
WireConnection;374;0;367;0
WireConnection;374;1;478;0
WireConnection;374;2;377;0
WireConnection;265;0;260;0
WireConnection;627;0;509;0
WireConnection;212;0;201;0
WireConnection;212;1;213;0
WireConnection;76;0;75;0
WireConnection;297;0;374;0
WireConnection;297;1;298;0
WireConnection;297;2;627;0
WireConnection;70;0;69;0
WireConnection;70;1;68;0
WireConnection;219;0;94;2
WireConnection;219;1;220;0
WireConnection;601;0;70;0
WireConnection;601;1;76;0
WireConnection;601;2;599;0
WireConnection;199;0;200;0
WireConnection;199;2;211;0
WireConnection;199;3;212;0
WireConnection;350;0;297;0
WireConnection;111;0;110;0
WireConnection;111;1;112;0
WireConnection;102;0;10;0
WireConnection;244;0;601;0
WireConnection;475;0;103;0
WireConnection;475;1;476;0
WireConnection;624;0;71;0
WireConnection;624;1;601;0
WireConnection;231;0;219;0
WireConnection;231;1;232;0
WireConnection;203;0;202;4
WireConnection;203;1;199;0
WireConnection;141;0;351;0
WireConnection;141;1;111;0
WireConnection;61;0;141;0
WireConnection;61;1;475;0
WireConnection;61;2;62;0
WireConnection;230;0;231;0
WireConnection;230;1;229;0
WireConnection;505;0;203;0
WireConnection;73;0;624;0
WireConnection;401;0;230;0
WireConnection;625;0;61;0
WireConnection;625;1;79;0
WireConnection;206;0;202;0
WireConnection;205;0;505;0
WireConnection;312;0;83;0
WireConnection;312;1;384;0
WireConnection;207;0;625;0
WireConnection;207;1;210;0
WireConnection;207;2;208;0
WireConnection;221;0;401;0
WireConnection;95;0;96;0
WireConnection;95;1;94;2
WireConnection;223;0;207;0
WireConnection;223;1;315;0
WireConnection;223;2;224;0
WireConnection;82;0;51;0
WireConnection;82;1;363;0
WireConnection;82;2;312;0
WireConnection;114;0;223;0
WireConnection;98;0;97;0
WireConnection;98;1;95;0
WireConnection;460;0;458;0
WireConnection;621;0;622;0
WireConnection;621;1;82;0
WireConnection;621;2;128;0
WireConnection;498;0;489;0
WireConnection;498;1;2;0
WireConnection;470;0;41;0
WireConnection;499;0;498;0
WireConnection;52;0;621;0
WireConnection;233;0;30;0
WireConnection;459;0;460;0
WireConnection;459;1;115;0
WireConnection;490;1;501;0
WireConnection;100;0;98;0
WireConnection;581;0;344;0
WireConnection;514;0;339;0
WireConnection;514;1;515;0
WireConnection;457;0;458;0
WireConnection;457;1;115;0
WireConnection;222;0;233;0
WireConnection;491;0;490;1
WireConnection;491;1;492;0
WireConnection;626;0;498;0
WireConnection;218;0;95;0
WireConnection;0;0;459;0
WireConnection;0;1;53;0
WireConnection;0;2;457;0
WireConnection;0;4;56;0
WireConnection;0;9;626;0
WireConnection;0;11;101;0
WireConnection;0;14;234;0
ASEEND*/
//CHKSM=B05C3E9EF28BD5BE7EA95E5AF93C2AD8DB6CD90A