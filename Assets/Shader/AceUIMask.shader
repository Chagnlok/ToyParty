Shader "Ace/AceUIMask" {
	Properties
   {
      _MainTex ("Base (RGBA)", 2D) = "white" {}
      _Mask ("Culling Mask", 2D) = "white" {}
      _Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
   }
   SubShader
   {
      LOD 100
   
      Tags 
      {
            "Queue" = "Transparent+500"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
      }
      Lighting Off
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      AlphaTest GEqual [_Cutoff]
      Pass
      {
         SetTexture [_Mask] {combine texture}
         SetTexture [_MainTex] {combine texture, previous}
      }
   }
    FallBack "Unlit/Transparent"
}
