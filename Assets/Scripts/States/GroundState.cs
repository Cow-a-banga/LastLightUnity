using System;
using StarterAssets;
using StarterAssets.States;
using UnityEngine;

public class GroundState : MonoBehaviour, IState
{
    private PlayerProperties _properties;
    
    //Может можно удалить
    private float _rotationVelocity;
    
    void Start()
    {
        _properties = GetComponent<PlayerProperties>();
    }
    
    public State Name => State.Grounded;
    
    public bool Check()
    {
        return MovementFunctions.IsGrounded(transform.position, _properties.GroundedOffset, _properties.GroundedRadius, _properties.GroundLayers);
    }

    public void Initialize()
    {
        _properties.Animator.SetBool(_properties.AnimIDGrounded, true);
    }

    public void Do(float deltaTime)
    {
        _properties.Stamina = Math.Min(_properties.Stamina + _properties.StaminaRegenPerSecond * deltaTime, _properties.MaxStamina);
        
        MovementFunctions.Move(_properties, transform);
    }

    public void End()
    {
        _properties.Animator.SetBool(_properties.AnimIDGrounded, false);
    }
}
