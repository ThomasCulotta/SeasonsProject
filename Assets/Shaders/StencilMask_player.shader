// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/StencilMask_Player"
{
	Properties
	{
        _StencilMask("Stencil Mask", int) = 0
        _DebugColor("Debug Color", Color) = (1, 1, 1, 1)
	}
    
	SubShader
	{
		Tags
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }

        Pass
        {
            ColorMask 0
            ZWrite off
            
            Stencil 
            {
                Ref[_StencilMask]
                Comp greater
                Pass replace
                Fail keep
            }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag       
            
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

            fixed4 _DebugColor;
   
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _DebugColor;
			}
			ENDCG
		}
	}
}
