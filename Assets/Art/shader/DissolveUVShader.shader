// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1942,x:32988,y:32661,varname:node_1942,prsc:2|emission-6704-OUT,alpha-4384-OUT;n:type:ShaderForge.SFN_Tex2d,id:3939,x:32405,y:32774,ptovrint:False,ptlb:texture,ptin:_texture,varname:node_3939,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a5b002bd980e83e47988909ba2d312a4,ntxv:0,isnm:False|UVIN-1093-OUT;n:type:ShaderForge.SFN_TexCoord,id:9457,x:31635,y:32608,varname:node_9457,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:1093,x:32196,y:32726,varname:node_1093,prsc:2|A-4537-OUT,B-9457-U,T-1429-V;n:type:ShaderForge.SFN_TexCoord,id:1429,x:31573,y:32982,varname:node_1429,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Color,id:6553,x:32452,y:32463,ptovrint:False,ptlb:Diffusecolor,ptin:_Diffusecolor,varname:node_6553,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6704,x:32762,y:32737,varname:node_6704,prsc:2|A-6553-RGB,B-9803-RGB,C-3939-RGB;n:type:ShaderForge.SFN_VertexColor,id:9803,x:32452,y:32619,varname:node_9803,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1325,x:32131,y:33258,ptovrint:False,ptlb:DissloveTex,ptin:_DissloveTex,varname:node_1325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Step,id:4363,x:32337,y:33159,varname:node_4363,prsc:2|A-1429-U,B-1325-R;n:type:ShaderForge.SFN_SwitchProperty,id:670,x:32563,y:33111,ptovrint:False,ptlb:Disslove,ptin:_Disslove,varname:node_670,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5880-OUT,B-4363-OUT;n:type:ShaderForge.SFN_Vector1,id:5880,x:32327,y:33033,varname:node_5880,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:4384,x:32778,y:32915,varname:node_4384,prsc:2|A-3939-A,B-9803-A,C-670-OUT,D-7134-A;n:type:ShaderForge.SFN_Append,id:183,x:31958,y:32985,varname:node_183,prsc:2|A-1429-Z,B-1429-W;n:type:ShaderForge.SFN_Add,id:4537,x:32000,y:32575,varname:node_4537,prsc:2|A-9457-UVOUT,B-183-OUT;n:type:ShaderForge.SFN_Tex2d,id:7134,x:32517,y:32934,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_7134,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:3939-6553-1325-670-7134;pass:END;sub:END;*/

Shader "Dissolve/DissolveUVShader" {
    Properties {
        _texture ("texture", 2D) = "white" {}
        [HDR]_Diffusecolor ("Diffusecolor", Color) = (0.5,0.5,0.5,1)
        _DissloveTex ("DissloveTex", 2D) = "white" {}
        [MaterialToggle] _Disslove ("Disslove", Float ) = 1
        _Opacity ("Opacity", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float4 _Diffusecolor;
            uniform sampler2D _DissloveTex; uniform float4 _DissloveTex_ST;
            uniform fixed _Disslove;
            uniform sampler2D _Opacity; uniform float4 _Opacity_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
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
                float2 node_1093 = lerp((i.uv0+float2(i.uv1.b,i.uv1.a)),float2(i.uv0.r,i.uv0.r),i.uv1.g);
                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(node_1093, _texture));
                float3 emissive = (_Diffusecolor.rgb*i.vertexColor.rgb*_texture_var.rgb);
                float3 finalColor = emissive;
                float4 _DissloveTex_var = tex2D(_DissloveTex,TRANSFORM_TEX(i.uv0, _DissloveTex));
                float4 _Opacity_var = tex2D(_Opacity,TRANSFORM_TEX(i.uv0, _Opacity));
                fixed4 finalRGBA = fixed4(finalColor,(_texture_var.a*i.vertexColor.a*lerp( 1.0, step(i.uv1.r,_DissloveTex_var.r), _Disslove )*_Opacity_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
