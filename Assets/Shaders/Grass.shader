Shader "Deferred/Grass" {
    Properties {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _TessellationUniform ("Tessellation Uniform", Range(1, 64)) = 1
        _BladeWidth("Blade Width", Float) = 0.05
        _BladeWidthRandom("Blade Width Random", Float) = 0.02
        _BladeHeight("Blade Height", Float) = 0.5
        _BladeHeightRandom("Blade Height Random", Float) = 0.3
        _BladeForward("Blade Forward Amount", Float) = 0.38
        _BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
        _BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
        _WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityPBSLighting.cginc"
    float _BladeHeight;
    float _BladeHeightRandom;
    float _BladeWidthRandom;
    float _BladeWidth;
    float _BladeForward;
    float _BladeCurve;
    float _BendRotationRandom;
    float2 _WindFrequency;

    struct vertexInput {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        float2 uv : TEXCOORD0;
    };

    vertexInput vert(vertexInput v) {
        return v;
    }

    struct TessellationFactors {
        float edge[3] : SV_TessFactor;
        float inside : SV_InsideTessFactor;
    };

    float _TessellationUniform;
    TessellationFactors patchConstantFunction (InputPatch<vertexInput, 3> patch) {
        TessellationFactors f;
        f.edge[0] = _TessellationUniform;
        f.edge[1] = _TessellationUniform;
        f.edge[2] = _TessellationUniform;
        f.inside = _TessellationUniform;
        return f;
    }

    [UNITY_domain("tri")]
    [UNITY_outputcontrolpoints(3)]
    [UNITY_outputtopology("triangle_cw")]
    [UNITY_partitioning("integer")]
    [UNITY_patchconstantfunc("patchConstantFunction")]
    vertexInput hull (InputPatch<vertexInput, 3> patch, uint id : SV_OutputControlPointID) {
        return patch[id];
    }

    struct geometryOutput {
        float4 pos : SV_POSITION;
        float3 normal : TEXCOORD1;
        float2 uv : TEXCOORD2;
        float4 tangent : TANGENT;
    };

    struct tessellationVert {
        float4 pos : SV_POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        float2 uv : TEXCOORD2;
    };

    tessellationVert tessVert(vertexInput v) {
        tessellationVert o;
        o.pos = v.vertex;
        o.normal = v.normal;
        o.uv = float2(0,0);//TRANSFORM_TEX(v.uv, _WindDistortionMap);
        o.tangent = v.tangent;
        return o;
    }

    [UNITY_domain("tri")]
    tessellationVert domain(TessellationFactors factors, OutputPatch<vertexInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation) {
        vertexInput v;
        v.vertex = patch[0].vertex * barycentricCoordinates.x + 
                patch[1].vertex * barycentricCoordinates.y + 
                patch[2].vertex * barycentricCoordinates.z;
        v.normal = float3(0,-1,0);
        v.tangent = patch[0].tangent * barycentricCoordinates.x + 
                    patch[1].tangent * barycentricCoordinates.y + 
                    patch[2].tangent * barycentricCoordinates.z;
        v.uv = patch[0].uv * barycentricCoordinates.x + 
            patch[1].uv * barycentricCoordinates.y + 
            patch[2].uv * barycentricCoordinates.z;
        return tessVert(v);
    }

    float rand(float3 co) {
        return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
    }

    float3x3 AngleAxis3x3(float angle, float3 axis) {
        float c, s;
        sincos(angle, s, c);
        float t = 1 - c;
        float x = axis.x;
        float y = axis.y;
        float z = axis.z;
        return float3x3(
            t * x * x + c, t * x * y - s * z, t * x * z + s * y,
            t * x * y + s * z, t * y * y + c, t * y * z - s * x,
            t * x * z - s * y, t * y * z + s * x, t * z * z + c
        );
    }

    geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix, float4 tangent) {
        geometryOutput o;
        float3 tangentPoint = float3(width, forward, height);

        float3 tangentNormal = -normalize(float3(0, -1, forward));

        float3 localNormal = mul(transformMatrix, tangentNormal);
        float3 localOffset = mul(transformMatrix, tangentPoint);
        float3 localPosition = vertexPosition + localOffset;
        
        o.pos = UnityObjectToClipPos(localPosition);

        o.normal = UnityObjectToWorldNormal(localNormal);
        o.uv = uv;


        o.tangent = float4(UnityObjectToWorldDir(tangent.xyz), tangent.w);
        return o;
    }

    #define BLADE_SEGMENTS 3
    [maxvertexcount(BLADE_SEGMENTS * 2 + 4)]
    void geo(triangle tessellationVert IN[3], inout TriangleStream<geometryOutput> triStream) {
        float3 pos = IN[0].pos.xyz;
        float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
        float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
        float2 uv = IN[0].uv;
        float2 windSample = float2(0.001,0.001);
        float3 wind = normalize(float3(windSample.x, windSample.y, 0));
        float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);
        float3 vNormal = float3(0, -1, 0);
        float4 vTangent = IN[0].tangent;
        float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;
        float3x3 tangentToWorld = float3x3(
            vTangent.x, vBinormal.x, vNormal.x,
            vTangent.y, vBinormal.y, 1,
            vTangent.z, vBinormal.z, vNormal.z
        );

        float3x3 transformationMatrix = mul(mul(mul(tangentToWorld, windRotation), facingRotationMatrix), bendRotationMatrix);
        float3x3 transformationMatrixFacing = mul(tangentToWorld, facingRotationMatrix);
        float3 worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;
        float height = (rand(worldPos) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
        float width = (rand(worldPos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
        float forward = rand(worldPos.yyz) * _BladeForward;

        for (int i = 0; i < BLADE_SEGMENTS - 1; i++) {
            float t = i / (float)(BLADE_SEGMENTS - 1);
            float segmentHeight = height * t;
            float segmentWidth = width * (1 - t * 0.5);
            float segmentForward = pow(t, _BladeCurve) * forward;

            float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;
            triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix, vTangent));
            triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix, vTangent));
        }

        float baseT = (BLADE_SEGMENTS - 1) / (float)(BLADE_SEGMENTS - 1);
        float baseHeight = height * baseT * 0.9;
        float baseWidth = width * (1 - baseT * 0.5);
        float baseForward = pow(baseT, _BladeCurve) * forward;

        float tipHeight = height;
        float tipWidth = width * 0.3;
        float tipForward = pow(1.0, _BladeCurve) * forward * 2;

        triStream.Append(GenerateGrassVertex(pos, baseWidth, baseHeight, baseForward, float2(0, 0.9), transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, -baseWidth, baseHeight, baseForward, float2(1, 0.9), transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, tipWidth, tipHeight, tipForward, float2(0, 1.0), transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, -tipWidth, tipHeight, tipForward, float2(1, 1.0), transformationMatrix, vTangent));
        triStream.RestartStrip();
    }
    ENDCG

    SubShader {
        Pass {
            Tags {"LightMode"="Deferred"}
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment pixel_shader
            #pragma geometry geo
            #pragma hull hull
            #pragma domain domain
            #pragma target 4.6
            #pragma exclude_renderers nomrt
            #pragma multi_compile ___ UNITY_HDR_ON
            #include "UnityPBSLighting.cginc"

            float4 _Color;

            struct structurePS {
                half4 albedo : SV_Target0;
                half4 specular : SV_Target1;
                half4 normal : SV_Target2;
                half4 emission : SV_Target3;
            };
            
            geometryOutput vertex_shader(float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0) {
                geometryOutput vs;
                vs.pos = vertex;
                vs.normal = normal;
                vs.uv = float2(0,0);//TRANSFORM_TEX(uv, _WindDistortionMap);
                vs.tangent = tangent;
                return vs;
            }
            
            structurePS pixel_shader(geometryOutput vs) {
                structurePS ps;
                float3 normalDirection = normalize(vs.normal);
                half3 specular = 0;
                half specularMonochrome;
                half3 diffuseColor = DiffuseAndSpecularFromMetallic(_Color, 0, specular, specularMonochrome);
                ps.albedo = half4(diffuseColor, 1.0);
                ps.specular = half4(0,0,0,0);
                ps.normal = half4(normalDirection * 0.5 + 0.5, 1.0);
                ps.emission = half4(0, 0, 0, 1);
                #ifndef UNITY_HDR_ON
                    ps.emission.rgb = exp2(-ps.emission.rgb);
                #endif
                return ps;
            }
            ENDCG
        }

        // Shadow Caster Pass
        Pass {
            Tags {"LightMode" = "ShadowCaster"}
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment shadow_fragment
            #pragma geometry geo
            #pragma hull hull
            #pragma domain domain
            #pragma target 4.6
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"
            #include "AutoLight.cginc" // For shadow support

            struct v2f {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD2; // Pass UVs if needed for texture array
            };

            geometryOutput vertex_shader(float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0) {
                geometryOutput vs;
                vs.pos = vertex;
                vs.normal = UnityObjectToWorldNormal(normal);
                vs.uv = float2(0,0);//TRANSFORM_TEX(uv, _WindDistortionMap);
                vs.tangent = float4(UnityObjectToWorldDir(tangent.xyz), tangent.w);
                return vs;
            }

            float4 shadow_fragment(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i) // Outputs depth to shadow map
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
