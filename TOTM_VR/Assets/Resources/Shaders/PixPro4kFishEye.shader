﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (c) 2016 Yasuhide Okamoto
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

Shader "PIXPRO/PixPro4kFishEye"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaTex("Mask", 2D) = "white" {}
        _KAngle ("k angle", float) = 104.0
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
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _MainTex_ST;
            float _KAngle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //v.uv[0] = 1 - v.uv[0]; // ***ADDED***
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float phi = i.uv[0] * 2.0 * UNITY_PI;
                float sf = 180.0 / _KAngle;
                //float r = (1.0 - i.uv[1]) * sf;
                float r = (i.uv[1]) * sf;
                float2 fisheye_uv = float2(cos(phi), sin(phi)) * r;

                float2 new_uv = (fisheye_uv + float2(1.0, 1.0)) * 0.5;
                //float2 new_uv = fisheye_uv;

                // clip
                float intex = step(0.0, new_uv[0]) * step(new_uv[0], 1.0) * step(0.0, new_uv[1]) * step(new_uv[1], 1.0);

                //fixed4 m = tex2D(_AlphaTex, i.uv);
                float4 col = tex2D(_MainTex, new_uv) * intex;
                //col[3] = tex2D(_AlphaTex, i.uv).r;
                col = col * (tex2D(_AlphaTex, new_uv).a);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);               
                return col;
            }
            ENDCG
        }

        /*
        GrabPass {"_GrabTex"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _GrabTex;
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //v.uv[0] = 1 - v.uv[0]; // ***ADDED***
                o.uv = ComputeGrabScreenPos(o.vertex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2Dproj(_GrabTex, i.uv) * (1-tex2D(_AlphaTex, i.uv).r);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }*/
    }
}
