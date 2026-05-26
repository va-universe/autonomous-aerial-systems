using System;
using TMPro;
using UnityEngine;

public class SecondAircraftController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _com; //Center of mass
    private AircraftInputActions _inputActions;

    private float _rollInput;
    private float _pitchInput;
    private float _yawInput;

    private float _flapInput;
    private float _throttleInput;

    [Header("Parameters")]
    public float Thrust;
    public float AirDensityAtSeaLevel;

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
        _com = transform.Find("Aerodynamics").Find("CenterOfMass").transform;

        _rb.centerOfMass = _com.position;
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
    /// Get the roll, pitch, yaw, flap and thrust input from the input action system
    /// </summary>
    private void GetInput()
    {
        _rollInput = _inputActions.AircraftWithFlaps.Roll.ReadValue<float>();
        _pitchInput = _inputActions.AircraftWithFlaps.Pitch.ReadValue<float>();
        _yawInput = _inputActions.AircraftWithFlaps.Yaw.ReadValue<float>();

        _flapInput = _inputActions.AircraftWithFlaps.Flap.ReadValue<float>();
        _throttleInput = _inputActions.Aircraft.Thrust.ReadValue<float>();
    }

    /// <summary>
    /// Deflect all control surfaces visually and mathematically
    /// </summary>
    private void DeflectControlSurfaces()
    {

    }

    /// <summary>
    /// Apply thrust force at the center of mass
    /// </summary>
    private void ApplyThrust()
    {
        Vector3 centerOfMass = _com.position;
        Vector3 thrustForce = transform.forward * Thrust * _throttleInput;
        _rb.AddForceAtPosition(thrustForce, centerOfMass, ForceMode.Force);
    }

    /// <summary>
    /// Update UI display text
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
