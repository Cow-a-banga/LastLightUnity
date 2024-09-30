using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StarterAssets;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    //TODO: Есть проблема с переходов Grounded -> Fall в анимации (нет аниамции падения)
    //TODO: Узнать у GPT как лучше сделать: использовать порядок, писать взаимоисключающие условия или для каждого стейта прописывать куда он может перейти
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !_properties.LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _properties.BottomClamp, _properties.TopClamp);

        // Cinemachine will follow this target
        _properties.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _properties.CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private IList<IState> _states;
    private IState _currentState;
    private StarterAssetsInputs _input;
    private PlayerProperties _properties;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _properties = GetComponent<PlayerProperties>(); 
        _cinemachineTargetYaw = _properties.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _input = GetComponent<StarterAssetsInputs>();
        _states = GetComponents<IState>().OrderBy(x => x.Name).ToList();
        _currentState = null;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var state in _states)
        {
            if (!state.Check()) continue;
            if (state.Name == _currentState?.Name) break;
            _currentState?.End();
            _currentState = state;
            _currentState.Initialize();
            break;
        }
        _currentState?.Do(Time.deltaTime);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
}
