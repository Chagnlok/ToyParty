Shader "Hidden/Custom/Flame 1" {
    Properties
    {
        _HSVAAdjust ("HSVA Adjust", Vector) = (0, 0, 0, 0)
    }
    SubShader {
        
        
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
                float2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                
            };
            

            float noise(vec3 p) //Thx to Las^Mercury
            {
                vec3 i = floor(p);
                vec4 a = dot(i, vec3(1., 57., 21.)) + vec4(0., 57., 21., 78.);
                vec3 f = cos((p-i)*acos(-1.))*(-.5)+.5;
                a = mix(sin(cos(a)*a),sin(cos(1.+a)*(1.+a)), f.x);
                a.xy = mix(a.xz, a.yw, f.y);
                return mix(a.x, a.y, f.z);
            }

            float sphere(vec3 p, vec4 spr)
            {
                return length(spr.xyz-p) - spr.w;
            }

            float flame(vec3 p)
            {
                float d = sphere(p*vec3(1.,.5,1.), vec4(.0,-1.,.0,1.));
                return d + (noise(p+vec3(.0,iTime*2.,.0)) + noise(p*3.)*.5)*.25*(p.y) ;
            }

            float scene(vec3 p)
            {
                return min(100.-length(p) , abs(flame(p)) );
            }

            vec4 raymarch(vec3 org, vec3 dir)
            {
                float d = 0.0, glow = 0.0, eps = 0.02;
                vec3  p = org;
                bool glowed = false;
                
                for(int i=0; i<64; i++)
                {
                    d = scene(p) + eps;
                    p += d * dir;
                    if( d>eps )
                    {
                        if(flame(p) < .0)
                            glowed=true;
                        if(glowed)
                            glow = float(i)/64.;
                    }
                }
                return vec4(p,glow);
            }


            
            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord * 2.0 - 1.0;
                o.texcoord.x *= iResolution.x / iResolution.y;
                o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                //o.fragcoord = v.texcoord * iResolution.xy;
                //o.color = v.color;
                
                return o;
            }
             
            fixed4 frag(v2f IN) : COLOR
            {
                // Softness factor
                float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
                float fade = clamp( min(factor.x, factor.y), 0.0, 1.0);


                vec2 v = IN.texcoord;
                
                vec3 org = vec3(0., -2., 4.);
                vec3 dir = normalize(vec3(v.x*1.6, -v.y, -1.5));
                
                vec4 p = raymarch(org, dir);
                float glow = p.w;
                
                vec4 col = mix(vec4(1.,.5,.1,1.), vec4(0.1,.5,1.,1.), p.y*.02+.4);
                
                fixed4 fragColor = mix(vec4(0,0,0,0), col, pow(glow*2.,4.));

                return fragColor * fade;
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
