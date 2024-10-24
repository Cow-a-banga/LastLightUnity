Shader "Hidden/Terrain/AddPass" {
Properties {
	_Control ("Control (RGBA)", 2D) = "black" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
	_Scale ("Texture Scale", Float) = 1
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-99"
		"IgnoreProjector"="True"
		"RenderType" = "Opaque"
	}
	
CGPROGRAM
#pragma surface surf Lambert decal:add
struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float _Scale;

void surf (Input IN, inout SurfaceOutput o) {
	float2 realUV0, realUV1, realUV2, realUV3;
	
	realUV0 = IN.uv_Splat0 * _Scale;
	realUV1 = IN.uv_Splat1 * _Scale;
	realUV2 = IN.uv_Splat2 * _Scale;
	realUV3 = IN.uv_Splat3 * _Scale;
	
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	fixed3 col;
	
	col  = splat_control.r * tex2D (_Splat0, realUV0).rgb;
	col += splat_control.g * tex2D (_Splat1, realUV1).rgb;
	col += splat_control.b * tex2D (_Splat2, realUV2).rgb;
	col += splat_control.a * tex2D (_Splat3, realUV3).rgb;
	o.Albedo = col;

	o.Alpha = 0.0;
}
ENDCG  
}

Fallback off
}
