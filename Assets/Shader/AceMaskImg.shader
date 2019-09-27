Shader "Custom/AceMaskImg"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}

		Pass
		{

			Cull Off
			Lighting Off
			ZWrite Off
			AlphaTest Off
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			Stencil {
                Ref 2
                Comp equal
                Pass keep
               
             
            }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;


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
            };

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color;
                return o;
            }

            fixed4 frag(Fragment IN) : COLOR
            {
                //float4 o = float4(1, 0, 0, 0.2);

                float4 color = tex2D (_MainTex, IN.uv_MainTex);
                return color * IN.color;
            }

			ENDCG
		}
	}
}
