using StarterAssets.States;
using UnityEngine;

public class SwimState : MonoBehaviour, IState
{
    private GameObject _waterObject;
    private float _desiredY;
    private float _waterSurfaceHeight;
    
    private PlayerProperties _properties;
    
    void Start()
    {
        _properties = GetComponent<PlayerProperties>();
    }
    
    public State Name => State.Swim;

    public bool Check()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _properties.GroundedRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Water"))
            {
                _waterObject = hitCollider.gameObject;
                return true;
            }
        }
        return false;
    }

    public void Initialize()
    {
        _waterSurfaceHeight = _waterObject.transform.position.y + _waterObject.GetComponent<Collider>().bounds.extents.y;
        AudioSource.PlayClipAtPoint(_properties.LandingWaterAudioClip, transform.TransformPoint(_properties.Controller.center), _properties.LandingAudioVolume);
        _properties.Animator.SetBool(_properties.AnimIDSwim, true);
    }

    public void Do(float deltaTime)
    {
        float playerHeight = _properties.Controller.bounds.extents.y;
        float desiredY = _waterSurfaceHeight - _properties.SwimDepth * playerHeight;
        float newYPosition = Mathf.Lerp(transform.position.y, desiredY, 0.1f);
        float movementY = newYPosition - transform.position.y;
        
        MovementFunctions.Move(_properties, transform, true, movementY);
    }

    public void End()
    {
        _waterObject = null;
        _waterSurfaceHeight = 0.0f;
        _properties.Animator.SetBool(_properties.AnimIDSwim, false);
    }
}
