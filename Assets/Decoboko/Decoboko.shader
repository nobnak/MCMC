Shader "Custom/Decoboko" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Size ("Noise Size", Vector) = (1, 2, 4, 8)
		_Speed ("Noise Speed", Vector) = (1, 1, 1, 1)
		_Gain ("Noise Gain", Vector) = (0.25, 0.25, 0.25, 0.25)
		_Pow ("Noise Power", Vector) = (1, 1, 1, 1)
		_Rand ("Random", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		ZTest Always ZWrite Off Cull Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/Shader/Libs/Noise.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			float4 _Size;
			float4 _Speed;
			float4 _Gain;
			float4 _Pow;
			float _Rand;
			
			struct vsin {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float3 pos0 : TEXCOORD0;
				float3 pos1 : TEXCOORD1;
				float3 pos2 : TEXCOORD2;
				float3 pos3 : TEXCOORD3;
			};
			
			vs2ps vert(vsin IN) {
				#if UNITY_UV_STARTS_AT_TOP
				IN.uv.y = 1 - IN.uv.y;
				#endif
				IN.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				float3 pos = float3(IN.uv * float2(_MainTex_TexelSize.z / _MainTex_TexelSize.w, 1), _Time.y);
				
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.pos0 = float3(_Size.x, _Size.x, _Speed.x) * (pos + _Rand * float3( 11,  17,  29));
				OUT.pos1 = float3(_Size.y, _Size.y, _Speed.y) * (pos + _Rand * float3( 53,  61,  79));
				OUT.pos2 = float3(_Size.z, _Size.z, _Speed.z) * (pos + _Rand * float3(109, 113, 139));
				OUT.pos3 = float3(_Size.w, _Size.w, _Speed.w) * (pos + _Rand * float3(173, 181, 223));
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				float4 c = saturate(0.5 + 0.5 * float4(snoise(IN.pos0), snoise(IN.pos1), snoise(IN.pos2), snoise(IN.pos3)));
				c = pow(c, _Pow);
				return dot(_Gain, c);
			}
			ENDCG
		}
	} 
	FallBack off
}
