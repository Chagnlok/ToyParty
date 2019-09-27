

Shader "Custom/RainierMood" 
{

	Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
        _Resolution ("Resolution", float) = 10
    }

	SubShader 
    {
		LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
        }
        
        Pass
        {
        
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
           
            #pragma vertex vert
            #pragma fragment frag           
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Resolution;

            // Maximum number of cells a ripple can cross.
            #define MAX_RADIUS 2

            // Set to 1 to hash twice. Slower, but less patterns.
            #define DOUBLE_HASH 0

            // Hash functions shamefully stolen from:
            // https://www.shadertoy.com/view/4djSRW
            #define HASHSCALE1 .1031
            #define HASHSCALE3 float3(.1031, .1030, .0973)
        
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                
            };
    
            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;

                return o;
            }

            float hash12(float2 p)
            {
                 float3 p3  = frac(float3(p.xyx) * HASHSCALE1);
                 p3 += dot(p3, p3.yzx + 19.19);
                 return frac((p3.x + p3.y) * p3.z);
            }

            float2 hash22(float2 p)
            {
                 float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
                 p3 += dot(p3, p3.yzx+19.19);
                 return frac((p3.xx+p3.yz)*p3.zy);

            }
                
            fixed4 frag(v2f IN) : COLOR
            {
                float iTime = _Time.y / 2.0f;
                
                float2 fragCoord = IN.texcoord;
                float resolution = _Resolution;
                float2 uv = fragCoord * resolution;
                float2 p0 = floor(uv);

                float2 circles = float2(0.0f, 0.0f);
                for (int j = -MAX_RADIUS; j <= MAX_RADIUS; ++j)
                {
                    for (int i = -MAX_RADIUS; i <= MAX_RADIUS; ++i)
                    {
                        float2 pi = p0 + float2(i, j);
                        #if DOUBLE_HASH
                        float2 hsh = hash22(pi);
                        #else
                        float2 hsh = pi;
                        #endif
                        float2 p = pi + hash22(hsh);

                        float t = frac(0.3*iTime + hash12(hsh));
                        float2 v = p - uv;
                        float d = length(v) - (float(MAX_RADIUS) + 1.)*t;
             
                        float h = 1e-3;
                        float d1 = d - h;
                        float d2 = d + h;
                        float p1 = sin(31.*d1) * smoothstep(-0.6, -0.3, d1) * smoothstep(0., -0.3, d1);
                        float p2 = sin(31.*d2) * smoothstep(-0.6, -0.3, d2) * smoothstep(0., -0.3, d2);
                        circles += 0.5 * normalize(v) * ((p2 - p1) / (2. * h) * (1. - t) * (1. - t));
                    } 
                }
                circles /= float((MAX_RADIUS*2+1)*(MAX_RADIUS*2+1));

                float intensity = lerp(0.01, 0.15, smoothstep(0.1, 0.6, abs(frac(0.05*iTime + 0.5)*2.-1.)));
                float3 n = float3(circles, sqrt(1. - dot(circles, circles)));
                float3 color = tex2D(_MainTex, fragCoord - intensity*n.xy).rgb + 5.*pow(clamp(dot(n, normalize(float3(1., 0.7, 0.5))), 0., 1.), 6.);
                
                return float4(color, 1.0); 
             
                //float4 color = tex2D (_MainTex, float2(IN.texcoord  / _ScreenParams.xy));
                //return color * IN.color * 1.0f;
            }


            ENDCG
        }
	}
	FallBack "Diffuse"
}
