using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Shader = UnityEngine.Shader;

[Serializable]
[PostProcess(typeof(ToonShadingRenderer), PostProcessEvent.AfterStack, "Custom/Toon")]
public class ToonShading : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Toon threshold min")]
    public FloatParameter min = new() { value = 0.3f };
    [Range(0f, 1f), Tooltip("Toon threshold avg")]
    public FloatParameter avg = new() { value = 0.6f };
    [Range(0f, 1f), Tooltip("Toon threshold max")]
    public FloatParameter max = new() { value = 0.9f };
}

public class ToonShadingRenderer : PostProcessEffectRenderer<ToonShading>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ToonShader"));
        sheet.properties.SetFloat("_ThresholdMin", settings.min);
        sheet.properties.SetFloat("_ThresholdAvg", settings.avg);
        sheet.properties.SetFloat("_ThresholdMax", settings.max);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}