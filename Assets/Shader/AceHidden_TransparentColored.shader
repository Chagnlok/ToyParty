Shader "AceHidden/Unlit/Transparent Colored"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			float4 _ClipRange0;
			float4 _ClipRange1;
			float4 _ClipRange2;
			float4 _ClipRange4;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				half4 color : COLOR;
				float2 v0 : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.v0 = v.vertex.xy;

				return o;
			}

			half4 frag (v2f IN) : SV_Target
			{
				float2 v0 = IN.v0 - _ClipRange0.xy;
				float2 v1 = _ClipRange0.zw;
				float d = v0.x * v1.y - v0.y * v1.x;
				if ( d > 0.0f )
					discard;

				v0 = IN.v0 - _ClipRange1.xy;
				v1 = _ClipRange1.zw;
				d = v0.x * v1.y - v0.y * v1.x;
				if ( d > 0.0f )
					discard;

				v0 = IN.v0 - _ClipRange2.xy;
				v1 = _ClipRange2.zw;
				d = v0.x * v1.y - v0.y * v1.x;
				if ( d > 0.0f )
					discard;

				v0 = IN.v0 - _ClipRange4.xy;
				v1 = _ClipRange4.zw;
				d = v0.x * v1.y - v0.y * v1.x;
				if ( d > 0.0f )
					discard;

				half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Premultiplied Colored"
}
