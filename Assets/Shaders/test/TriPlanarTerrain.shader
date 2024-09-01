/*Please do support www.bitshiftprogrammer.com by joining the facebook page : fb.com/BitshiftProgrammer
Legal Stuff:
This code is free to use no restrictions but attribution would be appreciated.
Any damage caused either partly or completly due to usage this stuff is not my responsibility*/
Shader "BitshiftProgrammer/TriPlanarTerrain"
{
    Properties {
	_TransitionFalloff ("Transition Falloff", Range(0.01, 10.0)) = 4.0
        [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
        [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        [HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
        [HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
        [HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
        [HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
        // used in fallback on old cards & base map
        [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
    }

    CGINCLUDE
        #pragma surface surf Lambert vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer noinstancing
        #pragma multi_compile_fog
        #include "TerrainSplatmapCommon.cginc"

	float _TransitionFalloff;

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 splat_control; // Determines where each of the textures show up and by how much
            half weight; // A bit complicated thing, But it supports more than 4 textures to be used on the terrain by providing o.Alpha with weight value.
            fixed4 mixedDiffuse; //Will be the color output after putting on all the splat maps
            SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal); //The actual function that puts the splat map texture on the terrain, Outputs color value into mixedDiffuse.

	    fixed3 col1 = tex2D(_Splat0, IN.uv_Splat0); // Takes color from from first texture applied on Terrain
	    fixed3 col2 = tex2D(_Splat1, IN.uv_Splat1); // Takes color from from second texture applied on Terrain
	    float upwardNormalStr = dot(half3(0, 1, 0), o.Normal) * 0.5 + 0.5; // How much the normal at that point faces upwards. Changed from -1.0 to +1.0 range to 0.0 to +1.0
	    upwardNormalStr = pow(upwardNormalStr, _TransitionFalloff); // Larger _TransitionFalloff makes a sharper transition
	    fixed4 triPlanarColor = fixed4(lerp(col1, col2, upwardNormalStr), 1.0); // Chooses the appropriate color based on the normal
	    /*splat_control is used to determine which texture goes where.
	    The Red & Green channel strength determines where & how much the _Splat0 & _Splat1 textures show up.
	    Since we are manually overriding the placement of the first two textures we have to perform some check*/
	    float extentOfRedAndGreen = dot(half2(1,1), splat_control.rg) * 0.5 + 0.5; // 0 :- Means no Red or Green, 1:- Means alot of Red or Green
	    extentOfRedAndGreen = pow(extentOfRedAndGreen, 4); // Making the value sharper to prevent artifacts
	    o.Albedo = lerp(mixedDiffuse, triPlanarColor, extentOfRedAndGreen);
            o.Alpha = weight; // To support more than 4 textures, Not sure exactly how though.
        }
    ENDCG

    Category 
	{
        Tags 
		{
            "Queue" = "Geometry-99"
            "RenderType" = "Opaque"
        }
        // TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile __ _TERRAIN_NORMAL_MAP
            ENDCG
        }
        SubShader { // for sm2.0 targets
            CGPROGRAM
            ENDCG
        }
    }

    Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Diffuse-AddPass"
    Dependency "BaseMapShader" = "Diffuse"
    Dependency "Details0"      = "Hidden/TerrainEngine/Details/Vertexlit"
    Dependency "Details1"      = "Hidden/TerrainEngine/Details/WavingDoublePass"
    Dependency "Details2"      = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
    Dependency "Tree0"         = "Hidden/TerrainEngine/BillboardTree"

    Fallback "Diffuse"
}
