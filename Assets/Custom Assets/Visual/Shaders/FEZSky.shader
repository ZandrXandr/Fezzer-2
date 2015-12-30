Shader "Unlit/FEZSky"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TimeSpeed("TimeSpeed", Float) = 0
		_PixelCount("Pixel Count", Int) = 12
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull OFF

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
			float4 _MainTex_ST;
			int _TimeSpeed;
			int _PixelCount;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				float time = (_Time.y*_TimeSpeed)%_PixelCount;

				float4 c1 = tex2D(_MainTex, float2(time / _PixelCount,0.5f));

				// sample the texture
				fixed4 col = c1;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
}
