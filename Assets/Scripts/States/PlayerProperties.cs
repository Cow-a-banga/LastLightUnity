using System;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public enum StepType
{
    Grass,
    Earth,
    Rock
}

public class PlayerProperties : MonoBehaviour
{
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public StepType StepType;
        public AudioClip LandingGrassAudioClip;
        public AudioClip LandingEarthAudioClip;
        public AudioClip LandingWaterAudioClip;
        public AudioClip LandingRockAudioClip;
        public AudioClip SwimAudioClip;
        public AudioClip WaterJumpAudioClip;
        [Range(0,1)]
        public float LandingAudioVolume = 0.5f;
        public AudioClip[] FootstepGrassAudioClips;
        public AudioClip[] FootstepEarthAudioClips;
        public AudioClip[] FootstepWaterAudioClips;
        public AudioClip[] FootstepRockAudioClips;
        [Range(0,1)]
        public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Tooltip("Gravity when gliding")]
        public float GlidingGravity = -2.0f;
        
        [Tooltip("Terminal velocity")]
        public float TerminalVelocity = 53.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Swimming")]
        [Tooltip("Move speed of the character while swimming in m/s")]
        public float SwimSpeed = 1.5f;

        [Range(0,1)]
        [Tooltip("The depth at which the player should stay while swimming")]
        public float SwimDepth = 0.5f;

        [Tooltip("The vertical force applied to keep the player at the swim depth")]
        public float FloatForce = 5.0f;

        [Header("Climbing")]
        [Tooltip("Raycast to wall distance")]
        public float RaycastDistance = 3.0f;

        [Tooltip("Length of raycast timeout when jumping away from wall")]
        public float RaycastTimeout = 1.0f;
        
        [Tooltip("Climbing speed")]
        public float ClimbingSpeed = 10.0f;

        [Tooltip("Power of jumping away from the wall")]
        public float ClimbingJumpPower = 30.0f;

        [Header("Stamina")]
        public float MaxStamina = 100.0f;
        public float StaminaRegenPerSecond = 20.0f;
        public float FlyStaminaPerSecond = 5.0f;
        public float JumpStamina = 10.0f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

    public float Speed { get; set; }
    public float Stamina { get; set; }
    public float VerticalVelocity { get; set; }
    
    
    public PlayerProperties Properties { get; set; }
    public CharacterController Controller { get; set; }
    public StarterAssetsInputs Input { get; set; }
    public Animator Animator { get; set; }
    public GameObject MainCamera { get; set; }

    [HideInInspector]
    public float AnimationBlend;
    [HideInInspector]
    public float TargetRotation;
    [HideInInspector]
    public float RotationVelocity;
    
    public int AnimIDSpeed { get; set; }
    public int AnimIDMotionSpeed { get; set; }
    public int AnimIDFreeFall { get; set; }
    public int AnimIDJump { get; set; }
    public int AnimIDGrounded { get; set; }
    public int AnimIDSwim { get; set; }
    
    //Может можно удалить
    private float _rotationVelocity;
    
    void Start()
    {
        Animator = GetComponent<Animator>();
        Properties = GetComponent<PlayerProperties>();
        Controller = GetComponent<CharacterController>();
        Input = GetComponent<StarterAssetsInputs>();
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        
        AnimIDSpeed = Animator.StringToHash("Speed");
        AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        AnimIDFreeFall = Animator.StringToHash("FreeFall");
        AnimIDJump = Animator.StringToHash("Jump");
        AnimIDGrounded = Animator.StringToHash("Grounded");
        AnimIDSwim = Animator.StringToHash("Swim");
    }
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        AudioClip[] footstepAudioClips = StepType switch
        {
            StepType.Grass => FootstepGrassAudioClips,
            StepType.Earth => FootstepEarthAudioClips,
            StepType.Rock => FootstepRockAudioClips,
            _ => Array.Empty<AudioClip>(),
        };
        if (footstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        AudioClip landingAudioClip = StepType switch
        {
            StepType.Grass => LandingGrassAudioClip,
            StepType.Earth => LandingEarthAudioClip,
            StepType.Rock => LandingRockAudioClip,
            _ => null,
        };

        AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(Controller.center), LandingAudioVolume);
    }

    private void OnSwim(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(SwimAudioClip, transform.TransformPoint(Controller.center), LandingAudioVolume);
    }
}
