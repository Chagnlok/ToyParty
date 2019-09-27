Shader "Hidden/Custom/BallOfFire 1" 
{
	Properties 
    {
        
        _HSVAAdjust ("HSVA Adjust", Vector) = (0, 0, 0, 0)
    }
    SubShader 
    {
        
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            //"RenderType" = "Transparent"
            "RenderType"="Opaque"
            //"DisableBatching" = "True"
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

            #define vec3 float3
            #define vec2 float2
            #define vec4 float4

            
            float4 _HSVAAdjust;

            float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
            float2 _ClipArgs0 = float2(1000.0, 1000.0);

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                
            };
            

            float snoise(vec3 uv, float res)
            {
                 const vec3 s = vec3(1e0, 1e2, 1e3);
    
                 uv *= res;
    
                 vec3 uv0 = floor(fmod(uv, res))*s;
                 vec3 uv1 = floor(fmod(uv+vec3(1.0f, 1.0f, 1.0f), res))*s;
    
                 vec3 f = frac(uv); f = f*f*(3.0-2.0*f);

                 vec4 v = vec4(uv0.x+uv0.y+uv0.z, uv1.x+uv0.y+uv0.z,
                          uv0.x+uv1.y+uv0.z, uv1.x+uv1.y+uv0.z);

                 vec4 r = frac(sin(v*1e-1)*1e3);
                 float r0 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);
    
                 r = frac(sin((v + uv1.z - uv0.z)*1e-1)*1e3);
                 float r1 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);
    
                 return lerp(r0, r1, f.z)*2.-1.;
            }

            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = -0.5f + v.texcoord;
                o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                //o.color = v.color;

                return o;
            }

            fixed4 frag(v2f IN) : COLOR
            {
                // Softness factor
                float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
                float fade = clamp( min(factor.x, factor.y), 0.0, 1.0);

                float iTime = _Time.y;
                vec2 p = IN.texcoord;
                //float2 iResolution = _ScreenParams.xy;
                //p.x *= iResolution.x/iResolution.y;
    
                float color = 3.0 - (3.*length(2.*p));
    
                vec3 coord = vec3(atan2(p.x,p.y)/6.2832+.5, length(p)*.4, .5);
    
                for(int i = 1; i <= 7; i++)
                {
                    float power = pow(2.0, float(i));
                    color += (1.5 / power) * snoise(coord + vec3(0.,-iTime*.05, iTime*.01), power*16.);
                }
                return vec4( color, pow(max(color,0.),2.)*0.4, pow(max(color,0.),3.)*0.15 ,  color * 4 / 3) * fade;
            
            }

            ENDCG
        }

    
    }
    FallBack "Diffuse" 
}
