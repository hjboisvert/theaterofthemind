// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (c) 2016 Yasuhide Okamoto
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

Shader "PIXPRO/PixPro4kFishEye"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mult("Multiplier", float) = 1.0 // used for fade in/out in FadeScript.cs
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        //Cull front // ***ADDED***
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _K_ANGLE;
            float _Mult;
            float _MaskWidth;
            float _MaskHeight;
            float _MaskTransition;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                //v.uv[0] = 1 - v.uv[0]; // ***ADDED***
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float phi = i.uv[0] * 2.0 * UNITY_PI;
                float sf = 180.0 / _K_ANGLE;
                //float r = (1.0 - i.uv[1]) * sf;
                float r = (i.uv[1]) * sf;
                float2 fisheye_uv = float2(cos(phi), sin(phi)) * r;

                float2 new_uv = (fisheye_uv + float2(1.0, 1.0)) * 0.5;
                //float2 new_uv = fisheye_uv;

                // clip
                float intex = step(0.0, new_uv[0]) * step(new_uv[0], 1.0) * step(0.0, new_uv[1]) * step(new_uv[1], 1.0);

                float4 col = tex2D(_MainTex, new_uv) * intex;

                float2 center = float2(0.5, 0.5);
                float vx = new_uv[0] - center[0];
                float vy = new_uv[1] - center[1];

                float maskRadiusX = clamp(_MaskWidth / 2, 0.0, 0.5);
                float maskRadiusY = clamp(_MaskHeight / 2, 0.0, 0.5);

                // if this is >= 1, the point (vx,vy) is in masked area
                float ellipse = (vx * vx) / (maskRadiusX * maskRadiusX) + (vy * vy) / (maskRadiusY * maskRadiusY);
                if (ellipse > 1.0) {
                    float x = clamp((ellipse - 1.0) / _MaskTransition, 0.0, 1.0);
                    fixed coserp = 0.5 * (1 - cos(x * 3.1415927));
                    col = col * (1-coserp);
                }

                return col * _Mult;
            }
            ENDCG
        }
    }
}
