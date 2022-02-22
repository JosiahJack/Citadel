Shader "Custom/AutomapMaskingFoW" {
Properties {
	_MainTex("MainTexture",2D) = "black"{}
    _Stencil ("Stencil ID", Float) = 0
	_StencilComp ("Stencil Comparison", Float) = 8
	_StencilOp ("Stencil Operation", Float) = 0
	_StencilWriteMask ("Stencil Write Mask", Float) = 255
	_StencilReadMask ("stencil Read Mask", Float) = 255
	_ColorMask ("Color Mask", Float) = 15
}
    SubShader {
        Tags { "Queue" = "Background-1" }

        Pass {
            ColorMask 0
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
               float4 vertex : POSITION;
            };
     
            struct v2f
            {
               float4 vertex : SV_POSITION;
               float size : PSIZE;
            };
         
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.size = 30;
                return o;
            }
         
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}