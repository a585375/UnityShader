Shader "Custom/BlinnPhong" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecPower ("Specular Power", Range(0, 1)) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _SpecPower;
		fixed4 _Color;


		void surf (Input IN, inout SurfaceOutput  o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			o.Alpha = c.a;

			o.Specular = _SpecPower;//高光强度
		}
		ENDCG
	}
	FallBack "Diffuse"
}
