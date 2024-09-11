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

    void Start()
    {
        _dialogueVariables = GetComponent<DialogueManager>().DialogueVariables;
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
    }
}
