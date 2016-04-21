Shader "Hidden/Berserk Effect" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_SwapTex("Color Data", 2D) = "transparent" {}
		_EffectStrength("Effect Strength", Float) = 0.2
		_LowThreshold("Low Threshold", Float) = 0.0
		_HighThreshold("Hight Threshold", Float) = 1.0
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			sampler2D _SwapTex;
			float _EffectStrength;
			float _LowThreshold;
			float _HighThreshold;

			fixed4 frag (v2f_img IN) : SV_Target {
				fixed4 final;
				fixed4 thresholdBlend;
				fixed4 c = tex2D(_MainTex, IN.uv);
				fixed4 swapCol = tex2D(_SwapTex, float2(c.r,0));
				thresholdBlend = clamp(c,fixed4(_LowThreshold,_LowThreshold,_LowThreshold,0.0f),fixed4(_HighThreshold,_HighThreshold,_HighThreshold,0.0f));
				final = lerp(c, swapCol, swapCol.a*_EffectStrength*thresholdBlend);
				return final;
			}
			ENDCG
		}
	}

Fallback off

}