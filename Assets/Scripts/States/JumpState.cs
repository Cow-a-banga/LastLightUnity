using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using StarterAssets.States;
using UnityEngine;

public class JumpState : MonoBehaviour, IState
{
    private PlayerProperties _properties;

    public State Name => State.Jump;
    
    void Start()
    {
        _properties = GetComponent<PlayerProperties>();
    }
    
    public bool Check()
    {
        //В первый раз не работает _input.jump возможно из-за основного контроллера
        return _properties.Input.jump && _properties.Stamina >= _properties.JumpStamina;
    }

    public void Initialize()
    {
        _properties.Stamina -= _properties.JumpStamina;
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _properties.VerticalVelocity = Mathf.Sqrt(_properties.JumpHeight * -2f * _properties.Gravity);
        
        _properties.Animator.SetBool(_properties.AnimIDJump, true);
        _properties.Input.jump = false;
    }

    public void Do(float deltaTime)
    {
        MovementFunctions.Move(_properties, transform);
    }

    public void End()
    {
        _properties.Animator.SetBool(_properties.AnimIDJump, false);
    }
}
