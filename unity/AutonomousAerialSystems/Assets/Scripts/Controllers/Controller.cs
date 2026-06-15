using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The unified base for controllers using the wing-camber-based system
/// </summary>
public class Controller : MonoBehaviour
{
    protected const float GRAVITY = 9.81f;

    protected Rigidbody _rb;
    protected Transform _com; //Center of mass
    protected AircraftInputActions _inputActions;

    #region Inputs
    protected float _rollInput;
    protected float _pitchInput;
    protected float _yawInput;
    protected float _flapInput;
    protected float _throttleInput;
    #endregion

    #region Control Surfaces
    protected ControlSurface _leftAileron;
    protected ControlSurface _rightAileron;
    protected ControlSurface _leftFlap;
    protected ControlSurface _rightFlap;
    protected ControlSurface _leftElevator;
    protected ControlSurface _rightElevator;
    protected ControlSurface _rudder;
    #endregion

    #region Wing Surfaces
    protected WingSurface _leftAileronParent;
    protected WingSurface _rightAileronParent;
    protected WingSurface _leftFlapParent;
    protected WingSurface _rightFlapParent;
    protected WingSurface _leftElevatorParent;
    protected WingSurface _rightElevatorParent;
    protected WingSurface _rudderParent;
    #endregion

    #region UI Display Text
    protected TextMeshProUGUI _speedText;
    protected TextMeshProUGUI _altitudeText;
    protected TextMeshProUGUI _stallingText;
    protected TextMeshProUGUI _gForceText;

    protected TextMeshProUGUI _leftAileronText;
    protected TextMeshProUGUI _rightAileronText;
    protected TextMeshProUGUI _leftFlapText;
    protected TextMeshProUGUI _rightFlapText;
    protected TextMeshProUGUI _leftElevatorText;
    protected TextMeshProUGUI _rightElevatorText;
    protected TextMeshProUGUI _rudderText;

    private int _numWingText;
    #endregion

    [Header("General Parameters")]
    public float WheelBrakeInput;
    public float AirDensityAtSeaLevel;
    public float Thrust;

    [Header("Stall Text")]
    public float StallTextThreshold;
    public float StallTextRedness;
    protected float _totalStall;

    [Header("G-Force")]
    public float GForce;
    protected Vector3 _previousVelocity;

    [Header("UI Display")]
    public Canvas UICanvas;

    protected void Awake()
    {
        _inputActions = new AircraftInputActions();
    }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _com = transform.Find("Aerodynamics").Find("CenterOfMass").transform;

        _previousVelocity = _rb.linearVelocity;
        _rb.centerOfMass = _com.localPosition;

        InitializeControlSurfaces();
        InitializeWingSurfaces();
        InitializeTextDisplays();
    }

    protected void Update()
    {
        UpdateDisplay();
        GetInput();
    }

    protected virtual void FixedUpdate()
    {
        DeflectControlSurfaces();
        ApplyThrust();

        CalculateGForce();
    }

    protected void OnEnable()
    {
        _inputActions.Enable();
    }

    /// <summary>
    /// Apply thrust force at the center of mass
    /// </summary>
    protected virtual void ApplyThrust()
    {
        Vector3 centerOfMass = transform.position + _rb.centerOfMass;
        Vector3 thrustForce = transform.forward * Thrust * _throttleInput;
        _rb.AddForceAtPosition(thrustForce, centerOfMass, ForceMode.Force);
    }

    /// <summary>
    /// Calculate the current G Force 
    /// </summary>
    protected void CalculateGForce()
    {
        Vector3 acceleration = (_rb.linearVelocity - _previousVelocity) / Time.fixedDeltaTime;
        Vector3 accelerationWithoutGravity = acceleration - Physics.gravity;

        GForce = Vector3.Dot(accelerationWithoutGravity, transform.up) / GRAVITY;
        _previousVelocity = _rb.linearVelocity;
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
    /// Gets all control surfaces from prefab
    /// </summary>
    protected void InitializeControlSurfaces()
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
    protected void InitializeWingSurfaces()
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
    /// Gets all texts from canvas
    /// </summary>
    protected virtual void InitializeTextDisplays()
    {
        Transform aircraftPanel = UICanvas.transform.Find("Aircraft Panel").transform;
        Transform wingPanel = UICanvas.transform.Find("Wing Panel").transform;

        _speedText = aircraftPanel.transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        _altitudeText = aircraftPanel.transform.Find("AltitudeText").GetComponent<TextMeshProUGUI>();
        _stallingText = aircraftPanel.transform.Find("StallingText").GetComponent<TextMeshProUGUI>();
        _gForceText = aircraftPanel.transform.Find("GForceText").GetComponent<TextMeshProUGUI>();

        _leftAileronText = wingPanel.Find("LeftAileronText").GetComponent<TextMeshProUGUI>();
        _rightAileronText = wingPanel.Find("RightAileronText").GetComponent<TextMeshProUGUI>();
        _leftFlapText = wingPanel.Find("LeftFlapText").GetComponent<TextMeshProUGUI>();
        _rightFlapText = wingPanel.Find("RightFlapText").GetComponent<TextMeshProUGUI>();
        _leftElevatorText = wingPanel.Find("LeftElevatorText").GetComponent<TextMeshProUGUI>();
        _rightElevatorText = wingPanel.Find("RightElevatorText").GetComponent<TextMeshProUGUI>();
        _rudderText = wingPanel.Find("RudderText").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Get the roll, pitch, yaw, flap and thrust input from the input action system
    /// </summary>
    protected virtual void GetInput()
    {
        _rollInput = _inputActions.AircraftWithFlaps.Roll.ReadValue<float>();
        _pitchInput = _inputActions.AircraftWithFlaps.Pitch.ReadValue<float>();
        _yawInput = _inputActions.AircraftWithFlaps.Yaw.ReadValue<float>();

        _flapInput = _inputActions.AircraftWithFlaps.Flap.ReadValue<float>();
        _throttleInput = _inputActions.AircraftWithFlaps.Thrust.ReadValue<float>();

        WheelBrakeInput = _inputActions.AircraftWithFlaps.WheelBrake.ReadValue<float>();
    }

    /// <summary>
    /// Update UI display text
    /// </summary>
    private void UpdateDisplay()
    {
        if (_speedText != null)
        {
            float speed = (float)Math.Round(_rb.linearVelocity.magnitude, 1);
            _speedText.text = $"Speed: {speed} m/s";
        }
        if (_altitudeText != null)
        {
            float altitude = Mathf.Round(transform.position.y);
            _altitudeText.text = $"Altitude: {altitude} m";
        }
        if (_stallingText != null)
        {
            float numTextModifier = _numWingText / 7f;
            if (_totalStall > StallTextThreshold * numTextModifier)
            {
                float stall = Mathf.Clamp01(_totalStall / (6f * numTextModifier * StallTextRedness));

                _stallingText.text = "STALLING";
                _stallingText.color = Color.Lerp(Color.white, Color.red, stall);
            }
            else
            {
                _stallingText.text = "";
            }
        }
        if (_gForceText != null)
        {
            float gForce = Mathf.Round(GForce);
            _gForceText.text = $"{gForce} G";

            float gForceStrength = 0f;
            if (GForce >= 0)
            {
                gForceStrength = Mathf.Clamp01((GForce - 1f) / 8f);
            }
            else
            {
                gForceStrength = Mathf.Clamp01(GForce / -4f);
            }

            _gForceText.color = Color.Lerp(Color.white, Color.red, gForceStrength);
        }

        _totalStall = 0f;
        _numWingText = 0;

        if (_leftAileronText != null)
        {
            WingSurface surface = _leftAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftAileronText.text = $"Left Aileron | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftAileronText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_leftFlapText != null)
        {
            WingSurface surface = _leftFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftFlapText.text = $"Left Flap | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftFlapText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_leftElevatorText != null)
        {
            WingSurface surface = _leftElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftElevatorText.text = $"Left Elevator | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftElevatorText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }

        if (_rudderText != null)
        {
            WingSurface surface = _rudderParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rudderText.text = $"Rudder | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _rudderText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }

        if (_rightAileronText != null)
        {
            WingSurface surface = _rightAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightAileronText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Aileron";
            _rightAileronText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_rightFlapText != null)
        {
            WingSurface surface = _rightFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightFlapText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Flap";
            _rightFlapText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_rightElevatorText != null)
        {
            WingSurface surface = _rightElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightElevatorText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Elevator";
            _rightElevatorText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
    }
}
