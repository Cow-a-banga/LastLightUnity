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
            directionalLight.enabled = !directionalLight.enabled;
            pointLight.enabled = !pointLight.enabled;
        }    
    }
}
