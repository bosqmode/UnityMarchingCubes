Shader "bosqmode/VertexColored"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 300

	  CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert

		float4 _Color;

		struct Input
		{
			float3 vertexcols;
		};

		void vert(inout appdata_full v, out Input o)
		{
		  o.vertexcols = v.color.rgb;

		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half3 c = IN.vertexcols.rgb * _Color.rgb;
			o.Albedo = c.rgb;
		}
		ENDCG
	}

		Fallback "Specular"
}