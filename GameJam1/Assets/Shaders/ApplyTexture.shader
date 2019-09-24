Shader "Custom/ApplyTexture"
{
	Properties
	{
		_MainTex("Main Texture (RBG)", 2D) = "white" {} //texture property
	_Colour("Color", Color) = (1,1,1,1) //colour property
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM //allows talk between two languages: shader lab and nvidia C for graphics

				  ///Function defines - defines the name for the vertex and fragment functions
#pragma vertex vert //define for the building function
#pragma fragment frag //define for the colouring function

				  ///Includes
#include "UnityCG.cginc" //built in shader functions

				  ///Structures - gets data: ex. vertices, normal, colour, uv, etc.
		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	///Imports - re-import property from shaderlab to nvidia cg
	float4 _Colour;
	sampler2D _MainTex;

	///Vertex Function - builds the object
	v2f vert(appdata IN)
	{
		v2f OUT;

		OUT.pos = UnityObjectToClipPos(IN.vertex);
		OUT.uv = IN.uv;

		return OUT;
	}

	///Fragment Function - colour it in
	fixed4 frag(v2f IN) : SV_Target
	{
		float4 texColour = tex2D(_MainTex, IN.uv);
		return texColour * _Colour;
	}

		ENDCG
	}
	}
}
