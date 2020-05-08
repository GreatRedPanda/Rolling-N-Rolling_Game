// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GridSkyBox"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
		_CellColour("CellColour", Color) = (0.5, 1.0, 1.0, 1.0)
		_OffsetX("OffsetX",  Float) = 0.1
		_CellCountX("CellCountX" , Float) = 0.1
		_OffsetY("OffsetY",  Float) = 0.1
		_CellCountY("CellCountY" , Float) = 0.1
		_LineSize("LineSize", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
		     Cull Front
			ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
       

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD1;
                
                float4 vertex : POSITION;
				float3 worldPos: TEXCOORD0;
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
                o.uv = v.uv;
               
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz/(_CellCountX);


			/*	OUT.position = mul(glstate.matrix.mvp, IN.vertex);
				OUT.texcoord = IN.texcoord;*/
                return o;
            }

            void frag ( v2f i, out fixed4 col:COLOR, out float depth: DEPTH)
            {
    
						
				 float2 id;// = frac(i.uv*10.24);
				 id.x = (i.uv.x*_CellCountX + _OffsetX);
				 id.y = (i.uv.y*_CellCountY + _OffsetY);



				 float2 wrapped = frac(id) - 0.5f;
				 float2 range = abs(wrapped);

				 float2 speeds;

				 speeds = fwidth(id);

				 float2 pixelRange = range / speeds;
				 float lineWeight = saturate(min(pixelRange.x, pixelRange.y) - _LineSize);

				 col= lerp(_GridColour, _CellColour, lineWeight);



				col.w = 1;
			   	depth = 0;
             
            }
            ENDCG
        }
    }
}
