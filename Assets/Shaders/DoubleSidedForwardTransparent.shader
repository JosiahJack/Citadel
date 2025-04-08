Shader "Custom/TwoSidedTransparentSpecular" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.03, 1)) = 0.078125
        _Gloss ("Gloss", Range(0,1)) = 0.4
        _Alpha ("Transparency", Range(0,1)) = 1.0
        _DitherScale ("Dither Scale", Range(0.1, 100)) = 1.0 // Added for control
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
        LOD 200
        
        Cull Off // Renders both sides
        
        // Main surface pass
        CGPROGRAM
        #pragma surface surf BlinnPhong alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        half _Shininess;
        half _Gloss;
        fixed _Alpha;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Specular = _Shininess;
            o.Gloss = _Gloss;
            o.Alpha = c.a * _Alpha;
        }
        ENDCG

        // Shadow caster pass with finer dithered transparency
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            Cull Off // Two-sided shadows

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _Alpha;
            float _DitherScale;

            v2f vert(appdata_base v) {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            // Dithering function with adjustable scale
            float DitherClip(float alpha, float2 screenPos) {
                float4x4 thresholdMatrix = float4x4(
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                );
                // Increase resolution by scaling screen position
                float2 pos = screenPos * _ScreenParams.xy * _DitherScale;
                float dither = thresholdMatrix[fmod(pos.x, 4)][fmod(pos.y, 4)];
                return alpha - dither * 0.5;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 texcol = tex2D(_MainTex, i.uv) * _Color;
                fixed alpha = texcol.a * _Alpha;
                
                // Apply finer dithering
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                clip(DitherClip(alpha, screenUV));
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
