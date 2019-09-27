Shader "Custom/AceMaskArea"
{
	Properties
	{
		
	}
	SubShader
	{
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
			Blend One OneMinusSrcAlpha

			Stencil {
                Ref 2
                Comp always
                Pass replace
             
            }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"



			struct appdata_t
			{
				float4 vertex : POSITION;
				//fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				//half4 color : COLOR;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.color = v.color;

				return o;
			}

			half4 frag (v2f IN) : SV_Target
			{
				//discard;
				//return IN.color;
				return half4(0,0,0,0);
			}
			ENDCG
		}

	}
}
