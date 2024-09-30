using StarterAssets.States;
using UnityEngine;

public class FlyState : MonoBehaviour, IState
{
    private PlayerProperties _properties;
    
    void Start()
    {
        _properties = GetComponent<PlayerProperties>();
    }
    
    public State Name => State.Fly;
    
    public bool Check()
    {
        return _properties.Input.fly && _properties.VerticalVelocity < 0.0f && _properties.Stamina > _properties.FlyStaminaPerSecond;
    }

    public void Initialize()
    {
        _properties.Animator.SetBool(_properties.AnimIDFreeFall, true);
    }

    public void Do(float deltaTime)
    {
        _properties.Stamina -= _properties.FlyStaminaPerSecond * deltaTime;
        
        if (_properties.VerticalVelocity < _properties.TerminalVelocity)
        {
            _properties.VerticalVelocity += _properties.GlidingGravity * deltaTime;
        }
        
        MovementFunctions.Move(_properties, transform);
    }

    public void End()
    {
        _properties.Animator.SetBool(_properties.AnimIDFreeFall, false);
    }
}
