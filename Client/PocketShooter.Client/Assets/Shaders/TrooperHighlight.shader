Shader "Custom/TrooperHighlight" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RimColor ("Enemy Rim Color", Color) = (1, 0, 0, 0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _OutlineColor ("Enemy Outline Color", Color) = (1, 0, 0, 1)
        _Outline ("Outline width", Range (0.0, 2)) = 2
    }

    SubShader
    {
        CGPROGRAM

        #pragma surface surf Lambert 

        struct Input 
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        sampler2D _MainTex;
        float4 _RimColor;
        float _RimPower;

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            half dotProduct = saturate(dot (normalize(IN.viewDir), o.Normal));
            half rim = 1.0 - dotProduct;

            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
        }

        ENDCG


         Pass
         {
            Cull Front
            Blend One OneMinusDstColor

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            uniform float _Outline;
            uniform float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = v.vertex;
                o.pos.xyz += v.normal.xyz *_Outline * 0.01;
                o.pos = UnityObjectToClipPos(o.pos);

                o.color = _OutlineColor;

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}