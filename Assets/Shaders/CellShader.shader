Shader "Unlit/CellShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
	    _LineTex("LineTex", 2D) = "white" {}
	   _LineColor("Line Color", Color) = (1,1,1,1)
		_CellColor("Cell Color", Color) = (0,0,0,0)
		_SelectedColor("Selected Color", Color) = (1,0,0,1)
		[IntRange] _GridSize("Grid Size", Range(1,100)) = 10
		_LineSize("Line Size", Range(0,1)) = 0.15
		[IntRange] _SelectCell("Select Cell Toggle ( 0 = False , 1 = True )", Range(0,1)) = 0.0
		[IntRange] _SelectedCellX("Selected Cell X", Range(0,100)) = 0.0
		[IntRange] _SelectedCellY("Selected Cell Y", Range(0,100)) = 0.0
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
			sampler2D _LineTex;
            float4 _MainTex_ST;
			float4 _LineColor;
			float4 _CellColor;
			float4 _SelectedColor;

			float _GridSize;
			float _LineSize;

			float _SelectCell;
			float _SelectedCellX;
			float _SelectedCellY;

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

					float gsize = floor(_GridSize);



			gsize += _LineSize;

		/*	float2 id;

			id.x = floor(i.uv.x*i.vertex.x/ (1/ gsize));
			id.y = floor(i.uv.y*i.vertex.y /(1 / gsize));

			if (id.x > id.y)
				id.x = id.y;
		
			   if (id.y > id.x)
				id.y = id.x;*/
                fixed4 col = tex2D(_MainTex, i.uv);

				float4 color = _CellColor;
			
				//This checks that the cell is currently selected if the Select Cell slider is set to 1 ( True )
				/*if ( id.x == _SelectedCellX && id.y == _SelectedCellY)
				{
					
					color = _SelectedColor;

				}

				
*/

			

				
				float2 id;
				
				id.x = floor(i.uv.x / (1.0 / gsize));
				id.y = floor(i.uv.y / (1.0 / gsize));

			
				if ( frac(i.uv.x*gsize) <= _LineSize || frac(i.uv.y*gsize) <= _LineSize)// || frac(i.uv.x) >gsize-_LineSize || frac(i.uv.y) >1 - _LineSize)
				{
				
					color = tex2D(_LineTex, id)*_LineColor;
				}
				
				col =col* color;
				
                // apply fog
              //  UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
