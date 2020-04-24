// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/BlinnPhong-DeferredShading" {
Properties {
    _LightTexture0 ("", any) = "" {}
    _LightTextureB0 ("", 2D) = "" {}
    _ShadowMapTexture ("", any) = "" {}
    _SrcBlend ("", Float) = 1
    _DstBlend ("", Float) = 1
}
SubShader {

// Pass 1: Lighting pass
//  LDR case - Lighting encoded into a subtractive ARGB8 buffer
//  HDR case - Lighting additively blended into floating point buffer
Pass {
    ZWrite Off
    Blend [_SrcBlend] [_DstBlend]

CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass
#pragma multi_compile ___ UNITY_HDR_ON

#pragma exclude_renderers nomrt

#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;
#define PI 3.14159265
#define PI8 25.1327412287

float4 BRDF (float3 diffColor, float3 specColor, float oneMinusReflectivity, float smoothness,
    float3 normal, float3 viewDir,
    UnityLight light)
{
    float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    float3 floatDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

    float Nov = dot(normal, viewDir);    // This abs allow to limit artifact
    float NoL = saturate(dot(normal, light.dir));
    float NoH = saturate(dot(normal, floatDir));
    float LoH = saturate(dot(light.dir, floatDir));
    float diffuseTerm = DisneyDiffuse(Nov, NoL, LoH, perceptualRoughness) * NoL; 
    float3 color = diffColor * diffuseTerm;
    float _SP = pow(8192, smoothness);
    float _SPP2 = _SP + 2;
    float d = _SPP2 / PI8 * pow(NoH, _SP);          //D
    float4 t = float4(1.04166666, 0.475, 0.01822916, 0.25);\
    t *= smoothness;\
    t += float4 (0,0,-0.015625, 0.75);\
    float a1 = t.w;\
    float a0 = t.x * min(t.y, exp2(-9.28 * Nov)) + t.z;\
    float3 f = saturate(float3(a0 + specColor * (a1 - a0)));         //F
    float k = 2 / sqrt(PI * _SPP2);
    float oneMK = 1 - k;
    float v = 1 / (NoL * oneMK + k) * (Nov * oneMK + k);        //V
    color += d * f * v * FresnelTerm (specColor, LoH);
    color *= light.color;
    return float4(color, 1);
}


float4 CalculateLight (unity_v2f_deferred i)
{
    float3 wpos;
    float2 uv;
    float atten, fadeDist;
    UnityLight light;
    UNITY_INITIALIZE_OUTPUT(UnityLight, light);
    UnityDeferredCalculateLightParams (i, wpos, uv, light.dir, atten, fadeDist);

    light.color = _LightColor.rgb * atten;

    // unpack Gbuffer
    float4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
    float4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
    float4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
    UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

    float3 eyeVec = normalize(wpos-_WorldSpaceCameraPos);
    float oneMinusReflectivity = 1 - SpecularStrength(data.specularColor.rgb);


    float4 res = BRDF (data.diffuseColor, data.specularColor, oneMinusReflectivity, data.smoothness, data.normalWorld, -eyeVec, light);

    return res;
}

#ifdef UNITY_HDR_ON
half4
#else
fixed4
#endif
frag (unity_v2f_deferred i) : SV_Target
{
    float4 c = CalculateLight(i);
    #ifdef UNITY_HDR_ON
    return c;
    #else
    return exp2(-c);
    #endif
}

ENDCG
}


// Pass 2: Final decode pass.
// Used only with HDR off, to decode the logarithmic buffer into the main RT
Pass {
    ZTest Always Cull Off ZWrite Off
    Stencil {
        ref [_StencilNonBackground]
        readmask [_StencilNonBackground]
        // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
        compback equal
        compfront equal
    }

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers nomrt

#include "UnityCG.cginc"

sampler2D _LightBuffer;
struct v2f {
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(vertex);
    o.texcoord = texcoord.xy;
#ifdef UNITY_SINGLE_PASS_STEREO
    o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
#endif
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    return -log2(tex2D(_LightBuffer, i.texcoord));
}
ENDCG
}

}
Fallback Off
}