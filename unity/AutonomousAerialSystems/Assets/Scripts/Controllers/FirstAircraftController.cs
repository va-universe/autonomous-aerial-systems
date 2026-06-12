using System;
using TMPro;
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

    [Header("UI Display")]
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI AltitudeText;

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
        UpdateText();
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
        _rb.AddForceAtPosition(transform.forward * Thrust * _thrustInput, CenterOfMass.position, ForceMode.Force);
    }

    /// <summary>
    /// Update display text with current parameters
    /// </summary>
    private void UpdateText()
    {
        if (SpeedText != null)
        {
            float speed = (float)Math.Round(_rb.linearVelocity.magnitude, 1);
            SpeedText.text = $"Speed: {speed} m/s";
        }
        if (AltitudeText != null)
        {
            float altitude = Mathf.Round(transform.position.y);
            AltitudeText.text = $"Altitude: {altitude} m";
        }
    }
}
