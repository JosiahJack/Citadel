Shader "Custom/StandardTextureArray" {
    Properties {
        _MainTex("Albedo", 2DArray) = "" {}
        _SpecGlossMap("Specular", 2DArray) = "" {}
        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2DArray) = "" {}
        _EmissionMap("Emission", 2DArray) = "" {}

        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }
            ZWrite On
            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt
            #pragma multi_compile_prepassfinal
            #pragma vertex vertDeferred
            #pragma fragment fragDeferred
            #include "UnityStandardCore_TexArray.cginc"
            ENDCG
        }

        /*
        // Second pass: Custom lighting from compute buffer
        Pass {
            Name "CUSTOM_LIGHTING"
            Tags { "LightMode" = "ForwardBase" } // You can use a custom tag here if needed
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vertDeferred
            #pragma fragment fragDeferred

            // G-buffer samplers
            sampler2D _GBuffer0; // Albedo
            sampler2D _GBuffer1; // Normal
            sampler2D _GBuffer2; // Specular
            sampler2D _GBuffer3; // Depth or Emission

            float3 _CameraWorldPos; // Set this from C#

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1; // Pass world position
            };

			v2f vertDeferred(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Compute world position (transforming from object space to world space)
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            struct LightData {
				float4 color;
				float3 position;
				float intensity;
				float range;
				float spotAngle;
				float3 direction;
			};

            // Custom light buffer (set from C# script)
            StructuredBuffer<LightData> Lights;
            int lightsCount; // Number of lights

            // Fragment shader for applying custom lighting
            float4 fragDeferred(v2f i) : SV_Target {
                float3 worldPos = i.worldPos; // Assume worldPos comes from vertex-to-fragment input
                return float4(worldPos,1);

                float3 viewDir = normalize(_CameraWorldPos - worldPos);

                // Sample albedo and normal from the G-buffer
                float4 albedoTex = tex2D(_GBuffer0, i.uv);
                float3 albedo = albedoTex.rgb;

                float4 normalTex = tex2D(_GBuffer1, i.uv);
                float3 normal = normalize(normalTex.rgb * 2.0 - 1.0); // Decode normal

                float3 result = 0;

                // Apply lighting from custom light buffer
                for (int i = 0; i < lightsCount; i++) {
                    LightData lit = Lights[i];
					float3 lightdir = lit.position - worldPos;
					float distance = length(lightdir);

					if (distance > lit.range) continue;

					float invRange = 1 / lit.range;
					float attenuation = saturate(1.0 - (distance * invRange));
					attenuation = pow(attenuation, 3.2);

					lightdir = normalize(lightdir);
					if (lit.spotAngle > 0) { // Spot light
						float3 spotdir = normalize(lit.direction);
						float dotSpot = dot(lightdir,-spotdir);
						float cosAng = cos(lit.spotAngle * 0.017453293); // pi/180, half angle
						if (dotSpot < cosAng) continue;

						attenuation *= pow(saturate((dotSpot - cosAng) / (1.0 - cosAng)), 4.0);
					}

					float lambertian = max(dot(normal, lightdir), 0.0);
					//float lambertian = max(dot(viewDir, lightdir), 0.0);
					result += lit.color * lit.intensity * attenuation * lambertian;
                }

                result += albedo;
                return float4(result, 1.0);
            }

            ENDCG
        }
        */
    }


    FallBack "VertexLit"
}
