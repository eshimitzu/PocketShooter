// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-1944-OUT;n:type:ShaderForge.SFN_Append,id:5803,x:31482,y:32728,varname:node_5803,prsc:2|A-9729-OUT,B-8579-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8579,x:31196,y:32826,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_8579,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:4458,x:31694,y:32728,varname:node_4458,prsc:2|A-5803-OUT,B-1468-T;n:type:ShaderForge.SFN_Time,id:1468,x:31482,y:32885,varname:node_1468,prsc:2;n:type:ShaderForge.SFN_Add,id:4083,x:31875,y:32728,varname:node_4083,prsc:2|A-4458-OUT,B-8481-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8481,x:31694,y:32875,varname:node_8481,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:796,x:32043,y:32728,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_796,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a0e42e69f9790f7439ae7e6b1d1b400a,ntxv:0,isnm:False|UVIN-4083-OUT;n:type:ShaderForge.SFN_Tex2d,id:3833,x:32043,y:32934,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_3833,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8e5ef7d280fba3a49a1715f6bc308f6e,ntxv:0,isnm:False|UVIN-8481-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3581,x:32254,y:32917,varname:node_3581,prsc:2|A-796-RGB,B-3833-RGB;n:type:ShaderForge.SFN_Tex2d,id:1045,x:32265,y:32593,ptovrint:False,ptlb:BaseTexture,ptin:_BaseTexture,varname:node_1045,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ba802e9d32f036c42b66c91fa604dab6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:1944,x:32482,y:32756,varname:node_1944,prsc:2|A-1045-RGB,B-3581-OUT;n:type:ShaderForge.SFN_Vector1,id:9729,x:31196,y:32735,varname:node_9729,prsc:2,v1:0;proporder:1045-3833-796-8579;pass:END;sub:END;*/

Shader "KatanaGloss" {
    Properties {
        _BaseTexture ("BaseTexture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Gloss ("Gloss", 2D) = "white" {}
        _Speed ("Speed", Float ) = -0.5
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float _Speed;
            uniform sampler2D _Gloss; uniform float4 _Gloss_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _BaseTexture; uniform float4 _BaseTexture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _BaseTexture_var = tex2D(_BaseTexture,TRANSFORM_TEX(i.uv0, _BaseTexture));
                float4 node_1468 = _Time;
                float2 node_4083 = ((float2(0.0,_Speed)*node_1468.g)+i.uv0);
                float4 _Gloss_var = tex2D(_Gloss,TRANSFORM_TEX(node_4083, _Gloss));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (_BaseTexture_var.rgb+(_Gloss_var.rgb*_Mask_var.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
