using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the second aircraft prototype
/// </summary>
public class SecondAircraftController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _com; //Center of mass
    private AircraftInputActions _inputActions;

    #region Inputs
    private float _rollInput;
    private float _pitchInput;
    private float _yawInput;
    private float _flapInput;
    private float _throttleInput;
    #endregion

    #region Control Surfaces
    private ControlSurface _leftAileron;
    private ControlSurface _rightAileron;
    private ControlSurface _leftFlap;
    private ControlSurface _rightFlap;
    private ControlSurface _leftElevator;
    private ControlSurface _rightElevator;
    private ControlSurface _rudder;
    #endregion

    #region Wing Surfaces
    private WingSurface _leftAileronParent;
    private WingSurface _rightAileronParent;
    private WingSurface _leftFlapParent;
    private WingSurface _rightFlapParent;
    private WingSurface _leftElevatorParent;
    private WingSurface _rightElevatorParent;
    private WingSurface _rudderParent;
    #endregion

    [Header("Parameters")]
    public float Thrust;
    public float AirDensityAtSeaLevel;

    [Header("UI Display")]
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI AltitudeText;

    public TextMeshProUGUI LeftAileronText;
    public TextMeshProUGUI RightAileronText;
    public TextMeshProUGUI LeftFlapText;
    public TextMeshProUGUI RightFlapText;
    public TextMeshProUGUI LeftElevatorText;
    public TextMeshProUGUI RightElevatorText;
    public TextMeshProUGUI RudderText;

    void Awake()
    {
        _inputActions = new AircraftInputActions();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _com = transform.Find("Aerodynamics").Find("CenterOfMass").transform;

        _rb.centerOfMass = _com.position;

        InitializeControlSurfaces();
        InitializeWingSurfaces();
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
    /// Gets all control surfaces from prefab
    /// </summary>
    private void InitializeControlSurfaces()
    {
        Transform pivots = transform.Find("Visual Components").Find("Pivots").transform;

        _leftAileron = pivots.Find("Left Aileron Pivot").GetComponent<ControlSurface>();
        _rightAileron = pivots.Find("Right Aileron Pivot").GetComponent<ControlSurface>();
        _leftFlap = pivots.Find("Left Flap Pivot").GetComponent<ControlSurface>();
        _rightFlap = pivots.Find("Right Flap Pivot").GetComponent<ControlSurface>();
        _leftElevator = pivots.Find("Left Elevator Pivot").GetComponent<ControlSurface>();
        _rightElevator = pivots.Find("Right Elevator Pivot").GetComponent<ControlSurface>();
        _rudder = pivots.Find("Rudder Pivot").GetComponent<ControlSurface>();
    }

    /// <summary>
    /// Gets all wing surfaces from prefab
    /// </summary>
    private void InitializeWingSurfaces()
    {
        Transform wingSurfaces = transform.Find("Aerodynamics").Find("Wing Surfaces").transform;

        _leftAileronParent = wingSurfaces.Find("Left Aileron Parent").GetComponent<WingSurface>();
        _rightAileronParent = wingSurfaces.Find("Right Aileron Parent").GetComponent<WingSurface>();
        _leftFlapParent = wingSurfaces.Find("Left Flap Parent").GetComponent<WingSurface>();
        _rightFlapParent = wingSurfaces.Find("Right Flap Parent").GetComponent<WingSurface>();
        _leftElevatorParent = wingSurfaces.Find("Left Elevator Parent").GetComponent<WingSurface>();
        _rightElevatorParent = wingSurfaces.Find("Right Elevator Parent").GetComponent<WingSurface>();
        _rudderParent = wingSurfaces.Find("Rudder Parent").GetComponent<WingSurface>();
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
        UpdateWingSurfaceData();
        VisualizeControlSurfaces();
    }

    /// <summary>
    /// Update the deflection angles in the wing surfaces
    /// </summary>
    private void UpdateWingSurfaceData()
    {
        _leftAileronParent.ControlSurfaceDeflection = _leftAileron.MaxDeflection * _rollInput;
        _rightAileronParent.ControlSurfaceDeflection = _rightAileron.MaxDeflection * -_rollInput;

        _leftFlapParent.ControlSurfaceDeflection = _leftFlap.MaxDeflection * _flapInput;
        _rightFlapParent.ControlSurfaceDeflection = _rightFlap.MaxDeflection * _flapInput;

        _leftElevatorParent.ControlSurfaceDeflection = _leftElevator.MaxDeflection * _pitchInput;
        _rightElevatorParent.ControlSurfaceDeflection = _rightElevator.MaxDeflection * _pitchInput;

        _rudderParent.ControlSurfaceDeflection = _rudder.MaxDeflection * _yawInput;
    }

    /// <summary>
    /// Visualize control surface deflection
    /// </summary>
    private void VisualizeControlSurfaces()
    {
        _leftAileron.DeflectSurface(_rollInput);
        _rightAileron.DeflectSurface(-_rollInput);

        _leftFlap.DeflectSurface(_flapInput);
        _rightFlap.DeflectSurface(_flapInput);

        _leftElevator.DeflectSurface(_pitchInput);
        _rightElevator.DeflectSurface(_pitchInput);

        _rudder.DeflectSurface(_yawInput);
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

        if (LeftAileronText != null)
        {
            WingSurface surface = _leftAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            LeftAileronText.text = $"Left Aileron | Lift: {lift} kN | Drag: {drag} kN | Stall: {stall}%";
            LeftAileronText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }
        if (LeftFlapText != null)
        {
            WingSurface surface = _leftFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            LeftFlapText.text = $"Left Flap | Lift: {lift} kN | Drag: {drag} kN | Stall: {stall}%";
            LeftFlapText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }
        if (LeftElevatorText != null)
        {
            WingSurface surface = _leftElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            LeftElevatorText.text = $"Left Elevator | Lift: {lift} kN | Drag: {drag} kN | Stall: {stall}%";
            LeftElevatorText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }

        if (RudderText != null)
        {
            WingSurface surface = _rudderParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            RudderText.text = $"Rudder | Lift: {lift} kN | Drag: {drag} kN | Stall: {stall}%";
            RudderText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }

        if (RightAileronText != null)
        {
            WingSurface surface = _rightAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            RightAileronText.text = $"Stall: {stall}% | Drag: {drag} kN | Lift: {lift} kN | Right Aileron";
            RightAileronText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }
        if (RightFlapText != null)
        {
            WingSurface surface = _rightFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            RightFlapText.text = $"Stall: {stall}% | Drag: {drag} kN | Lift: {lift} kN | Right Flap";
            RightFlapText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }
        if (RightElevatorText != null)
        {
            WingSurface surface = _rightElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);

            RightElevatorText.text = $"Stall: {stall}% | Drag: {drag} kN | Lift: {lift} kN | Right Elevator";
            RightElevatorText.color = Color.Lerp(Color.white, Color.red, stall / 100f);
        }
    }
}
