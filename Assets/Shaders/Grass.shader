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

    struct vertexInput {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
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
        float4 tangent : TANGENT;
    };

    struct tessellationVert {
        float4 pos : SV_POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
    };

    tessellationVert tessVert(vertexInput v) {
        tessellationVert o;
        o.pos = v.vertex;
        o.normal = v.normal;
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

    geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float3x3 transformMatrix, float4 tangent) {
        geometryOutput o;
        float3 tangentPoint = float3(width, forward, height);
        float3 tangentNormal = -normalize(float3(0, -1, forward));
        float3 localNormal = mul(transformMatrix, tangentNormal);
        float3 localOffset = mul(transformMatrix, tangentPoint);
        float3 localPosition = vertexPosition + localOffset;
        o.pos = UnityObjectToClipPos(localPosition);
        o.normal = UnityObjectToWorldNormal(localNormal);
        o.tangent = float4(UnityObjectToWorldDir(tangent.xyz), tangent.w);
        return o;
    }

    #define BLADE_SEGMENTS 3
    [maxvertexcount(BLADE_SEGMENTS * 2 + 4)]
    void geo(triangle tessellationVert IN[3], inout TriangleStream<geometryOutput> triStream) {
        float3 pos = IN[0].pos.xyz;
//         float3 worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;

        // Each blade of grass is constructed in tangent space with respect
		// to the emitting vertex's normal and tangent vectors, where the width
		// lies along the X axis and the height along Z.

		// Construct random rotations to point the blade in a direction.
        float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));

		// Matrix to bend the blade in the direction it's facing.
        float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
        float2 windSample = float2(0.001,0.001);
        float3 wind = normalize(float3(windSample.x, windSample.y, 0));
        float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

		// Construct a matrix to transform our blade from tangent space
		// to local space; this is the same process used when sampling normal maps.
        float3 vNormal = IN[0].normal;//float3(0, 1, 0);
        float4 vTangent = IN[0].tangent;
        float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;
        float3x3 tangentToLocal = float3x3(
            vTangent.x, vBinormal.x, vNormal.x,
            vTangent.y, vBinormal.y, vNormal.y,
            vTangent.z, vBinormal.z, vNormal.z
        );

        // Construct full tangent to local matrix, including our rotations.
		// Construct a second matrix with only the facing rotation; this will be used 
		// for the root of the blade, to ensure it always faces the correct direction.
        float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);
        float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);

        float height = abs(rand(pos) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
        float width = abs(rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
        float forward = rand(pos.yyz) * _BladeForward;

        for (int i = 0; i < BLADE_SEGMENTS - 1; i++) {
            float t = i / (float)(BLADE_SEGMENTS - 1);

            float segmentHeight = height * t;
            float segmentWidth = width * (1 - t * 0.5);
            float segmentForward = pow(t, _BladeCurve) * forward;

			// Select the facing-only transformation matrix for the root of the blade.
            float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

            triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, transformMatrix, vTangent));
            triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, transformMatrix, vTangent));
        }

		// Add the final 4 vertices as the squared tip (cut grass!).
        float baseT = (BLADE_SEGMENTS - 1) / (float)(BLADE_SEGMENTS - 1);
        float baseHeight = height * baseT * 0.9;
        float baseWidth = width * (1 - baseT * 0.5);
        float baseForward = pow(baseT, _BladeCurve) * forward;

        float tipHeight = height;
        float tipWidth = width * 0.3;
        float tipForward = pow(1.0, _BladeCurve) * forward * 2;

        triStream.Append(GenerateGrassVertex(pos, baseWidth, baseHeight, baseForward, transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, -baseWidth, baseHeight, baseForward, transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, tipWidth, tipHeight, tipForward, transformationMatrix, vTangent));
        triStream.Append(GenerateGrassVertex(pos, -tipWidth, tipHeight, tipForward, transformationMatrix, vTangent));
        triStream.RestartStrip();
    }
    ENDCG

    SubShader {
        Pass {
            Tags {"LightMode"="Deferred"}
            CGPROGRAM
            #pragma vertex vert
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
            
            structurePS pixel_shader(geometryOutput vs) {
                structurePS ps;
                float3 normalDirection = normalize(vs.normal);
                half3 diffuseColor = _Color;
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
            #pragma vertex vert
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

            float4 shadow_fragment(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i) // Outputs depth to shadow map
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
