Shader "Custom/StencilShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Shadow Radius", float) = 1
        _Color ("Shadow Color", Color) = (0.9, 0.9, 0.9)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Stencil {
                Ref 2
                Comp always
                Pass replace
            }
            
            Cull Front
            ZTest Greater
            ZWrite Off
            Blend Zero One
        }
        
        Pass
        {
            Stencil {
                Ref 2
                Comp Equal
                Pass Zero
            }
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend DstColor Zero
            
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
                float4 worldDirection : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float3 worldPosition : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float3 _Color;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float4 screenPosition = o.vertex;
                screenPosition.y *= _ProjectionParams.x;
                
                float4 worldDirection;
                worldDirection.xyz = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
                worldDirection.w = dot(worldDirection.xyz, float3(0, -1, 0));

                o.worldDirection = worldDirection;
                o.screenPosition = screenPosition;
                o.worldPosition = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float perspectiveDivide = 1.0f / i.screenPosition.w;
                float3 direction = i.worldDirection.xyz * perspectiveDivide;
                float2 screenUV = (i.screenPosition.xy * perspectiveDivide) * 0.5f + 0.5f;
                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
                float3 worldspace = direction * depth + _WorldSpaceCameraPos;
                
                float2 delta = worldspace.xz - i.worldPosition.xz;
                float range = 1 - length(delta) / _Radius;
                float2 shadowUV = (delta / _Radius + 1) * 0.5f;
                fixed4 shadowColor = tex2D(_MainTex, shadowUV);
                
                float normalCoef = step(0, i.worldDirection.w);
                shadowColor.xyz = lerp(fixed3(1, 1, 1), _Color, normalCoef);
                return shadowColor;
            }
            ENDCG
        }
    }
}
