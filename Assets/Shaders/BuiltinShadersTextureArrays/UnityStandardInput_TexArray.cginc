// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
#ifndef UNITY_STANDARD_INPUT_INCLUDED
#define UNITY_STANDARD_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityPBSLighting.cginc" // TBD: remove
#include "UnityStandardUtils.cginc"

UNITY_DECLARE_TEX2DARRAY(_MainTex);
float4      _MainTex_ST;

UNITY_DECLARE_TEX2DARRAY(_BumpMap);
UNITY_DECLARE_TEX2DARRAY(_SpecGlossMap);

half4       _EmissionColor;
UNITY_DECLARE_TEX2DARRAY(_EmissionMap);

struct VertexInput {
    float4 vertex   : POSITION;
    half3 normal    : NORMAL;
    fixed4 color      : COLOR;  // Add vertex color input
    float3 uv0      : TEXCOORD0;
    float2 uv1      : TEXCOORD1;
    half4 tangent   : TANGENT;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float4 TexCoords(VertexInput v) {
    float4 texcoord;
    texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
    UNITY_SETUP_INSTANCE_ID(v);
    texcoord.z = floor(v.color.r * 255);
    return texcoord;
}

half3 Albedo(float4 texcoords) {
	half3 tmp = UNITY_SAMPLE_TEX2DARRAY(_MainTex, texcoords.xyz).rgb;
    half3 albedo = tmp.rgb;
    return albedo;
}

half Alpha(float3 uv) {
    return UNITY_SAMPLE_TEX2DARRAY(_MainTex, uv).a;
}

half4 SpecularGloss(float3 uv) {
    return UNITY_SAMPLE_TEX2DARRAY(_SpecGlossMap, uv);
}

half3 Emission(float3 uv) {
    return UNITY_SAMPLE_TEX2DARRAY(_EmissionMap, uv).rgb * _EmissionColor.rgb;
}

half3 NormalInTangentSpace(float4 texcoords) {
    half3 normalTangent = UnpackScaleNormal(UNITY_SAMPLE_TEX2DARRAY(_BumpMap, texcoords.xyz),1);
    return normalTangent;
}

#endif // UNITY_STANDARD_INPUT_INCLUDED
