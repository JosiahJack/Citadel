Shader "Roystan/Grass"
{
    Properties
    {
		[Header(Shading)]
        _TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		[Space]
		_TessellationUniform ("Tessellation Uniform", Range(1, 64)) = 1
		[Header(Blades)]
		_BladeWidth("Blade Width", Float) = 0.05
		_BladeWidthRandom("Blade Width Random", Float) = 0.02
		_BladeHeight("Blade Height", Float) = 0.5
		_BladeHeightRandom("Blade Height Random", Float) = 0.3
		_BladeForward("Blade Forward Amount", Float) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
		_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
		[Header(Wind)]
		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindStrength("Wind Strength", Float) = 1
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
    }

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "CustomTessellation.cginc"
	
	struct geometryOutput
	{
		float4 pos : SV_POSITION;
	#if UNITY_PASS_FORWARDBASE		
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		// unityShadowCoord4 is defined as a float4 in UnityShadowLibrary.cginc.
		float3 worldPos : TEXCOORD1;
		unityShadowCoord4 _ShadowCoord : TEXCOORD2;
	#endif
	};

	// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
	// Extended discussion on this function can be found at the following link:
	// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
	// Returns a number in the 0...1 range.
	float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	// Construct a rotation matrix that rotates around the provided axis, sourced from:
	// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
	float3x3 AngleAxis3x3(float angle, float3 axis)
	{
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

	geometryOutput VertexOutput(float3 pos, float3 normal, float2 uv)
	{
		geometryOutput o;

		o.pos = UnityObjectToClipPos(pos);

	#if UNITY_PASS_FORWARDBASE
		o.normal = UnityObjectToWorldNormal(normal);
		o.uv = uv;
		o.worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;
		// Shadows are sampled from a screen-space shadow map texture.
		o._ShadowCoord = ComputeScreenPos(o.pos);
	#elif UNITY_PASS_SHADOWCASTER
		// Applying the bias prevents artifacts from appearing on the surface.
		o.pos = UnityApplyLinearShadowBias(o.pos);
	#endif

		return o;
	}

	float2 hash(float2 p) {
		p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
		return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
	}

	float perlinNoise(float2 p) {
		float2 i = floor(p);
		float2 f = frac(p);
		
		float2 u = f * f * (3.0 - 2.0 * f); // Smoothstep
		
		float2 a = hash(i + float2(0.0, 0.0));
		float2 b = hash(i + float2(1.0, 0.0));
		float2 c = hash(i + float2(0.0, 1.0));
		float2 d = hash(i + float2(1.0, 1.0));
		
		return lerp(lerp(dot(a, f - float2(0,0)), dot(b, f - float2(1,0)), u.x),
					lerp(dot(c, f - float2(0,1)), dot(d, f - float2(1,1)), u.x), u.y);
	}

	geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix) {
		float3 tangentPoint = float3(width, forward, height);

		float3 tangentNormal = normalize(float3(0, -1, forward));

		float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
		float3 localNormal = mul(transformMatrix, tangentNormal);
		return VertexOutput(localPosition, localNormal, uv);
	}

	float _BladeHeight;
	float _BladeHeightRandom;

	float _BladeWidthRandom;
	float _BladeWidth;

	float _BladeForward;
	float _BladeCurve;

	float _BendRotationRandom;

	sampler2D _WindDistortionMap;
	float4 _WindDistortionMap_ST;

	float _WindStrength;
	float2 _WindFrequency;

	#define BLADE_SEGMENTS 3

	// Geometry program that takes in a single triangle and outputs a blade
	// of grass at that triangle first vertex position, aligned to the vertex's normal.
	[maxvertexcount(BLADE_SEGMENTS * 2 + 4)] // Increased to handle explicit quad for tip
	void geo(triangle vertexOutput IN[3], inout TriangleStream<geometryOutput> triStream)
	{
		float3 pos = IN[0].vertex.xyz;
		float3 worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;

		// Use Perlin noise based on world XZ coordinates
		float2 noiseInput = worldPos.xz * 0.1;
		float noiseValue = perlinNoise(noiseInput) * 0.5 + 0.5;
		float noiseValue2 = perlinNoise(noiseInput + float2(10.0, 20.0)) * 0.5 + 0.5;
		float noiseValue3 = perlinNoise(noiseInput + float2(-5.0, 15.0)) * 0.5 + 0.5;

		// Construct transformation matrices
		float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
		float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
		float3 wind = normalize(float3(windSample.x, windSample.y, 0));
		float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

		float3 vNormal = IN[0].normal;
		float4 vTangent = IN[0].tangent;
		float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

		float3x3 tangentToLocal = float3x3(
			vTangent.x, vBinormal.x, vNormal.x,
			vTangent.y, vBinormal.y, vNormal.y,
			vTangent.z, vBinormal.z, vNormal.z
		);

		float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);
		float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);

		float height = (noiseValue * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (noiseValue2 * 2 - 1) * _BladeWidthRandom + _BladeWidth;
		float forward = noiseValue3 * _BladeForward;

		// Generate segments up to the second-to-last one
		for (int i = 0; i < BLADE_SEGMENTS - 1; i++)
		{
			float t = i / (float)(BLADE_SEGMENTS - 1);
			float segmentHeight = height * t;
			float segmentWidth = width * (1 - t * 0.5); // Taper less aggressively until the tip
			float segmentForward = pow(t, _BladeCurve) * forward;

			float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

			triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
			triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
		}

		// Define the final segment explicitly as a quad with a squared, narrower tip
		float baseT = (BLADE_SEGMENTS - 1) / (float)(BLADE_SEGMENTS - 1); // t = 1 for the base of the tip
		float baseHeight = height * baseT * 0.9; // Slightly below full height to leave room for tip
		float baseWidth = width * (1 - baseT * 0.5); // Width at the base of the final segment
		float baseForward = pow(baseT, _BladeCurve) * forward;

		float tipHeight = height; // Full height for the tip
		float tipWidth = width * 0.3; // Narrower tip (adjustable, e.g., 0.3 = 30% of base width)
		float tipForward = pow(1.0, _BladeCurve) * forward;

		// Bottom of the final segment (quad base)
		triStream.Append(GenerateGrassVertex(pos, baseWidth, baseHeight, baseForward, float2(0, 0.9), transformationMatrix));
		triStream.Append(GenerateGrassVertex(pos, -baseWidth, baseHeight, baseForward, float2(1, 0.9), transformationMatrix));

		// Top of the final segment (squared tip)
		triStream.Append(GenerateGrassVertex(pos, tipWidth, tipHeight, tipForward, float2(0, 1.0), transformationMatrix));
		triStream.Append(GenerateGrassVertex(pos, -tipWidth, tipHeight, tipForward, float2(1, 1.0), transformationMatrix));

		// Restart strip to ensure a separate quad
		triStream.RestartStrip();
	}
	ENDCG

    SubShader {
        Pass
        {
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
			#pragma geometry geo
            #pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_fwdbase
            
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;

			// Calculate point light contribution
            float3 CalculatePointLight(float3 normal, float3 worldPos)
            {
                float3 totalLight = 0;
                
                // Unity supports up to 4 point lights in forward rendering
                for (int i = 0; i < 4; i++)
                {
                    // Skip if light is not active
                    if (unity_LightColor[i].a == 0)
                        continue;

                    float3 lightPos = float3(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i]);
                    float3 lightVec = lightPos - worldPos;
                    float distance = length(lightVec);
                    float3 lightDir = lightVec / distance;

                    // Attenuation based on light range
                    float range = unity_4LightAtten0[i];
                    float attenuation = 1.0 / (1.0 + range * distance * distance);
                    
                    // Lambertian lighting with translucency
                    float NdotL = saturate(dot(normal, lightDir) + _TranslucentGain);
                    totalLight += unity_LightColor[i].rgb * NdotL * attenuation;
                }
                
                return totalLight;
            }

			float4 frag (geometryOutput i,  fixed facing : VFACE) : SV_Target
            {			
				float3 normal = facing > 0 ? i.normal : -i.normal;

				float shadow = SHADOW_ATTENUATION(i);
				float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				float4 lightIntensity = NdotL * _LightColor0;// + float4(ambient, 1);

                float3 pointLights = CalculatePointLight(normal, i.worldPos);
                float3 finalLight = lightIntensity + pointLights;// + ambient;
				float4 baseColor = lerp(_BottomColor, _TopColor, i.uv.y);
				float4 col = baseColor * float4(finalLight, 1);
				return col;
            }
            ENDCG
        }

		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_shadowcaster

			float4 frag(geometryOutput i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
    }
}
