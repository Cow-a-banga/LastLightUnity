#if !defined(MY_CELSHADING_INCLUDED)
#define MY_CELSHADING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"
			
sampler2D _MainTex;
float4 _MainTex_ST;
float4 _Tint;
float _Metallic;
float _Smoothness;

sampler2D _Texture1, _Texture2, _Texture3, _Texture4;

struct VertexData {
    float4 position : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};
			
struct Interpolators {
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 worldPos : TEXCOORD2;
};

Interpolators MyVertexProgram (VertexData v) {
    Interpolators i;
    i.position = UnityObjectToClipPos(v.position);
    i.worldPos = mul(unity_ObjectToWorld, v.position);
    i.normal = normalize(UnityObjectToWorldNormal(v.normal));
    i.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return i;
}

UnityLight CreateLight (Interpolators i) {
    UnityLight light;
    
    #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
        light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
    #else
        light.dir = _WorldSpaceLightPos0.xyz;
    #endif
    
    UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
    light.color = _LightColor0.rgb * attenuation;
    light.ndotl = DotClamped(i.normal, light.dir);
    return light;
}
			
float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
    i.normal = normalize(i.normal);
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
    
    //float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

    // float3 specularTint;
    // float oneMinusReflectivity;
    // albedo = DiffuseAndSpecularFromMetallic(
    //     albedo, _Metallic, specularTint, oneMinusReflectivity
    // );
    //
    // UnityIndirect indirectLight;
    // indirectLight.diffuse = 0;
    // indirectLight.specular = 0;


    //----------------------

    UnityLight light = CreateLight(i);
    float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
    i.normal = normalize(i.normal);

    float NdotL = DotClamped(light.dir, i.normal);
    float levels = 2.0;
    float stepV = 1.0 / levels;
    NdotL = floor(NdotL / stepV) * stepV;
    
    
    //float3 diffuse = albedo * light.color * DotClamped(light.dir, i.normal);

    float3 celShadedDiffuse = albedo * light.color * NdotL;
    return float4(celShadedDiffuse, 1);
    

    //----------------------
    
    // return UNITY_BRDF_PBS(
    //     albedo, specularTint,
    //     oneMinusReflectivity, _Smoothness,
    //     i.normal, viewDir,
    //     CreateLight(i), indirectLight
    // );
}
#endif