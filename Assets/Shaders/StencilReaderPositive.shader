Shader "Custom/StencilReaderPositive"
{
	Properties
    {
        _StencilMask ("StencilMask", int) = 0
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
    
	SubShader
    {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		LOD 200

        Stencil
        {
            Ref[_StencilMask]
            Comp equal
            Pass keep
            Fail keep
        }

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv_BumpTex : TEXCOORD1;
        };

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        
		void surf (Input IN, inout SurfaceOutputStandard o)
        {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpTex));

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
