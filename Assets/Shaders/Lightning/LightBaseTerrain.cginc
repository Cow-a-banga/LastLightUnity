#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

sampler2D _Control;
 
// Textures
sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;

sampler2D _MainTex;
float4 _MainTex_ST;
float3 _Tint;
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

    fixed4 splatControl = tex2D(_Control, i.uv);
    fixed4 prealbedo = splatControl.r * tex2D (_Splat0, i.uv * _Splat0_ST.xy);
    prealbedo += splatControl.g * tex2D(_Splat1, i.uv * _Splat1_ST.xy);
    prealbedo += splatControl.b * tex2D (_Splat2, i.uv * _Splat2_ST.xy);
    prealbedo += splatControl.a * tex2D (_Splat3, i.uv * _Splat3_ST.xy);

    float3 specularTint;
    float oneMinusReflectivity;
    float3 albedo = DiffuseAndSpecularFromMetallic(
        prealbedo.rgb, _Metallic, specularTint, oneMinusReflectivity
    );

    UnityIndirect indirectLight;
    indirectLight.diffuse = 0;
    indirectLight.specular = 0;
    
    return UNITY_BRDF_PBS(
        albedo, specularTint,
        oneMinusReflectivity, _Smoothness,
        i.normal, viewDir,
        CreateLight(i), indirectLight
    );
}
#endif