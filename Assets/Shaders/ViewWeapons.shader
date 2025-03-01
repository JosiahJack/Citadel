// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ViewWeapons" {
	Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
        _DepthOffset ("Depth Offset",Range(-0.5,0)) = -0.2 // Addjustable depth offset to render in front
		_UnlitFac ("Unlit Factor",Range(0,1)) = 0.1
	}

	SubShader {
		Tags {"RenderType"="Opaque"}
		ZWrite On ZTest LEqual
		Lighting On
        Cull Back

        // Base pass (ambient + first light)
        Pass {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #pragma multi_compile_fwdbase // Support multiple lights
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _DepthOffset;
            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            Interpolators MyVertexProgram (VertexData v) {
                Interpolators i;
                i.position = UnityObjectToClipPos(v.position);
				i.position.z -= _DepthOffset;
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.worldNormal = UnityObjectToWorldNormal(v.normal);
                i.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
                return i;
            }
            float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
                float4 color = tex2D(_MainTex, i.uv) * _Tint;
                float3 normal = normalize(i.worldNormal);
                float3 worldPos = i.worldPos;

                // Ambient lighting
                float3 lighting = UNITY_LIGHTMODEL_AMBIENT.rgb;

                // Handle main light (if directional, else zero)
                float3 lightDir;
                float3 lightColor = _LightColor0.rgb;
                if (_WorldSpaceLightPos0.w == 0) { // Directional
                    lightDir = normalize(_WorldSpaceLightPos0.xyz);
                } else { // Point light as main light
                    lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
                }
                float diff = max(0, dot(normal, lightDir));
                lighting += diff * lightColor;

                color.rgb *= lighting;
                return color;
            }
            ENDCG
        }
// 		Pass {
// 			CGPROGRAM
// 			#pragma vertex MyVertexProgram
// 			#pragma fragment MyFragmentProgram
// 			#include "UnityCG.cginc"
// 
// 			float4 _Tint;
// 			sampler2D _MainTex;
// 			float4 _MainTex_ST;
// 
// 			struct VertexData {
// 				float4 position : POSITION;
// 				float2 uv : TEXCOORD0;
// 			};
// 
// 			struct Interpolators {
// 				float4 position : SV_POSITION;
// 				float2 uv : TEXCOORD0;
// 			};
// 
// 			Interpolators MyVertexProgram (VertexData v) {
// 				Interpolators i;
// 				i.position = UnityObjectToClipPos(v.position);
// 				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
// 				return i;
// 			}
// 
// 			float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
// 				return tex2D(_MainTex, i.uv) * _Tint;
// 			}
// 
// 			ENDCG
// 		}

        // Additional pass for point lights
        Pass {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One // Additive blending for extra lights
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _DepthOffset;
			float _UnlitFac;
            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            Interpolators MyVertexProgram (VertexData v) {
                Interpolators i;
                i.position = UnityObjectToClipPos(v.position);
				i.position.z -= _DepthOffset;
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.worldNormal = UnityObjectToWorldNormal(v.normal);
                i.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
                return i;
            }
            float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				float4 albedo = tex2D(_MainTex, i.uv) * _Tint;
                float4 color = albedo;
                float3 normal = normalize(i.worldNormal);
                float3 worldPos = i.worldPos;

                // Point light calculation
                float3 lightDir = _WorldSpaceLightPos0.xyz - worldPos; // Position - world pos
                float distance = length(lightDir);
                lightDir = normalize(lightDir);
                float3 lightColor = _LightColor0.rgb;
                float attenuation = 1.0 / (1.0 + distance * distance); // Simple falloff
                float diff = max(0, dot(normal, lightDir));
                color.rgb *= diff * lightColor * attenuation;
				color.rgb += albedo * _UnlitFac;
                return color;
            }
            ENDCG
        }
	}
}
