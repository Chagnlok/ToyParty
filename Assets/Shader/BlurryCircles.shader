Shader "Custom/BlurryCircles" 
{
	Properties 
    {
        
	}
	SubShader 
    {
		

        LOD 200

		Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Always
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members wxy)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            #define vec3 float3
            #define vec2 float2
            #define vec4 float4
            #define iResolution _ScreenParams
            #define iTime (_Time.y)
            #define mat2 float2x2
            #define mat3 float3x3
            #define fract frac
            #define mix lerp
            #define mod(a, b)   ( (a) - ( (b) * floor( (a) / (b) ) ) )

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
          
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                //float2 fragcoord : TEXCOORD1;
                //float  wxy;
            };
            
            
            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord * 2.0 - 1.0;
                o.texcoord.x *= iResolution.x / iResolution.y;
                //o.fragcoord = v.texcoord * iResolution.xy;
                //o.color = v.color;
                
                return o;
            }


            fixed4 frag(v2f IN) : COLOR
            {
                vec2 w = iResolution.xy;
                vec2 p = IN.texcoord ;
                vec2 o = p;
                
                vec3 d=vec3(0, 0, 0);
                float t=iTime*.1,e=length(o),k=o.y+o.x;
                float l,r,a;
                for(int i=0;i<40;i++)
                {
                    a = float(i);
                    r = fract(sin(a*9.7)) * 0.8;
                    vec2 temp = p + vec2(sin(a+a-t),cos(t+a)+t*.1);
                    p = mod(temp, 2.0) - 1.0  ;
                    l = length( p );
                    d += pow(
                        mix(    vec3(.6,.46,.4),
                                vec3(.25,.15,.3)+vec3(0,k,k)*.25,a/40.),
                                vec3(3,3,3))
                        * (pow(max(1.-abs(l-r+e*.2),0.),25.)*.2+smoothstep(r,r-e*.2,l));
                }

                vec3 c = sqrt(d)*1.4;
                return vec4(c, 1.0f);
            }

            ENDCG

        }



		
	}
	FallBack "Diffuse"
}
