Shader "Custom/StandardTextureArray" {
    Properties {
        _MainTex("Albedo", 2DArray) = "" {}
        _SpecGlossMap("Specular", 2DArray) = "" {}
        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2DArray) = "" {}
        _EmissionColor("Color", Color) = (1,1,1)
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
            #pragma shader_feature _EMISSION
            #pragma multi_compile_prepassfinal
            #pragma vertex vertDeferred
            #pragma fragment fragDeferred
            #include "UnityStandardCore_TexArray.cginc"
            ENDCG
        }

        // Shadow Caster Pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            Cull Off // Enable two-sided shadow casting

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
                float4 uv : TEXCOORD0; // Pass UVs if needed for texture array
            };

            v2f vertShadow(appdata_base v) {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = float4(v.texcoord.xy, 0, 0); // Assuming index 0 for simplicity
                return o;
            }

            float4 fragShadow(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
