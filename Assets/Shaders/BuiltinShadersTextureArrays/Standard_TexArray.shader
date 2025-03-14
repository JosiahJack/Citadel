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
    }


    //FallBack "VertexLit"
}
