Shader "Heyworks/Mobile/UnlitNoFog" 
{
	Properties 
	{
		 _MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass 
		{
			Lighting Off
			Fog {Mode Off}
			SetTexture [_MainTex] 
			{
				constantColor (1,1,1,1)
				combine texture, constant
			}
		}
	}
}
