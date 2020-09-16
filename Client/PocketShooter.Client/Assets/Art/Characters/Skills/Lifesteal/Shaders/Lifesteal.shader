// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32683,y:32690,varname:node_4795,prsc:2|emission-2393-OUT,alpha-2568-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32413,y:32782,varname:node_2393,prsc:2|A-5886-RGB,B-2053-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32166,y:32847,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2568,x:32338,y:33004,varname:node_2568,prsc:2|A-7979-R,B-2053-A,C-5886-A;n:type:ShaderForge.SFN_Tex2d,id:7979,x:32121,y:33111,ptovrint:False,ptlb:OpacityTexture,ptin:_OpacityTexture,varname:node_7979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:34499c93fe2394d4ab7ba2c3888f533b,ntxv:0,isnm:False|UVIN-1865-OUT;n:type:ShaderForge.SFN_Append,id:5084,x:31661,y:33348,varname:node_5084,prsc:2|A-4258-OUT,B-4074-OUT;n:type:ShaderForge.SFN_Multiply,id:1659,x:31835,y:33348,varname:node_1659,prsc:2|A-5084-OUT,B-8604-T;n:type:ShaderForge.SFN_Time,id:8604,x:31661,y:33487,varname:node_8604,prsc:2;n:type:ShaderForge.SFN_Add,id:1865,x:32012,y:33348,varname:node_1865,prsc:2|A-1659-OUT,B-3941-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3941,x:31835,y:33487,varname:node_3941,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:4258,x:31362,y:33470,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_4258,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:5886,x:31942,y:32748,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5886,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3495bc9884a5eca458bd6d5a34a6163c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:4074,x:31431,y:33681,varname:node_4074,prsc:2,v1:0;proporder:5886-7979-4258;pass:END;sub:END;*/

Shader "Lifesteal" {
    Properties {
        _Color ("Color", 2D) = "white" {}
        _OpacityTexture ("OpacityTexture", 2D) = "white" {}
        _Speed ("Speed", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _OpacityTexture; uniform float4 _OpacityTexture_ST;
            uniform float _Speed;
            uniform sampler2D _Color; uniform float4 _Color_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Color_var = tex2D(_Color,TRANSFORM_TEX(i.uv0, _Color));
                float3 emissive = (_Color_var.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 node_8604 = _Time;
                float2 node_1865 = ((float2(_Speed,0.0)*node_8604.g)+i.uv0);
                float4 _OpacityTexture_var = tex2D(_OpacityTexture,TRANSFORM_TEX(node_1865, _OpacityTexture));
                fixed4 finalRGBA = fixed4(finalColor,(_OpacityTexture_var.r*i.vertexColor.a*_Color_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
