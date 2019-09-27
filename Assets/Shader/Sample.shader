Shader "Custom/Sample" {
	Properties {

	    // Softness factor    
        //_HSVAAdjust ("HSVA Adjust", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            //ZTest Always
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
                // Softness factor
                //float2 worldPos : TEXCOORD1;
            };

            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord * 2.0 - 1.0;
                o.texcoord.x *= iResolution.x / iResolution.y;
                
                return o;
            }

            fixed4 frag(v2f IN) : COLOR
            {
                // Softness factor
                //float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
                //float fade = clamp( min(factor.x, factor.y), 0.0, 1.0);

                return fixed4(0,0,0,0); //* fade;
            }

            ENDCG
        }
	}
	FallBack "Diffuse"
}
