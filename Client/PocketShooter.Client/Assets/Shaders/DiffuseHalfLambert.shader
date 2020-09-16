// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/DiffuseHalfLambert" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf HalfLambert noforwardadd

inline fixed4 LightingHalfLambert(SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed diff = dot(s.Normal, lightDir) * 0.5 + 0.5;
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
	c.a = s.Alpha;
	return c;
}

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
