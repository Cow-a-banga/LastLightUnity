using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LightSwitcher : MonoBehaviour
{
    public Color DayDirectionalLight;
    public Color DayAmbientLight;
    public Color NightDirectionalLight;
    public Color NightAmbientLight;

    [Tooltip("Глобальное освещение")]
    public Light directionalLight;

    [Tooltip("Свет над игроком")]
    public Light pointLight;

    [Tooltip("Клавиша переключения")]
    public KeyCode switchKey = KeyCode.L;

    private DialogueVariables _dialogueVariables;
    private bool lightOn = true;
    private Light[] lights;

    void OnEnable()
    {
        // Подписываемся на событие обновления окна сцены
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        // Отписываемся от события обновления окна сцены
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void Start()
    {
        _dialogueVariables = GetComponent<DialogueManager>().DialogueVariables;
        lights = FindObjectsOfType<Light>();
    }

    private void SwitchLight()
    {
        lightOn = !lightOn;

        if (lightOn)
        {
            directionalLight.color = DayDirectionalLight;
            RenderSettings.ambientLight = DayAmbientLight;
        }
        else
        {
            directionalLight.color = NightDirectionalLight;
            RenderSettings.ambientLight = NightAmbientLight;
        }
        pointLight.enabled = !lightOn;
        _dialogueVariables.SetVariable("light_on", new Ink.Runtime.BoolValue(lightOn));
    }

    void OnSceneGUI(SceneView sceneView)
    {
        // Проверяем событие нажатия клавиши только в режиме редактирования
        if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == switchKey)
        {
            Debug.Log("Key pressed: " + Event.current.keyCode);
            SwitchLight();
            // Обновляем сцену, чтобы изменения применились
            SceneView.RepaintAll();
            // Обработка события, чтобы предотвратить дальнейшее распространение
            Event.current.Use();
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKeyDown(switchKey))
            {
                Debug.Log("hey");
                SwitchLight();
            }

            if (((Ink.Runtime.BoolValue)_dialogueVariables.GetVariable("light_on")).value != lightOn)
            {
                SwitchLight();
            }
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
    