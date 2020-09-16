Shader "Heyworks/Terrain" {
	Properties {
		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		_Mask ("Albedo (RGB)", 2D) = "white" {}
		_BlendFactor("Blend Factor", Range(-1, 1)) = 0
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		
		#pragma surface surf Lambert	

		sampler2D _MainTex;
		sampler2D _MainTex2;
		sampler2D _Mask;
		fixed _BlendFactor;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_MainTex2;
			float2 uv_Mask;
		};	

		void surf (Input IN, inout SurfaceOutput o) 
		{		
			fixed4 c1 = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D (_MainTex2, IN.uv_MainTex2);		
			fixed mask = tex2D (_Mask, IN.uv_Mask).r;
			fixed4 output = lerp(c1, c2, saturate(mask + _BlendFactor));
			o.Albedo = output.rgb;		
			o.Alpha = output.a;
		}
		ENDCG
	}
	FallBack "Mobile/Diffuse"
}
