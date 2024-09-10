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

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            directionalLight.intensity = directionalLight.intensity > 0.5f ? 0.001f : 1.0f; 
            pointLight.enabled = !pointLight.enabled;
        }    
    }
}
