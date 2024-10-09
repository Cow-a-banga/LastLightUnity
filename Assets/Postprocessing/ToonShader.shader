Shader "Hidden/Custom/ToonShader"
{
    HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _ThresholdMin;
        float _ThresholdAvg;
        float _ThresholdMax;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            // Получаем цвет пикселя
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            // Рассчитываем яркость на основе максимального значения RGB
            float brightness = max(color.r, max(color.g, color.b));

            // Toon shading: проверка уровней яркости и квантование
            if (brightness < _ThresholdMin)
            {
                brightness = 0.25; // Самый темный уровень
            }
            else if (brightness < _ThresholdAvg)
            {
                brightness = 0.5; // Средний уровень
            }
            else if (brightness < _ThresholdMax)
            {
                brightness = 0.75; // Светлый уровень
            }
            else
            {
                brightness = 1.0; // Самый яркий уровень
            }

            // Применяем квантованную яркость к цвету
            float3 toonColor = color.rgb * brightness;

            return float4(toonColor, color.a);
        }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
