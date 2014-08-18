Shader "Custom/UnlitColor" {
	Properties {
		_Color ("Color", Color) = (1, 0, 0, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200 ZTest Always ZWrite Off Fog { Mode Off }
		Blend SrcAlpha One
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			
			float4 vert(float4 vertex : POSITION) : POSITION {
				return mul(UNITY_MATRIX_MVP, vertex);
			}
			float4 frag() : COLOR {
				return _Color;
			}
			ENDCG
		}
	} 
	FallBack off
}
