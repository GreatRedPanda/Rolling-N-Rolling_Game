Shader "Unlit/Rectangle"
{
	Properties
	{
		_GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
		_CellColour("CellColour", Color) = (0.5, 1.0, 1.0, 1.0)
		_OffsetX ("OffsetX",  Float) = 0.1
		_CellCountX("CellCountX" , Float) = 0.1
	    _OffsetY("OffsetY",  Float) = 0.1
		_CellCountY("CellCountY" , Float) = 0.1


		_LineSize("LineSize", Float)=0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			uniform float4 _GridColour;

			float _CellCountX;
			
			float _CellCountY;
		
			float _OffsetX;
			float _OffsetY;
			float _LineSize;
			float4 _CellColour;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
               
			float2 id;// = frac(i.uv*10.24);
			id.x = frac(i.uv.x*_CellCountX + _OffsetX);
			id.y = frac(i.uv.y*_CellCountY + _OffsetY);

			//col.x = col.x*frac(i.uv.x * 2);
			if (id.x<_LineSize || id.x>(1-_LineSize) || id.y< _LineSize || id.y>(1 - _LineSize))
				col = _GridColour;
			else
				col = _CellColour;
                return col;
            }
            ENDCG
        }
    }
}
