Shader "Custom/MCMC" {
	Properties {
		_MainTex ("Samples", 2D) = "black" {}
		_DistTex ("Distribution", 2D) = "black" {}
		_Blend ("Blend", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DistTex;
			float _Blend;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			vs2ps vert(appdata IN) {
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				float4 s = tex2D(_MainTex, IN.uv);
				float4 d = tex2D(_DistTex, IN.uv);
				return _Blend * s + (1 - _Blend) * d;
			}
			ENDCG
		}
	} 
	FallBack off
}
