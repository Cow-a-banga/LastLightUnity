Shader "Roystan/Toon/WaterWithPointLights"
{
    Properties
    {
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance", Float) = 1
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        _SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27
        _FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
        _FoamMinDistance("Foam Minimum Distance", Float) = 0.04
        _LightRange("Light Range", Float) = 10.0 // Новая переменная для максимального расстояния света
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define SMOOTHSTEP_AA 0.01

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1 - top.a);

                return float4(color, alpha);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float3 viewNormal : NORMAL;
                float3 worldPos : TEXCOORD3;  // Мировая позиция для освещения
            };

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float4 _FoamColor;

            float _DepthMaxDistance;
            float _FoamMaxDistance;
            float _FoamMinDistance;
            float _SurfaceNoiseCutoff;
            float _SurfaceDistortionAmount;

            float2 _SurfaceNoiseScroll;

            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;

            int _PointLightCount; // Количество точечных источников света
            float4 _LightPositions[4]; // Позиции источников света
            float4 _LightColors[4]; // Цвета источников света
            float _LightRange; // Дальность действия света

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.viewNormal = COMPUTE_VIEW_NORMAL;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Позиция в мире
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);
                float depthDifference = existingDepthLinear - i.screenPosition.w;
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
                float3 normalDot = saturate(dot(existingNormal, i.viewNormal));
                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
                float foamDepthDifference01 = saturate(depthDifference / foamDistance);

                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

                float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x,
                                        (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);
                float4 surfaceNoiseColor = _FoamColor;
                surfaceNoiseColor.a *= surfaceNoise;

                // Освещение точечными источниками
                float3 pointLightColor = float3(0, 0, 0); // Инициализация цвета от источников света

                for (int j = 0; j < _PointLightCount; j++)
                {
                    float3 lightDir = normalize(_LightPositions[j].xyz - i.worldPos); // Направление на источник света
                    float dist = length(_LightPositions[j].xyz - i.worldPos); // Расстояние до источника света
                    float attenuation = saturate(1.0 - (dist / _LightRange)); // Ослабление света по расстоянию

                    float diff = max(dot(i.viewNormal, lightDir), 0.0); // Диффузное освещение
                    pointLightColor += diff * _LightColors[j].rgb * attenuation; // Учитываем ослабление
                }

                // Затемнение, если света нет
                if (pointLightColor.r < 0.1) // Если очень темно
                {
                    waterColor.rgb *= 0.001; // Затемняем воду
                }

                // Совмещение пены с водой и светом
                float4 finalColor = alphaBlend(surfaceNoiseColor, waterColor);
                return float4(finalColor.rgb * pointLightColor, 1.0); // Возвращаем итоговый цвет
            }

            ENDCG
        }
    }
}
