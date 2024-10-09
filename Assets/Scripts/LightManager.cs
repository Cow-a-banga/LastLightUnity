using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    [Tooltip("Глобальное освещение")]
    public Light directionalLight;

    [Tooltip("Свет над игроком")]
    public Light pointLight;

    [Tooltip("Клавиша переключения")]
    public KeyCode switchKey = KeyCode.L;

    private DialogueVariables _dialogueVariables;
    private bool lightOn = true;
    private Light[] lights;

    void Start()
    {
        _dialogueVariables = GetComponent<DialogueManager>().DialogueVariables;
        lights = FindObjectsOfType<Light>();
    }

    private void SwitchLight()
    {
        lightOn = !lightOn;
        directionalLight.intensity = lightOn ? 1.0f : 0.001f; 
        pointLight.enabled = !lightOn;
        _dialogueVariables.SetVariable("light_on", new Ink.Runtime.BoolValue(lightOn));
        
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            SwitchLight();
        }

        if(((Ink.Runtime.BoolValue)_dialogueVariables.GetVariable("light_on")).value != lightOn)
        {
            SwitchLight();
        }  
        
        int pointLightCount = 0;
        Vector4[] lightPositions = new Vector4[4]; // Пример, если ты ожидаешь до 4 источников
        Vector4[] lightColors = new Vector4[4];

        foreach (Light light in lights)
        {
            if (light.type == LightType.Point && pointLightCount < 4) // Убедись, что не выйдешь за пределы массива
            {
                lightPositions[pointLightCount] = light.transform.position;
                lightColors[pointLightCount] = light.color;
                pointLightCount++;
            }
        }

        Shader.SetGlobalInt("_PointLightCount", pointLightCount);
        Shader.SetGlobalVectorArray("_LightPositions", lightPositions);
        Shader.SetGlobalVectorArray("_LightColors", lightColors);
    }
}
