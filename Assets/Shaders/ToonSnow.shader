Shader "Custom/ToonSnow"
{
	Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "black" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0


        _Ramp("Toon Ramp", 2D) = "gray" {}
        _SnowRamp("Snow Toon Ramp (RGB)", 2D) = "gray" {}
        _SnowAngle("Angle of snow buildup", Vector) = (0,1,0)
        _SnowColor("Snow Base Color", Color) = (0.5,0.5,0.5,1)
        _TColor("Snow Top Color", Color) = (0.5,0.5,0.5,1)
        _GlossinessSnow ("Snow Smoothness", Range(0,1)) = 0.5
        _MetallicSnow ("Snow Metallic", Range(0,1)) = 0.0
        _RimColor("Snow Rim Color", Color) = (0.5,0.5,0.5,1)
        _RimPower("Snow Rim Power", Range(0,4)) = 3
        _SnowSize("Snow Amount", Range(-3,3)) = 1
        _Height("Snow Height", Range(0,0.025)) = 0.015
	}
    
	SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _NormalTex;
        fixed4 _Color;

        half _Glossiness;
        half _Metallic;

        half _GlossinessSnow;
        half _MetallicSnow;

        sampler2D _SnowRamp;
        float4 _SnowColor;
        float4 _TColor;
        float4 _SnowAngle;
        float4 _RimColor;

        float _SnowSize;
        float _Height;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv_Bump : TEXCOORD1;
            float3 worldPos;
            float3 viewDir;
            float3 lightDir;
        };

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // light direction for snow ramp
            o.lightDir = WorldSpaceLightDir(v.vertex);

            // snow direction convertion to worldspace
            float4 snowC = mul(_SnowAngle , unity_ObjectToWorld);

            // if dot product >= _SnowSize, we extrude it
            half result = step(_SnowSize, dot(v.normal, snowC.xyz));

            // scale vertices along normal
            v.vertex.xyz += v.normal * _Height * result;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // local position for snow color blend
            float3 localPos = (IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz);

            // light value for snow toon ramp
            half d = dot(o.Normal, IN.lightDir) * 0.5 + 0.5;

            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            half3 rampS = tex2D(_SnowRamp, float2(d, d)).rgb;
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

            // if dot product >= _SnowSize, we turn it into snow
            half result = step(_SnowSize - 0.4, dot(o.Normal, _SnowAngle.xyz));

            o.Albedo = c.rgb * _Color;
            // blend base snow with top snow based on position
            // outer lerp chooses current Albedo if result is 0 or new snow Albedo if result is 1
            o.Albedo = lerp(o.Albedo, lerp(_SnowColor * rampS, _TColor * rampS, saturate(localPos.y)), result);

            //o.Normal = lerp(UnpackNormal(tex2D(_NormalTex, IN.uv_Bump)), o.Normal, result);

            // add glow rimlight to snow
            o.Emission = _RimColor.rgb * pow(rim, _RimPower) * result;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = lerp(_Metallic, _MetallicSnow, result);
            o.Smoothness = lerp(_Glossiness, _GlossinessSnow, result);

            o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
