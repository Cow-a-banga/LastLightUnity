using System.Collections;
using System.Collections.Generic;
using StarterAssets.States;
using UnityEngine;

public class FallState : MonoBehaviour, IState
{
    private PlayerProperties _properties;
    
    void Start()
    {
        _properties = GetComponent<PlayerProperties>();
    }
    
    public State Name => State.Fall;
    
    public bool Check()
    {
        return !MovementFunctions.IsGrounded(transform.position, _properties.GroundedOffset, _properties.GroundedRadius, _properties.GroundLayers);
    }

    public void Initialize()
    {
        _properties.Animator.SetBool(_properties.AnimIDFreeFall, true);
    }

    public void Do(float deltaTime)
    {
        if (_properties.VerticalVelocity < _properties.TerminalVelocity)
        {
            _properties.VerticalVelocity += _properties.Gravity * Time.deltaTime;
        }
        
        MovementFunctions.Move(_properties, transform);
    }

    public void End()
    {
        _properties.Animator.SetBool(_properties.AnimIDFreeFall, false);
    }
}
