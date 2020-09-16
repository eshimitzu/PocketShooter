// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33960,y:32348,varname:node_4795,prsc:2|emission-3947-OUT;n:type:ShaderForge.SFN_Tex2d,id:906,x:32490,y:32386,ptovrint:False,ptlb:LuckyTexture,ptin:_LuckyTexture,varname:node_906,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:23aca464adb24d8458709d97e8b98b83,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:8832,x:32753,y:32860,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_8832,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:5939,x:33468,y:32513,varname:node_5939,prsc:2|A-7698-RGB,B-6902-OUT;n:type:ShaderForge.SFN_OneMinus,id:6902,x:33464,y:32778,varname:node_6902,prsc:2|IN-6667-OUT;n:type:ShaderForge.SFN_Multiply,id:6339,x:33385,y:32382,varname:node_6339,prsc:2|A-906-RGB,B-6667-OUT;n:type:ShaderForge.SFN_Add,id:3947,x:33683,y:32393,varname:node_3947,prsc:2|A-6667-OUT,B-5939-OUT;n:type:ShaderForge.SFN_Tex2d,id:7698,x:33063,y:32499,ptovrint:False,ptlb:BaseTexture,ptin:_BaseTexture,varname:node_7698,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4541d634c40c6e349bc77826c5005ec4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:9312,x:33349,y:33388,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:2751,x:33577,y:33375,varname:node_2751,prsc:2,tffrom:0,tfto:0|IN-9312-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8631,x:33806,y:33375,varname:node_8631,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2751-XYZ;n:type:ShaderForge.SFN_Tex2d,id:1374,x:33735,y:32958,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_1374,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f50bd16b3b2f92c4ba08407c1292e65d,ntxv:0,isnm:False|UVIN-4549-OUT;n:type:ShaderForge.SFN_Slider,id:8083,x:33298,y:33160,ptovrint:False,ptlb:NoiseSpeed,ptin:_NoiseSpeed,varname:node_8083,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_Add,id:4549,x:33878,y:33146,varname:node_4549,prsc:2|A-8631-OUT,B-1631-OUT;n:type:ShaderForge.SFN_Time,id:6848,x:33349,y:33262,varname:node_6848,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6667,x:33172,y:32763,varname:node_6667,prsc:2|A-906-RGB,B-1374-RGB,C-8832-OUT;n:type:ShaderForge.SFN_Multiply,id:1631,x:33643,y:33172,varname:node_1631,prsc:2|A-8083-OUT,B-6848-T;proporder:7698-906-8832-1374-8083;pass:END;sub:END;*/

Shader "Lucky" {
    Properties {
        _BaseTexture ("BaseTexture", 2D) = "white" {}
        _LuckyTexture ("LuckyTexture", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Noise ("Noise", 2D) = "white" {}
        _NoiseSpeed ("NoiseSpeed", Range(0, 1)) = 0.3
        
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RimColor ("Enemy Rim Color", Color) = (1, 0, 0, 0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _OutlineColor ("Enemy Outline Color", Color) = (1, 0, 0, 0)
        _Outline ("Outline width", Range (0.0, 1)) = .005
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _LuckyTexture; uniform float4 _LuckyTexture_ST;
            uniform float _Opacity;
            uniform sampler2D _BaseTexture; uniform float4 _BaseTexture_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _NoiseSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _LuckyTexture_var = tex2D(_LuckyTexture,TRANSFORM_TEX(i.uv0, _LuckyTexture));
                float4 node_6848 = _Time;
                float2 node_4549 = (i.normalDir.rgb.rg+(_NoiseSpeed*node_6848.g));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_4549, _Noise));
                float3 node_6667 = (_LuckyTexture_var.rgb*_Noise_var.rgb*_Opacity);
                float4 _BaseTexture_var = tex2D(_BaseTexture,TRANSFORM_TEX(i.uv0, _BaseTexture));
                float3 emissive = (node_6667+(_BaseTexture_var.rgb*(1.0 - node_6667)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        
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
    CustomEditor "ShaderForgeMaterialInspector"
}
