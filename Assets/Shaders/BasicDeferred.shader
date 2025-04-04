//Written by Przemyslaw Zaworski
//https://github.com/przemyslawzaworski

Shader "Deferred (Metallic Gloss)" 
{
	Properties 
	{
        _MainTex ("Diffuse Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Metallic ("Metallic", Range(0, 1)) = 1
		_Gloss ("Gloss", Range(0, 1)) = 0.8
	}
	SubShader 
	{
		Pass 
		{
			Tags {"LightMode"="Deferred"}
         
			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma exclude_renderers nomrt
			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma target 3.0
			
			#include "UnityPBSLighting.cginc"

			sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			float _Metallic;
			float _Gloss;

			struct structureVS 
			{
				float4 screen_vertex : SV_POSITION;
				float4 world_vertex : TEXCOORD0;
				float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD2;
			};
			
			struct structurePS
			{
				half4 albedo : SV_Target0;
				half4 specular : SV_Target1;
				half4 normal : SV_Target2;
				half4 emission : SV_Target3;
			};
			
			structureVS vertex_shader (float4 vertex : POSITION,float3 normal : NORMAL, float2 uv : TEXCOORD2) 
			{
				structureVS vs;
				vs.screen_vertex = UnityObjectToClipPos( vertex );
				vs.world_vertex = mul(unity_ObjectToWorld, vertex);				
				vs.normal = UnityObjectToWorldNormal(normal);
                vs.uv = TRANSFORM_TEX(uv, _MainTex);
				return vs;
			}
			
			structurePS pixel_shader (structureVS vs)
			{
				structurePS ps;
				float3 normalDirection = normalize(vs.normal);
				half3 specular;
				half specularMonochrome;
                half4 texColor = tex2D(_MainTex, vs.uv) * _Color;
				half3 diffuseColor = DiffuseAndSpecularFromMetallic(texColor.rgb, _Metallic, specular, specularMonochrome );
				ps.albedo = half4( diffuseColor, 1.0 );
				ps.specular = half4( specular, _Gloss );
				ps.normal = half4( normalDirection * 0.5 + 0.5, 1.0 );
				ps.emission = half4(0,0,0,1);
				#ifndef UNITY_HDR_ON
					ps.emission.rgb = exp2(-ps.emission.rgb);
				#endif
				return ps;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
