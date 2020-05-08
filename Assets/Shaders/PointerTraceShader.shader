// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PointerTraceShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {} // Regular object texture 
		_PointerPosition("Pointer Position", vector) = (0,0,0,0) // The location of the player - will be set by script
		_VisibleDistance("Visibility Distance", float) = 10.0 // How close does the player have to be to make object visible
		_OutlineWidth("Outline Width", float) = 3.0 // Used to add an outline around visible area a la Mario Galaxy
		_OutlineColour("Outline Colour", color) = (1.0,1.0,0.0,1.0) // Colour of the outline
		_MeshColour("Mesh Colour", color) = (1.0,1.0,0.0,1.0) // Colour of the outline

	}
		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag       

			// Access the shaderlab properties
			uniform sampler2D _MainTex;
			uniform float4 _PointerPosition;
			uniform float _VisibleDistance;
			uniform float _OutlineWidth;
			uniform fixed4 _OutlineColour;
			uniform fixed4 _MeshColour;
			// Input to vertex shader
			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			// Input to fragment shader
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 position_in_world_space : TEXCOORD0;
				float4 tex : TEXCOORD1;
			};

			// VERTEX SHADER
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
				output.tex = input.texcoord;
				return output;
			}

			// FRAGMENT SHADER
			float4 frag(vertexOutput input) : COLOR
			{
				// Calculate distance to player position
				float dist = distance(input.position_in_world_space, _PointerPosition);

			//float speed = fwidth(dist);


			//float3 wrapped = frac(input.position_in_world_space) - 0.5f;
		/*	float3 range = abs(input.position_in_world_space);

			float3 speeds = fwidth(input.position_in_world_space/_PointerPosition);

			float3 pixelRange = range / speeds;

			float lineWeight = saturate(min(min(pixelRange.x, pixelRange.y), pixelRange.z) - _VisibleDistance);

*/
			if(dist < _VisibleDistance+ _OutlineWidth  && dist> _VisibleDistance)
			{
				float diff = _VisibleDistance+ _OutlineWidth - dist;
				return lerp( _MeshColour, _OutlineColour,(diff) / (_OutlineWidth));
			}
			
			else 
				if (dist < _VisibleDistance)
			{
				
				return _OutlineColour;
			}
			else
			{
				return _MeshColour;
			}
				

		}

		ENDCG
		} // End Pass
		} // End Subshader
			FallBack "Diffuse"
} // End Shader
