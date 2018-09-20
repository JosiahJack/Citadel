Shader "GUI/3D Text Shader" { 
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Font Texture",2D) = "white" {}
	}

	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Cull Back
		CGPROGRAM
		#pragma surface SS_Main Lambert vertex:VS_Main alpha:blend
		#pragma target 3.0
		
		uniform fixed4 _Color;
		uniform sampler2D _MainTex;
		
		struct Output {
			float4 vertex : POSITION;
			float3 normal: NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		};
		
		struct Input {
			float2 uv_MainTex;
		};
		
		void VS_Main (inout Output o) {
			o.normal = float3 (0, 0, -1);
		}
		
		void SS_Main (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = _Color.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}