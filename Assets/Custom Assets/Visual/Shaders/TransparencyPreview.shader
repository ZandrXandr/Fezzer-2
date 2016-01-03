Shader "Custom/TransparencyPreview" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white"
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		ZWrite On

		Pass{
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask 0
		}
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput  o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = 0.6;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
