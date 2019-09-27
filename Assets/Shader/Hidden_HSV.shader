// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Custom/HSVRangeShader 1"
{
	Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
       //_Color ("Alpha Color Key", Color) = (0,0,0,1)
       //_HSVRangeMin ("HSV Affect Range Min", Range(0, 1)) = 0
       //_HSVRangeMax ("HSV Affect Range Max", Range(0, 1)) = 1
       _HSVAAdjust ("HSVA Adjust", Vector) = (0, 0, 0, 0)
       //_StencilComp ("Stencil Comparison", Float) = 8
       //_Stencil ("Stencil ID", Float) = 0
       //_StencilOp ("Stencil Operation", Float) = 0
       //_StencilWriteMask ("Stencil Write Mask", Float) = 255
       //_StencilReadMask ("Stencil Read Mask", Float) = 255
       //_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }


        //Stencil
        //{
        //    Ref [_Stencil]
        //    Comp [_StencilComp]
        //    Pass [_StencilOp]
        //    ReadMask [_StencilReadMask]
        //    WriteMask [_StencilWriteMask]
        //}
        //ColorMask [_ColorMask]


        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON

            sampler2D _MainTex;
            //float4 _Color;
            //float _HSVRangeMin;
            //float _HSVRangeMax;
            float4 _HSVAAdjust;

            float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);

            struct Vertex
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct Fragment
            {
                float4 vertex : POSITION;
                half2 uv_MainTex : TEXCOORD0;
                fixed4 color : COLOR;
                float2 worldPos : TEXCOORD1;
            };

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color;
                o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                return o;
            }

            float3 rgb2hsv(float3 c) {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed4 frag(Fragment IN) : COLOR
            {
            	// Softness factor
				float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
				float fade = clamp( min(factor.x, factor.y), 0.0, 1.0);
                //float4 o = float4(1, 0, 0, 0.2);

                float4 color = tex2D (_MainTex, IN.uv_MainTex);
                float3 hsv = rgb2hsv(color.rgb);
                //float affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
                //float3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult) * IN.color.rgb;
                float3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz ) * IN.color.rgb;
                //float3 rgb2 = lerp(half3(0.0, 0.0, 0.0), rgb, fade);

                return fixed4(rgb, (color.a + _HSVAAdjust.a) * IN.color.a * fade );
            }

            ENDCG
        }
    }
}
