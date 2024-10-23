Shader "Custom/My Lighting Shader" {
	
	Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
	}

	SubShader {

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			
			#include "LightBaseTerrain.cginc"

			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}
			
			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0
			#pragma multi_compile_fwdadd
			
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			
			#include "CelShadingTerrain.cginc"

			ENDCG
		}
	}
}