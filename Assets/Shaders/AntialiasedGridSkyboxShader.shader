Shader "Unlit/AntialiasedGridSkyboxShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Scale("Scale",  Float) = 10
				   _GridColour("Grid Colour", color) = (1, 1, 1, 1)
		_BaseColour("Base Colour", color) = (1, 1, 1, 0)
		_GridSpacing("Grid Spacing", float) = 0.1
		_LineThickness("Line Thickness", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
		

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members vertex)
		   //#extension GL_OES_standard_derivatives : enable	
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
          //  #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
              
                float4 vertex : SV_POSITION;
				float3 worldPos: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Scale;
			const float pi = 3.141592653589793;
			fixed4 _GridColour;
			fixed4 _BaseColour;
			float _GridSpacing;
			float _LineThickness;
	


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos= mul(unity_ObjectToWorld, v.vertex).xyz / _GridSpacing;
					//mul(unity_ObjectToWorld, v.vertex).xy / _GridSpacing;
					//TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i): SV_Target
            {
				

				float3 wrapped = frac(i.worldPos) - 0.5f;
				float3 range = abs(wrapped);

				float3 speeds;

				speeds = fwidth(i.worldPos);

				float3 pixelRange = range / speeds;
				float lineWeight = saturate(min(min(pixelRange.x, pixelRange.y),pixelRange.z) - _LineThickness);



				//float2 coord = float2(length(i.worldPos.xz), atan2(i.worldPos.x, i.worldPos.z) * _Scale / pi);
				//float2 wrappedCoord = float2(coord.x, frac(coord.y / (2.0 * _Scale)) * (2.0 * _Scale));
				//float2 coordWidth = fwidth(coord);
				//float2 wrappedWidth = fwidth(wrappedCoord);
				//float2 width = coord.y < -_Scale * 0.5 || coord.y > _Scale * 0.5 ? wrappedWidth : coordWidth;
				//float2 grid = abs(frac(coord - 0.5) - 0.5) / width;
				//float li = min(grid.x, grid.y);
				//li = saturate( min(li, 1) - _LineThickness);

				return lerp(_GridColour, _BaseColour, lineWeight);

                //return col;
            }
            ENDCG
        }
    }
}
