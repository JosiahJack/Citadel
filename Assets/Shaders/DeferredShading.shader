Shader "Custom/DeferredShading" {
	
	Properties {
	}

	SubShader {

		Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#pragma exclude_renderers nomrt

			#pragma multi_compile_lightpass
			#pragma multi_compile _ UNITY_HDR_ON

			#include "MyDeferredShading.cginc"

			ENDCG
		}

		Pass {
			Cull Off
			ZTest Always
			ZWrite Off

			Stencil {
				Ref [_StencilNonBackground]
				ReadMask [_StencilNonBackground]
				CompBack Equal
				CompFront Equal
			}

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#pragma exclude_renderers nomrt

			#include "UnityCG.cginc"

			sampler2D _LightBuffer;

			struct VertexData {
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct Interpolators {
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			Interpolators VertexProgram (VertexData v) {
				Interpolators i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.uv;
				return i;
			}

			half4 FragmentProgram (Interpolators i) : SV_Target {
				return -log2(tex2D(_LightBuffer, i.uv));
			}

			ENDCG
		}
	}
}

/*
Shader "Custom/DeferredShading" {
	
	Properties {
	}

	SubShader {

		Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#pragma exclude_renderers nomrt

			#pragma multi_compile_lightpass
			#pragma multi_compile _ UNITY_HDR_ON

			#include "MyDeferredShading.cginc"

			ENDCG
		}

		Pass {
			Cull Off
			ZTest Always
			ZWrite Off

			Stencil {
				Ref [_StencilNonBackground]
				ReadMask [_StencilNonBackground]
				CompBack Equal
				CompFront Equal
			}

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#pragma exclude_renderers nomrt

			#include "UnityCG.cginc"

			sampler2D _LightBuffer;

			struct VertexData {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators VertexProgram (VertexData v) {
				Interpolators i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.uv;
				return i;
			}

			float4 FragmentProgram (Interpolators i) : SV_Target {
				return -log2(tex2D(_LightBuffer, i.uv));
			}

			ENDCG
		}
	}
}*/