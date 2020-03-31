Shader "Custom/LODShaderDiffuseMetallic" {
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }
        LOD 200

        CGPROGRAM

        //#pragma multi_compile _ LOD_FADE_CROSSFADE

        #pragma surface surf Lambert noforwardadd
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionMap;
        fixed4 _EmissionColor;

        struct Input
        {
            float4 screenPos;
            float2 uv_MainTex;
        };

        //half _Glossiness;
        //half _Metallic;
        //fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
            //#ifdef LOD_FADE_CROSSFADE
            //float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
            //UnityApplyDitherCrossFade(vpos);
            //#endif

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);// * _Color;
            o.Albedo = c.rgb;

            fixed4 em = tex2D (_EmissionMap, IN.uv_MainTex) * _EmissionColor;
			o.Emission = em.rgb;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Mobile/VetexLit"
}
