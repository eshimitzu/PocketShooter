// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33253,y:32664,varname:node_4795,prsc:2|emission-1217-OUT,alpha-3294-OUT,voffset-2225-OUT;n:type:ShaderForge.SFN_NormalVector,id:2681,x:30690,y:33301,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:8321,x:31002,y:33253,varname:node_8321,prsc:2,tffrom:0,tfto:1|IN-2681-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8121,x:31221,y:33260,varname:node_8121,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-8321-XYZ;n:type:ShaderForge.SFN_RemapRange,id:965,x:31423,y:33260,varname:node_965,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-8121-OUT;n:type:ShaderForge.SFN_Rotator,id:7071,x:31624,y:33260,varname:node_7071,prsc:2|UVIN-965-OUT,SPD-1774-OUT;n:type:ShaderForge.SFN_Panner,id:844,x:31814,y:33260,varname:node_844,prsc:2,spu:0.5,spv:0.25|UVIN-7071-UVOUT,DIST-3681-OUT;n:type:ShaderForge.SFN_Multiply,id:3681,x:31624,y:33396,varname:node_3681,prsc:2|A-1774-OUT,B-9079-T;n:type:ShaderForge.SFN_Time,id:9079,x:31423,y:33476,varname:node_9079,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:7628,x:32034,y:33203,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:445e3bf3ae883184fa00ea5e01f09028,ntxv:0,isnm:False|UVIN-844-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2225,x:32266,y:33420,varname:node_2225,prsc:2|A-7628-RGB,B-6583-OUT,C-2681-OUT;n:type:ShaderForge.SFN_Fresnel,id:4267,x:31651,y:32922,varname:node_4267,prsc:2|EXP-7276-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7276,x:31435,y:32972,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_7276,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_SwitchProperty,id:7153,x:32261,y:32782,ptovrint:False,ptlb:UseFresnel,ptin:_UseFresnel,varname:node_7153,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3034-OUT,B-4267-OUT;n:type:ShaderForge.SFN_Slider,id:3773,x:32384,y:33243,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_3773,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:6583,x:31813,y:33585,ptovrint:False,ptlb:VertexOffset,ptin:_VertexOffset,varname:node_6583,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1301138,max:1;n:type:ShaderForge.SFN_Slider,id:3034,x:31846,y:32981,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_3034,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1774,x:31217,y:33428,ptovrint:False,ptlb:NoiseSpeed,ptin:_NoiseSpeed,varname:node_1774,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1598172,max:1;n:type:ShaderForge.SFN_Multiply,id:3294,x:32731,y:33070,varname:node_3294,prsc:2|A-7628-G,B-3773-OUT;n:type:ShaderForge.SFN_Tex2d,id:8678,x:32676,y:32675,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_8678,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c979405936bde4e42a3c896165821c91,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:1217,x:32963,y:32692,varname:node_1217,prsc:2|A-7153-OUT,B-8678-RGB;proporder:7628-7276-7153-3773-6583-3034-1774-8678;pass:END;sub:END;*/

Shader "Immortality" {
    Properties {
        _Noise ("Noise", 2D) = "white" {}
        _Fresnel ("Fresnel", Float ) = 2
        [MaterialToggle] _UseFresnel ("UseFresnel", Float ) = 0
        _Opacity ("Opacity", Range(0, 1)) = 1
        _VertexOffset ("VertexOffset", Range(0, 1)) = 0.1301138
        _Emission ("Emission", Range(0, 1)) = 0
        _NoiseSpeed ("NoiseSpeed", Range(0, 1)) = 0.1598172
        _Texture ("Texture", 2D) = "white" {}
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
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Fresnel;
            uniform fixed _UseFresnel;
            uniform float _Opacity;
            uniform float _VertexOffset;
            uniform float _Emission;
            uniform float _NoiseSpeed;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_9079 = _Time;
                float4 node_8838 = _Time;
                float node_7071_ang = node_8838.g;
                float node_7071_spd = _NoiseSpeed;
                float node_7071_cos = cos(node_7071_spd*node_7071_ang);
                float node_7071_sin = sin(node_7071_spd*node_7071_ang);
                float2 node_7071_piv = float2(0.5,0.5);
                float2 node_7071 = (mul((mul( unity_WorldToObject, float4(v.normal,0) ).xyz.rgb.rgb*0.5+0.5)-node_7071_piv,float2x2( node_7071_cos, -node_7071_sin, node_7071_sin, node_7071_cos))+node_7071_piv);
                float2 node_844 = (node_7071+(_NoiseSpeed*node_9079.g)*float2(0.5,0.25));
                float4 _Noise_var = tex2Dlod(_Noise,float4(TRANSFORM_TEX(node_844, _Noise),0.0,0));
                v.vertex.xyz += (_Noise_var.rgb*_VertexOffset*v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 emissive = (lerp( _Emission, pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel), _UseFresnel )+_Texture_var.rgb);
                float3 finalColor = emissive;
                float4 node_9079 = _Time;
                float4 node_8838 = _Time;
                float node_7071_ang = node_8838.g;
                float node_7071_spd = _NoiseSpeed;
                float node_7071_cos = cos(node_7071_spd*node_7071_ang);
                float node_7071_sin = sin(node_7071_spd*node_7071_ang);
                float2 node_7071_piv = float2(0.5,0.5);
                float2 node_7071 = (mul((mul( unity_WorldToObject, float4(i.normalDir,0) ).xyz.rgb.rgb*0.5+0.5)-node_7071_piv,float2x2( node_7071_cos, -node_7071_sin, node_7071_sin, node_7071_cos))+node_7071_piv);
                float2 node_844 = (node_7071+(_NoiseSpeed*node_9079.g)*float2(0.5,0.25));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_844, _Noise));
                fixed4 finalRGBA = fixed4(finalColor,(_Noise_var.g*_Opacity));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
