// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Image Effects/Blink Curved"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LocalTime ("Time", Float) = 0
		_Smoothness ("Smoothness", Float) = 0
		_Curvature ("Curvature", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			float _LocalTime;
			float _Smoothness;
			float _Curvature;
			half4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.vertex = ComputeNonStereoScreenPos(v.vertex);
				//o.vertex.xy = TransformStereoScreenSpaceTex(o.vertex.xy, v.vertex.w);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//o.uv = v.uv;
				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				//fixed4 col = tex2D(_MainTex,  UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
				float v = i.uv[1];
				float curveFactor = pow (0.5 - i.uv[0], 2) * _Curvature;
				float v1 = v - curveFactor;
				float v2 = v + curveFactor;

				float darkness = saturate (_LocalTime - v1) * _Smoothness;
				darkness += saturate (_LocalTime - (1 - v2)) * _Smoothness;
				col.rgb *= 1 - darkness;
				return col;
			}
			ENDCG
		}
	}
}
