Shader "Custom/ImpostorShader" {
    Properties {
        _VolumeTex("Volumetric Texture", 3D) = "" {}
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler3D _VolumeTex;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldDir = normalize(mul((float3x3)unity_ObjectToWorld, v.normal)); // World space normal
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Convert world direction to UV for the 3D texture
                float3 uv = (i.worldDir + 1.0) * 0.5; // Map [-1,1] to [0,1]
                return tex3D(_VolumeTex, uv);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
