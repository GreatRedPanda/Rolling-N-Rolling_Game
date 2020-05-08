Shader "Unlit/GridShader"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_GridThickness("Grid Thickness", Float) = 0.1
		_GridSpacing("Grid Spacing", Float) = 0.1
		_GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
	}
		SubShader
	   {
		   Tags { "RenderType" = "Opaque" }
		   LOD 100

		   Pass
		   {
			   CGPROGRAM
			   #pragma vertex vert
			   #pragma fragment frag
			   // make fog work
			   #pragma multi_compile_fog

			   #include "UnityCG.cginc"

			   struct appdata
			   {
				   float4 vertex : POSITION;
				   float2 uv : TEXCOORD0;
			   };

			   struct v2f
			   {
				   float2 uv : TEXCOORD0;
				   UNITY_FOG_COORDS(1)
				   float4 vertex : SV_POSITION;
				   float2 worldPos:TEXCOORD1;
			   };

			   sampler2D _MainTex;
			   float4 _MainTex_ST;


			   // Access Shaderlab properties
			   uniform float _GridThickness;
			   uniform float _GridSpacing;
			   uniform float4 _GridColour;
			   uniform float4 _BaseColour;

			   v2f vert(appdata v)
			   {
				   v2f o;

				   o.vertex = UnityObjectToClipPos(v.vertex);

				   o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			    
				   UNITY_TRANSFER_FOG(o,o.vertex);

				o.worldPos = mul(unity_WorldToObject, o.uv);
				   o.worldPos.x += _GridThickness / 2 * _GridSpacing;
				   o.worldPos.y += _GridThickness / 2 * _GridSpacing;

				   return o;
			   }

			   fixed4 frag(v2f i) : SV_Target
			   {

					   fixed4 col = tex2D(_MainTex, i.uv);

					   float4 color = _GridColour;


					   if (frac(i.uv.x / _GridSpacing) < _GridThickness ||
						   frac(i.uv.y / _GridSpacing) < _GridThickness)
					   {
						   color = _GridColour;
					   }
					   else
					   {
						   color = _BaseColour;
					   }
					   col = col * color;


					   return col;
			   }
				   ENDCG
			   }
	   }
}
