using UnityEngine;

/// <summary>
/// The controller of the first aircraft prototype
/// </summary>
public class FirstAircraftController : MonoBehaviour
{
    private Rigidbody _rb;
    private AircraftInputActions _inputActions;

    private float _rollInput;
    private float _pitchInput;
    private float _yawInput;

    private float _thrustInput;

    [Header("Parameters")]
    public float Thrust;
    public float AirDensityAtSeaLevel;
    public Transform CenterOfMass;

    [Header("Control Surfaces")]
    public ControlSurface LeftAileron;
    public ControlSurface RightAileron;
    public ControlSurface LeftElevator;
    public ControlSurface RightElevator;
    public ControlSurface Rudder;

    void Awake()
    {
        _inputActions = new AircraftInputActions();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = CenterOfMass.position;
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        DeflectControlSurfaces();
        ApplyThrust();
    }

    void OnEnable()
    {
        _inputActions.Enable();
    }

    /// <summary>
    /// Get the roll, pitch and yaw input from the input action system
    /// </summary>
    private void GetInput()
    {
        _rollInput = _inputActions.Aircraft.Roll.ReadValue<float>();
        _pitchInput = _inputActions.Aircraft.Pitch.ReadValue<float>();
        _yawInput = _inputActions.Aircraft.Yaw.ReadValue<float>();

        _thrustInput = _inputActions.Aircraft.Thrust.ReadValue<float>();

        //Debug.Log($"Roll: {_rollInput}, Pitch: {_pitchInput}, Yaw: {_yawInput}");
    }

    /// <summary>
    /// Deflect all control surfaces based on inputs
    /// </summary>
    private void DeflectControlSurfaces()
    {
        LeftAileron.DeflectSurface(_rollInput);
        RightAileron.DeflectSurface(-_rollInput);

        LeftElevator.DeflectSurface(_pitchInput);
        RightElevator.DeflectSurface(_pitchInput);

        Rudder.DeflectSurface(_yawInput);
    }

    /// <summary>
    /// Apply thrust to this aircraft based on inputs
    /// </summary>
    private void ApplyThrust()
    {
        _rb.AddForce(transform.forward * Thrust * _thrustInput, ForceMode.Force);
    }
}
