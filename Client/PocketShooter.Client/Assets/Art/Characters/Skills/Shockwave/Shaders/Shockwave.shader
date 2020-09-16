// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33352,y:32672,varname:node_4795,prsc:2|normal-5291-OUT,emission-6816-OUT,alpha-8888-OUT,refract-2421-OUT;n:type:ShaderForge.SFN_Tex2d,id:8905,x:31871,y:32752,ptovrint:False,ptlb:RefractionMap,ptin:_RefractionMap,varname:node_8905,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3c99ba7ded2f0494c878a5fb3e115ea5,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Slider,id:1734,x:31744,y:32983,ptovrint:False,ptlb:NormalIntensity,ptin:_NormalIntensity,varname:node_1734,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector3,id:3559,x:31860,y:32575,varname:node_3559,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:5291,x:32183,y:32759,varname:node_5291,prsc:2|A-3559-OUT,B-8905-RGB,T-1734-OUT;n:type:ShaderForge.SFN_Slider,id:4830,x:31563,y:33235,ptovrint:False,ptlb:RefractionValue,ptin:_RefractionValue,varname:node_4830,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2057,x:32095,y:33095,varname:node_2057,prsc:2|A-1734-OUT,B-4830-OUT;n:type:ShaderForge.SFN_ComponentMask,id:56,x:32154,y:32916,varname:node_56,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8905-RGB;n:type:ShaderForge.SFN_Multiply,id:2255,x:32354,y:32926,varname:node_2255,prsc:2|A-56-OUT,B-2057-OUT;n:type:ShaderForge.SFN_Tex2d,id:8974,x:32354,y:33121,ptovrint:False,ptlb:OpacityMap,ptin:_OpacityMap,varname:node_8974,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5b1ea92730d2f9642ad511f7881d63bd,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8888,x:32690,y:32923,varname:node_8888,prsc:2|A-6051-OUT,B-8974-R,C-283-A;n:type:ShaderForge.SFN_ComponentMask,id:1758,x:32594,y:33152,varname:node_1758,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-8974-R;n:type:ShaderForge.SFN_Multiply,id:5711,x:32782,y:33072,varname:node_5711,prsc:2|A-2255-OUT,B-1758-OUT;n:type:ShaderForge.SFN_VertexColor,id:283,x:32759,y:33225,varname:node_283,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2421,x:33052,y:33085,varname:node_2421,prsc:2|A-5711-OUT,B-283-A;n:type:ShaderForge.SFN_Slider,id:6051,x:32441,y:32785,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_6051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:1318,x:32562,y:32545,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1318,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.03676468,c2:1,c3:0.880426,c4:1;n:type:ShaderForge.SFN_Multiply,id:6816,x:33026,y:32828,varname:node_6816,prsc:2|A-1318-RGB,B-283-RGB;proporder:8905-1734-8974-4830-6051-1318;pass:END;sub:END;*/

Shader "Shockwave" {
    Properties {
        _RefractionMap ("RefractionMap", 2D) = "black" {}
        _NormalIntensity ("NormalIntensity", Range(0, 1)) = 1
        _OpacityMap ("OpacityMap", 2D) = "white" {}
        _RefractionValue ("RefractionValue", Range(0, 1)) = 1
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Color ("Color", Color) = (0.03676468,1,0.880426,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
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
            uniform sampler2D _GrabTexture;
            uniform sampler2D _RefractionMap; uniform float4 _RefractionMap_ST;
            uniform float _NormalIntensity;
            uniform float _RefractionValue;
            uniform sampler2D _OpacityMap; uniform float4 _OpacityMap_ST;
            uniform float _Opacity;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float4 _RefractionMap_var = tex2D(_RefractionMap,TRANSFORM_TEX(i.uv0, _RefractionMap));
                float3 normalLocal = lerp(float3(0,0,1),_RefractionMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _OpacityMap_var = tex2D(_OpacityMap,TRANSFORM_TEX(i.uv0, _OpacityMap));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (((_RefractionMap_var.rgb.rg*(_NormalIntensity*_RefractionValue))*_OpacityMap_var.r.r)*i.vertexColor.a);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(_Opacity*_OpacityMap_var.r*i.vertexColor.a)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
